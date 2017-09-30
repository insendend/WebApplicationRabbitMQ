using System;
using ClassesLib.Sockets;
using ClassesLib.Sockets.Settings;

namespace AsyncTcpServer
{
    class TcpServerMain
    {
        static void Main(string[] args)
        {
            Console.Title = "TCP Server";

            var servSettings = TcpServerSettings.CreateDefault();
            var serv = new TcpServer(servSettings);
            serv.Start();

            Console.WriteLine("Press [enter] to exit.");
            Console.WriteLine();
            Console.ReadLine();
        }
    }
}
