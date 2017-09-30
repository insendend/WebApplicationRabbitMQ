using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using ClassesLib.Sockets.Settings;

namespace ClassesLib.Sockets
{
    public abstract class TcpServerBase : IDisposable
    {
        protected TcpServerSettings settings;
        protected Socket server;
        protected List<Socket> clients;

        protected TcpServerBase(TcpServerSettings settings)
        {
            clients = new List<Socket>();
            this.settings = settings;
        }

        public virtual async void Start()
        {
            try
            {
                server = new Socket(
                    settings.AddressFamily,
                    settings.SocketType,
                    settings.ProtocolType);

                server.Bind(settings.EndPoint);
                server.Listen(settings.ClientCount);
                Console.WriteLine($"Awaiting for TCP requests at {server.LocalEndPoint}...");

                while (true)
                {
                    var client = await Task.Run(() => server.Accept());
                    Console.WriteLine($"Client '{client.RemoteEndPoint}' connected.");

                    // receive full message
                    var recvBytes = await Receive(client);
                    Console.WriteLine($"Received from client: {recvBytes.Length} bytes");

                    // message processing
                    var res = ReceivedDataHandler(recvBytes);

                    // send back
                    var sentBytes = await Task.Run(() => client.Send(res));
                    Console.WriteLine($"Sent to client: {sentBytes} bytes");
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.Message);

                foreach (var client in clients)
                {
                    if (client.Connected)
                        continue;

                    Console.WriteLine($"Client {client.RemoteEndPoint} disconnected");
                    client.Dispose();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
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

        protected abstract byte[] ReceivedDataHandler(byte[] data);

        public void Stop()
        {
            Dispose();
        }

        public void Dispose()
        {
            server?.Dispose();

            if (clients is null) return;

            foreach (var client in clients)
            {
                client?.Disconnect(false);
                client?.Dispose();
            }
        }
    }
}
