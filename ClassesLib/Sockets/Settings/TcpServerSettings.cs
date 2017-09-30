using System.Net;
using System.Net.Sockets;

namespace ClassesLib.Sockets.Settings
{
    public class TcpServerSettings : TcpSettingsBase
    {
        public int ClientCount { get; set; }

        public static TcpServerSettings CreateDefault()
        {
            return new TcpServerSettings
            {
                AddressFamily = AddressFamily.InterNetwork,
                SocketType = SocketType.Stream,
                ProtocolType = ProtocolType.IP,
                EndPoint = new IPEndPoint(IPAddress.Any, 3333),
                ClientCount = 65535
            };
        }
    }
}
