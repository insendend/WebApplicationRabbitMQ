using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ClassesLib.Sockets
{
    public class TcpServer
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

        public async Task Start()
        {
            try
            {
                server.Bind(endPoint);

                server.Listen(clientCount);
                Console.WriteLine("Wait for clients...");
                while (true)
                {
                    var client = await Task.Run(() => server.Accept());
                    Console.WriteLine($"Client '{client.RemoteEndPoint}' connected.");

                    var buf = new byte[4096];
                    var recv = await Task.Run(() => client.Receive(buf));

                    var msg = Encoding.Unicode.GetString(buf, 0, recv);

                    var taskInfo = JsonConvert.DeserializeObject<TaskInfo>(msg);

                    if (taskInfo is null) return;

                    taskInfo.AddHours(1);

                    var newMsg = JsonConvert.SerializeObject(taskInfo);
                    buf = Encoding.Unicode.GetBytes(newMsg);

                    var sent = await Task.Run(() => client.Send(buf));
                    Console.WriteLine($"Sent {sent} bytes");
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine("SocketException");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void Close()
        {
            server.Close();
            server = null;
        }
    }
}
