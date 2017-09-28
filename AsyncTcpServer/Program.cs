using System;
using System.Net;
using ClassesLib.Sockets;

namespace AsyncTcpServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var serv = new TcpServer(IPAddress.Any, 3333);

            serv.Start();

            Console.ReadLine();
        }
    }
}
