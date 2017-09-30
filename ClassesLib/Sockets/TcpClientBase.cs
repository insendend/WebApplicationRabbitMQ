using System;
using System.IO;
using System.Net.Sockets;
using ClassesLib.Sockets.Settings;

namespace ClassesLib.Sockets
{
    public abstract class TcpClientBase : IDisposable
    {
        protected Socket client;
        protected readonly TcpClientSettings settings;

        protected TcpClientBase(TcpClientSettings settings)
        {
            this.settings = settings;
        }

        public void Connect()
        {
            client = new Socket(
                settings.AddressFamily,
                settings.SocketType,
                settings.ProtocolType);

            client.Connect(settings.EndPoint);
        }

        public int Send(byte[] msgBytes)
        {
            return client.Send(msgBytes);
        }

        public byte[] Receive()
        {
            using (var ms = new MemoryStream())
            {
                var buff = new byte[4096];

                while (true)
                {
                    var recv = client.Receive(buff);
                    ms.Write(buff, 0, recv);

                    if (recv < buff.Length)
                        break;

                    Array.Clear(buff, 0, recv);
                }
                return ms.ToArray();
            }
        }

        public void Dispose()
        {
            client?.Dispose();
        }
    }
}
