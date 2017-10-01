﻿using System;
using System.Net;
using System.Text;
using ClassesLib.Rabbit.Settings;
using ClassesLib.Serialization;
using ClassesLib.Sockets.Client;
using ClassesLib.Sockets.Settings;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ClassesLib.Rabbit.Server
{
    public class RpcServer : RpcServerBase
    {
        public RpcServer(RabbitServSettings settings) : base(settings)
        {
        }

        protected override async void MessageReceived(object sender, BasicDeliverEventArgs ea)
        {
            string response = null;

            var body = ea.Body;
            Console.WriteLine($"Received: {body.Length} bytes");

            var props = ea.BasicProperties;
            var replyProps = channel.CreateBasicProperties();
            replyProps.CorrelationId = props.CorrelationId;

            try
            {
                ISerializer<TaskInfo> serializer = new TaskInfoSerializer();

                var taskInfo = serializer.DesirializeToObj(body);
                taskInfo.AddHours(1);
                var objAsBytes = serializer.SerializeToBytes(taskInfo);

                var tcpSettings = new TcpClientSettings {Ip = IPAddress.Loopback, Port = 3333};
                var client = new TcpClient(tcpSettings);
                await client.SendAsync(objAsBytes);
                Console.WriteLine($"Sent: {objAsBytes.Length} bytes");

                var recvBytes = await client.ReceiveAsync();
                Console.WriteLine($"Received back: {recvBytes.Length} bytes");

                response = Encoding.UTF8.GetString(recvBytes);
            }
            catch (Exception e)
            {
                Console.WriteLine("[.] " + e.Message);
                response = "";
            }
            finally
            {
                var responseBytes = Encoding.UTF8.GetBytes(response);
                Console.WriteLine($"Sent back: {responseBytes.Length} bytes");

                channel.BasicPublish(
                    exchange: "",
                    routingKey: props.ReplyTo,
                    basicProperties: replyProps,
                    body: responseBytes);

                channel.BasicAck(
                    deliveryTag: ea.DeliveryTag,
                    multiple: false);
            }
        }
    }
}