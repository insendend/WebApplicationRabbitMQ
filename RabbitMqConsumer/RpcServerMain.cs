using System;
using ClassesLib.Rabbit;
using ClassesLib.Rabbit.Server;
using ClassesLib.Rabbit.Settings;

namespace RabbitMqConsumer
{
    class RpcServerMain
    {
        static void Main(string[] args)
        {
            Console.Title = "RPC Server";

            var settings = new RabbitServSettings
            {
                QueueName = "rpc_queue",
                Exchange = "exch-rpc"
            };

            var rcpServer = new RpcServer(settings);
            rcpServer.Start();
        }
    }
}
