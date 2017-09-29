using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using ClassesLib.Serialization;

namespace ClassesLib.Sockets
{
    public class TcpServer : IServer
    {
        private Socket server;
        private IPEndPoint endPoint;
        private const int clientCount = 65535;
        private List<Socket> clients;

        public TcpServer(IPAddress ip, int port)
        {
            clients = new List<Socket>();
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            endPoint = new IPEndPoint(ip, port);
        }

        public async void Start()
        {
            try
            {
                server.Bind(endPoint);
                server.Listen(clientCount);
                Console.WriteLine($"Awaiting for TCP requests at {server.LocalEndPoint}...");

                while (true)
                {
                    var client = await Task.Run(() => server.Accept());
                    Console.WriteLine($"Client '{client.RemoteEndPoint}' connected.");

                    var recvBytes = await Receive(client);
                    Console.WriteLine($"Received from client: {recvBytes.Length} bytes");

                    ISerializer<TaskInfo> serializer = new TaskInfoSerializer();
                    var taskInfo = serializer.Desirialize(recvBytes);

                    if (taskInfo is null) return;

                    taskInfo.AddHours(1);

                    var objAsBytes = serializer.Serialize(taskInfo);

                    var sentBytes = await Task.Run(() => client.Send(objAsBytes));
                    Console.WriteLine($"Sent to client: {sentBytes} bytes");
                }
            }
            catch (SocketException)
            {
                Console.WriteLine("SocketException");

                foreach (var client in clients)
                {
                    if (!client.Connected)
                    {
                        Console.WriteLine($"Client {client.RemoteEndPoint} is off");
                        client.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        private async Task<byte[]> Receive(Socket client)
        {
            using (var ms = new MemoryStream())
            {
                var buff = new byte[4096];

                while (true)
                {
                    var recv = await Task.Run(() => client.Receive(buff));
                    await ms.WriteAsync(buff, 0, recv);

                    if (recv < buff.Length)
                        break;

                    Array.Clear(buff, 0, recv);
                }

                return ms.ToArray();
            }
        }

        public void Close()
        {
            if (server is null)
                return;

            server.Close();
            server = null;
        }
    }
}
