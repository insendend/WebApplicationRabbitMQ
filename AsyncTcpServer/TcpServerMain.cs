using System;
using Autofac;
using ClassesLib.Autofac;
using ClassesLib.Sockets.Server;

namespace AsyncTcpServer
{
    class TcpServerMain
    {
        static void Main(string[] args)
        {
            Console.Title = "TCP Server";

            var serv = IocContainer.Container.Resolve<TcpServerBase>();
            serv.Start();
        }
    }
}
