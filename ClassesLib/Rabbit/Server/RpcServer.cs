using System;
using System.Net;
using System.Text;
using Autofac;
using ClassesLib.Autofac;
using ClassesLib.Rabbit.Settings;
using ClassesLib.Serialization;
using ClassesLib.Sockets.Client;
using ClassesLib.Sockets.Settings;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;

namespace ClassesLib.Rabbit.Server
{
    public class RpcServer : RpcServerBase
    {
        public RpcServer(RabbitServSettings settings, ILogger logger) : base(settings, logger)
        {
        }

        protected override async void MessageReceived(object sender, BasicDeliverEventArgs ea)
        {
            string response = null;

            var body = ea.Body;
            Logger.Information($"Received: {body.Length} bytes");

            var props = ea.BasicProperties;
            var replyProps = Channel.CreateBasicProperties();
            replyProps.CorrelationId = props.CorrelationId;

            try
            {
                var serializer = IocContainer.Container.Resolve<ISerializer<TaskInfo>>();

                var taskInfo = serializer.DesirializeToObj(body);
                Logger.Information("Received object: {@taskInfo}", taskInfo);

                taskInfo.AddHours(1);
                Logger.Information("Added one hour: {@taskInfo}", taskInfo.Time);

                var objAsBytes = serializer.SerializeToBytes(taskInfo);

                var client = IocContainer.Container.Resolve<TcpClientBase>();
                await client.SendAsync(objAsBytes);
                Logger.Information($"Sent: {objAsBytes.Length} bytes");

                var recvBytes = await client.ReceiveAsync();
                Logger.Information($"Received back: {recvBytes.Length} bytes");

                response = Encoding.UTF8.GetString(recvBytes);
            }
            catch (Exception e)
            {
                Logger.Error(e, "Received msg failed");
                response = "";
            }
            finally
            {
                var responseBytes = Encoding.UTF8.GetBytes(response);
                Logger.Information($"Sent back: {responseBytes.Length} bytes");

                Channel.BasicPublish(
                    exchange: "",
                    routingKey: props.ReplyTo,
                    basicProperties: replyProps,
                    body: responseBytes);

                Channel.BasicAck(
                    deliveryTag: ea.DeliveryTag,
                    multiple: false);
            }
        }
    }
}
