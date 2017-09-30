using System.Net;
using System.Net.Sockets;

namespace ClassesLib.Sockets.Settings
{
    public class TcpClientSettings : TcpSettingsBase
    {
        public static TcpClientSettings CreateDefault()
        {
            return new TcpClientSettings
            {
                AddressFamily = AddressFamily.InterNetwork,
                SocketType = SocketType.Stream,
                ProtocolType = ProtocolType.IP,
                EndPoint = new IPEndPoint(IPAddress.Loopback, 3333)
            };
        }
    }
}
