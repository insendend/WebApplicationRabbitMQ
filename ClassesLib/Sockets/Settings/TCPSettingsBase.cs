using System.Net;

namespace ClassesLib.Sockets.Settings
{
    public abstract class TcpSettingsBase
    {
        public IPAddress Ip { get; set; }

        public int Port { get; set; }
    }
}
