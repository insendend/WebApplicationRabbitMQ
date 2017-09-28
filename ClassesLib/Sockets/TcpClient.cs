using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ClassesLib.Sockets
{
    public class TcpClient
    {
        private Socket client;
        private IPEndPoint endPoint;

        public TcpClient(IPAddress ip, int port)
        {
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            endPoint = new IPEndPoint(ip, port);
        }

        public int Send(string msg)
        {
            try
            {
                client.Connect(endPoint);
                Console.WriteLine("Connected");

                return client.Send(Encoding.Unicode.GetBytes(msg));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return -1;
            }
        }

        public string Receive()
        {
            var buf = new byte[4096];

            var recv = client.Receive(buf);
            var response = Encoding.Unicode.GetString(buf, 0, recv);
            return response;
        }

        public void Close()
        {
            client?.Close();
            client = null;
        }
    }
}
