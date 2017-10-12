using System;
using ClassesLib.Rabbit.Settings;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;

namespace ClassesLib.Rabbit.Server
{
    public abstract class RpcServerBase : IDisposable
    {
        private readonly RabbitServSettings _settings;

        protected IModel Channel;
        private EventingBasicConsumer _consumer;

        protected ILogger Logger;

        protected RpcServerBase(RabbitServSettings settings, ILogger logger)
        {
            _settings = settings;
            Logger = logger;
        }

        private void SetupServer()
        {
            var factory = new ConnectionFactory
            {
                HostName = _settings.HostName,
                UserName = _settings.Login,
                Password = _settings.Password, 
            };

            var connection = factory.CreateConnection();
            Channel = connection.CreateModel();

            Channel.BasicQos(0, 1, false);

            _consumer = new EventingBasicConsumer(Channel);
            _consumer.Received += MessageReceived;

            Logger.Information($"Awaiting for RPC requests at {_settings.HostName}...");
        }

        public virtual void Start()
        {
            SetupServer();

            Channel.BasicConsume(
                queue: _settings.QueueName,
                autoAck: true,
                consumer: _consumer);

            Logger.Information("Press [enter] to exit.");
            Console.WriteLine();
            Console.ReadLine();
            Environment.Exit(0);
        }

        protected abstract void MessageReceived(object sender, BasicDeliverEventArgs ea);

        public void Dispose()
        {
            Channel?.Dispose();
        }
    }
}
