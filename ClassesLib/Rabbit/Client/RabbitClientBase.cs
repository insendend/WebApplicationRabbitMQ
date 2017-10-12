using System;
using System.Collections.Concurrent;
using System.Text;
using ClassesLib.Rabbit.Settings;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ClassesLib.Rabbit.Client
{
    public abstract class RabbitClientBase : IDisposable
    {
        protected readonly RabbitClientSettings Settings;

        protected IConnection Connection;
        protected IModel Channel;
        protected string ReplyQueueName;
        protected EventingBasicConsumer Consumer;
        protected BlockingCollection<string> RespQueue = new BlockingCollection<string>();
        protected IBasicProperties Props;

        protected string CorrelationId;

        protected RabbitClientBase(RabbitClientSettings settings)
        {
            Settings = settings;
        }

        private void SetupClient()
        {
            var factory = new ConnectionFactory
            {
                HostName = Settings.HostName,
                UserName = Settings.Login,
                Password = Settings.Password
            };

            Connection = factory.CreateConnection();
            Channel = Connection.CreateModel();

            ReplyQueueName = Channel.QueueDeclare().QueueName;

            Props = Channel.CreateBasicProperties();
            CorrelationId = Guid.NewGuid().ToString();
            Props.CorrelationId = CorrelationId;
            Props.ReplyTo = ReplyQueueName;


            Consumer = new EventingBasicConsumer(Channel);
            Consumer.Received += MessageReceived;
        }

        protected abstract void MessageReceived(object sender, BasicDeliverEventArgs ea);

        public virtual string Call(string message)
        {
            SetupClient();

            var messageBytes = Encoding.UTF8.GetBytes(message);
            Channel.BasicPublish(
                exchange: Settings.Exchange,
                routingKey: Settings.QueueName,
                basicProperties: Props,
                body: messageBytes);

            Channel.BasicConsume(
                consumer: Consumer,
                queue: ReplyQueueName,
                autoAck: true);

            return RespQueue.Take();
        }

        public void Dispose()
        {
            Connection?.Dispose();
            Channel?.Dispose();
            RespQueue?.Dispose();
        }
    }
}
