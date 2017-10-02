using System;
using ClassesLib.Rabbit.Settings;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;

namespace ClassesLib.Rabbit.Server
{
    public abstract class RpcServerBase : IDisposable
    {
        private readonly RabbitServSettings settings;

        protected IModel channel;
        private EventingBasicConsumer consumer;

        protected ILogger logger;

        protected RpcServerBase(RabbitServSettings settings, ILogger logger)
        {
            this.settings = settings;
            this.logger = logger;
            SetupServer();
        }

        private void SetupServer()
        {
            var factory = new ConnectionFactory
            {
                HostName = settings.HostName,
                UserName = settings.Login,
                Password = settings.Password, 
            };

            var connection = factory.CreateConnection();
            channel = connection.CreateModel();

            channel.BasicQos(0, 1, false);

            consumer = new EventingBasicConsumer(channel);
            consumer.Received += MessageReceived;

            logger.Information($"Awaiting for RPC requests at {settings.HostName}...");
        }

        public virtual void Start()
        {
            channel.BasicConsume(
                queue: settings.QueueName,
                autoAck: false,
                consumer: consumer);

            logger.Information("Press [enter] to exit.");
            Console.WriteLine();
            Console.ReadLine();
        }

        protected abstract void MessageReceived(object sender, BasicDeliverEventArgs ea);

        public void Dispose()
        {
            channel?.Dispose();
        }
    }
}
