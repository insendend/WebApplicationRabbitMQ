using System;
using ClassesLib.Rabbit;

namespace RabbitMqConsumer
{
    class RpcServerMain
    {
        static void Main(string[] args)
        {
            Console.Title = "RPC Server";
            var rcpServer = new RcpServer();
            rcpServer.Start();

            Console.WriteLine("Press [enter] to exit.");
            Console.WriteLine();
            Console.ReadLine();
        }
    }
}
