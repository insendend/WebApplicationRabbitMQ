using System;
using System.Net;
using ClassesLib.Sockets;

namespace AsyncTcpServer
{
    class TcpServerMain
    {
        static void Main(string[] args)
        {
            Console.Title = "TCP Server";

            var serv = new TcpServer(IPAddress.Any, 3333);
            serv.Start();

            Console.WriteLine("Press [enter] to exit.");
            Console.WriteLine();
            Console.ReadLine();
        }
    }
}
