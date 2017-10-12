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
        private Socket _client;
        private readonly TcpClientSettings _settings;

        protected TcpClientBase(TcpClientSettings settings)
        {
            _settings = settings;
        }

        public async Task<int> SendAsync(byte[] msgBytes)
        {
            _client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            await _client.ConnectAsync(new IPEndPoint(_settings.Ip, _settings.Port));
            return _client.Connected ? await _client.SendAsync(msgBytes, SocketFlags.None) : 0;
        }

        public async Task<byte[]> ReceiveAsync()
        {
            using (var ms = new MemoryStream())
            {
                var buff = new byte[4096];

                while (true)
                {
                    var recv = await _client.ReceiveAsync(buff, SocketFlags.None);
                    await ms.WriteAsync(buff, 0, recv);

                    if (recv < buff.Length)
                        break;

                    Array.Clear(buff, 0, recv);
                }
                return ms.ToArray();
            }
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}
