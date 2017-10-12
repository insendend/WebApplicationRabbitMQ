using System;
using Autofac;
using ClassesLib.Autofac;
using ClassesLib.Rabbit.Server;

namespace RabbitMqConsumer
{
    class RpcServerMain
    {
        static void Main(string[] args)
        {
            Console.Title = "RPC Server";

            var rpcServer = IocContainer.Container.Resolve<RpcServerBase>();
            rpcServer.Start();
        }
    }
}
