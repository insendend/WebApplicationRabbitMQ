using System;
using System.Net;
using ClassesLib.Sockets.Server;
using ClassesLib.Sockets.Settings;

namespace AsyncTcpServer
{
    class TcpServerMain
    {
        static void Main(string[] args)
        {
            Console.Title = "TCP Server";

            var servSettings = new TcpServerSettings {Ip = IPAddress.Any, Port = 3333, ClientCount = 1024};
            var serv = new TcpServer(servSettings);
            serv.Start();

            Console.WriteLine("Press [enter] to exit.");
            Console.WriteLine();
            Console.ReadLine();
        }
    }
}
