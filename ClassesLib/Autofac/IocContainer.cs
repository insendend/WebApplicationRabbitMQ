using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using ClassesLib.Http;
using ClassesLib.Rabbit.Client;
using ClassesLib.Rabbit.Server;
using ClassesLib.Rabbit.Settings;
using ClassesLib.Serialization;
using ClassesLib.Sockets.Client;
using ClassesLib.Sockets.Server;
using ClassesLib.Sockets.Settings;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace ClassesLib.Autofac
{
    public class IocContainer
    {
        private static readonly ContainerBuilder Builder = new ContainerBuilder();

        private static IContainer _container;
        public static IContainer Container => _container ?? (_container = GetContainer());

        private IocContainer() { }

        public static void AddServices(IServiceCollection services)
        {
            Builder.Populate(services);
        }

        private static IContainer GetContainer()
        {
            var logFile = Path.Combine(Environment.CurrentDirectory, "logFile.json");
            Log.Logger = new LoggerConfiguration()
                .WriteTo.LiterateConsole()
                //.WriteTo.File(new JsonFormatter(), logFile)
                .CreateLogger();

            // Registrations
            Builder.RegisterType<TaskInfoSerializer>().As<ISerializer<TaskInfo>>();
            Builder.RegisterType<HttpProvider>().As<IHttpProvider>();
            Builder.RegisterType<RpcClient>().As<RabbitClientBase>()
                .WithParameter("settings", new RabbitClientSettings
                {
                    QueueName = "rpc_queue",
                    Exchange = "exch-rpc"
                });
            Builder.RegisterType<RpcServer>().As<RpcServerBase>()
                .WithParameters(new List<Parameter>
                {
                    new NamedParameter("settings", new RabbitServSettings
                    {
                        QueueName = "rpc_queue",
                        Exchange = "exch-rpc"
                    }),
                    new NamedParameter("logger", Log.Logger)
                });
            Builder.RegisterType<TcpServer>().As<TcpServerBase>()
                .WithParameters(new List<Parameter>
                {
                    new NamedParameter("settings", new TcpServerSettings
                    {
                        Ip = IPAddress.Any,
                        Port = 3333,
                        ClientCount = 1024
                    }),
                    new NamedParameter("logger", Log.Logger)
                });
            Builder.RegisterType<TcpClient>().As<TcpClientBase>()
                .WithParameters(new List<Parameter>
                {
                    new NamedParameter("settings", new TcpClientSettings
                    {
                        Ip = IPAddress.Loopback,
                        Port = 3333
                    }),
                    new NamedParameter("logger", Log.Logger)
                });

            return Builder.Build();
        }
    }
}
