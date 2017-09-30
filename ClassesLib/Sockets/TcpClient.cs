using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using ClassesLib.Sockets.Settings;

namespace ClassesLib.Sockets
{
    public class TcpClient : TcpClientBase
    {
        public TcpClient(TcpClientSettings settings) : base(settings)
        {
        }

    }
}
