using System;
using System.IO;
using System.Net.Sockets;

namespace ClassesLib.Sockets.Server
{
    public class CustomStateObject : IDisposable
    {
        public Socket Client { get; set; }
        public byte[] BuffBytes { get; set; }
        public MemoryStream ContentStream { get; set; }
        public int ReadBytes { get; set; }

        public CustomStateObject(int buffSize = 4096)
        {
            BuffBytes = new byte[buffSize];
            ContentStream = new MemoryStream();
        }

        public void Dispose()
        {
            Client?.Dispose();
            ContentStream?.Dispose();
        }
    }
}
