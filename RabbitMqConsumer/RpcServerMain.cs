using System;
using ClassesLib.Rabbit;

namespace RabbitMqConsumer
{
    class RpcServerMain
    {
        static void Main(string[] args)
        {
            Console.Title = "RPC Server";

            var settings = new RabbitSettings {HostName = "localhost", QueueName = "rpc_queue"};
            var rcpServer = new RpcServer(settings);
            rcpServer.Start();

            Console.WriteLine("Press [enter] to exit.");
            Console.WriteLine();
            Console.ReadLine();
        }
    }
}
