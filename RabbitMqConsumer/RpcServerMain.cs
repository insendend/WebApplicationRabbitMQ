using System;
using System.IO;
using ClassesLib.Rabbit.Server;
using ClassesLib.Rabbit.Settings;
using Serilog;
using Serilog.Formatting.Json;

namespace RabbitMqConsumer
{
    class RpcServerMain
    {
        static void Main(string[] args)
        {
            Console.Title = "RPC Server";

            var logFile = Path.Combine(Environment.CurrentDirectory, "logFile.json");
            var logger = new LoggerConfiguration()
                .WriteTo.LiterateConsole()
                .WriteTo.File(new JsonFormatter(), logFile)
                .CreateLogger();

            var settings = new RabbitServSettings
            {
                QueueName = "rpc_queue",
                Exchange = "exch-rpc"
            };

            var rcpServer = new RpcServer(settings, logger);
            rcpServer.Start();
        }
    }
}
