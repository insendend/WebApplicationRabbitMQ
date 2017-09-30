using System.Net;
using System.Net.Sockets;

namespace ClassesLib.Sockets.Settings
{
    public abstract class TcpSettingsBase
    {
        public AddressFamily AddressFamily { get; set; }
        public ProtocolType ProtocolType { get; set; }
        public SocketType SocketType { get; set; }

        public IPEndPoint EndPoint { get; set; }
    }
}
