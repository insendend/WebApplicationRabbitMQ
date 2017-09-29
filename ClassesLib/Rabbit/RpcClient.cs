using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Text;
using ClassesLib.Rabbit.Settings;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ClassesLib.Rabbit
{
    public class RpcClient : RabbitClientBase
    {
        public RpcClient(RabbitSettings settings) : base(settings)
        {
            this.settings = settings;
        }

        protected override void MessageReceived(object sender, BasicDeliverEventArgs ea)
        {
            var body = ea.Body;
            var response = Encoding.UTF8.GetString(body);
            if (ea.BasicProperties.CorrelationId == correlationId)
            {
                respQueue.Add(response);
            }
        }

        public string Call(string message)
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

        public void Close()
        {
            connection.Close();
        }
    }
}
