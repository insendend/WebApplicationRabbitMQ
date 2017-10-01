using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using ClassesLib.Sockets.Settings;

namespace ClassesLib.Sockets.Client
{
    public abstract class TcpClientBase : IDisposable
    {
        private readonly Socket client;
        private readonly TcpClientSettings settings;

        protected TcpClientBase(TcpClientSettings settings)
        {
            this.settings = settings;
            client = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.IP);
        }

        public async Task<int> SendAsync(byte[] msgBytes)
        {
            try
            {
                await client.ConnectAsync(new IPEndPoint(settings.Ip, settings.Port));
                return client.Connected ? await client.SendAsync(msgBytes, SocketFlags.None) : 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 0;
            }
        }

        public async Task<byte[]> ReceiveAsync()
        {
            try
            {
                using (var ms = new MemoryStream())
                {
                    var buff = new byte[4096];

                    while (true)
                    {
                        var recv = await client.ReceiveAsync(buff, SocketFlags.None);
                        await ms.WriteAsync(buff, 0, recv);

                        if (recv < buff.Length)
                            break;

                        Array.Clear(buff, 0, recv);
                    }
                    return ms.ToArray();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public void Dispose()
        {
            client?.Dispose();
        }
    }
}
