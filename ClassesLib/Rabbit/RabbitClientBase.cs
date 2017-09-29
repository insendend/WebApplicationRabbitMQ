using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using ClassesLib.Rabbit.Settings;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ClassesLib.Rabbit
{
    public abstract class RabbitClientBase : IDisposable
    {
        protected RabbitSettings settings;

        protected readonly IConnection connection;
        protected readonly IModel channel;
        protected readonly string replyQueueName;
        protected readonly EventingBasicConsumer consumer;
        protected readonly BlockingCollection<string> respQueue = new BlockingCollection<string>();
        protected readonly IBasicProperties props;

        protected readonly string correlationId;

        protected RabbitClientBase(RabbitSettings settings)
        {
            var factory = new ConnectionFactory { HostName = settings.HostName };

            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            replyQueueName = channel.QueueDeclare().QueueName;
            consumer = new EventingBasicConsumer(channel);

            props = channel.CreateBasicProperties();
            correlationId = Guid.NewGuid().ToString();
            props.CorrelationId = correlationId;
            props.ReplyTo = replyQueueName;

            consumer.Received += MessageReceived;
        }

        protected abstract void MessageReceived(object sender, BasicDeliverEventArgs ea);

        public virtual string Call(string message)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(
                exchange: "",
                routingKey: settings.QueueName,
                basicProperties: props,
                body: messageBytes);

            channel.BasicConsume(
                consumer: consumer,
                queue: replyQueueName,
                autoAck: true);

            return respQueue.Take(); ;
        }

        public void Dispose()
        {
            connection?.Dispose();
            channel?.Dispose();
            respQueue?.Dispose();
        }
    }
}
