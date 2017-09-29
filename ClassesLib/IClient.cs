using System.Net;

namespace ClassesLib
{
    public interface IClient
    {
        void Connect(IPEndPoint endPoint);

        int Send(byte[] msgBytes);

        byte[] Receive();
    }
}
