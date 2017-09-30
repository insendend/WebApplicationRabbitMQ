using System;
using System.Collections.Concurrent;
using System.Text;
using ClassesLib.Rabbit.Settings;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ClassesLib.Rabbit
{
    public abstract class RabbitClientBase : IDisposable
    {
        protected RabbitClientSettings settings;

        protected IConnection connection;
        protected IModel channel;
        protected string replyQueueName;
        protected EventingBasicConsumer consumer;
        protected BlockingCollection<string> respQueue = new BlockingCollection<string>();
        protected IBasicProperties props;

        protected string correlationId;

        protected RabbitClientBase(RabbitClientSettings settings)
        {
            this.settings = settings;
            SetupClient();
        }

        private void SetupClient()
        {
            var factory = new ConnectionFactory
            {
                HostName = settings.HostName,
                UserName = settings.Login,
                Password = settings.Password
            };

            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            replyQueueName = channel.QueueDeclare().QueueName;

            props = channel.CreateBasicProperties();
            correlationId = Guid.NewGuid().ToString();
            props.CorrelationId = correlationId;
            props.ReplyTo = replyQueueName;


            consumer = new EventingBasicConsumer(channel);
            consumer.Received += MessageReceived;
        }

        protected abstract void MessageReceived(object sender, BasicDeliverEventArgs ea);

        public virtual string Call(string message)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(
                exchange: settings.Exchange,
                routingKey: settings.QueueName,
                basicProperties: props,
                body: messageBytes);

            channel.BasicConsume(
                consumer: consumer,
                queue: replyQueueName,
                autoAck: true);

            return respQueue.Take();
        }

        public void Dispose()
        {
            connection?.Dispose();
            channel?.Dispose();
            respQueue?.Dispose();
        }
    }
}
