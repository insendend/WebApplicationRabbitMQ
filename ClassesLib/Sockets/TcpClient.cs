using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ClassesLib.Sockets
{
    public class TcpClient : IClient
    {
        private Socket client;

        public TcpClient()
        {
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
        }

        public void Connect(IPEndPoint endPoint)
        {
            client.Connect(endPoint);
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
                    ms.WriteAsync(buff, 0, recv);

                    if (recv < buff.Length)
                        break;

                    Array.Clear(buff, 0, recv);
                }

                return ms.ToArray();
            }
        }

        public void Close()
        {
            client?.Close();
            client = null;
        }
    }
}
