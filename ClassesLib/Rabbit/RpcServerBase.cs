using System;
using ClassesLib.Rabbit.Settings;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ClassesLib.Rabbit
{
    public abstract class RpcServerBase : IDisposable
    {
        protected RabbitSettings settings;

        protected IModel channel;
        protected EventingBasicConsumer consumer;

        protected RpcServerBase(RabbitSettings settings)
        {
            this.settings = settings;

            var factory = new ConnectionFactory { HostName = this.settings.HostName };
            var connection = factory.CreateConnection();
            channel = connection.CreateModel();

            channel.QueueBind(queue: settings.QueueName,
                exchange: settings.Exchange,
                routingKey: "");
        }

        public virtual void Start()
        {
            //channel.QueueDeclare(
            //    queue: settings.QueueName,
            //    durable: false,
            //    exclusive: false,
            //    autoDelete: false,
            //    arguments: null);

            channel.BasicQos(0, 1, false);

            consumer = new EventingBasicConsumer(channel);

            channel.BasicConsume(
                queue: settings.QueueName,
                autoAck: false,
                consumer: consumer);

            Console.WriteLine($"Awaiting RPC requests at channel #{channel.ChannelNumber}...");

            consumer.Received += MessageReceived;
        }

        protected abstract void MessageReceived(object sender, BasicDeliverEventArgs ea);

        public void Dispose()
        {
            channel?.Dispose();
        }
    }
}
