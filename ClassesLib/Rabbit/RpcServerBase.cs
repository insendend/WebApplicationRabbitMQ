using System;
using ClassesLib.Rabbit.Settings;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ClassesLib.Rabbit
{
    public abstract class RpcServerBase : IDisposable
    {
        protected RabbitServSettings settings;

        protected IModel channel;
        protected EventingBasicConsumer consumer;

        protected RpcServerBase(RabbitServSettings settings)
        {
            this.settings = settings;
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

            Console.WriteLine("Awaiting RPC requests...");
        }

        public virtual void Start()
        {
            channel.BasicConsume(
                queue: settings.QueueName,
                autoAck: false,
                consumer: consumer);

            Console.WriteLine("Press [enter] to exit.");
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
