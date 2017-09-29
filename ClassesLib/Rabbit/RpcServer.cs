using System;
using System.Net;
using System.Text;
using ClassesLib.Serialization;
using ClassesLib.Sockets;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ClassesLib.Rabbit
{
    public class RpcServer : RpcServerBase
    {
        public RpcServer(RabbitSettings settings) : base(settings)
        {

        }

        protected override void MessageReceived(object sender, BasicDeliverEventArgs ea)
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

                var taskInfo = serializer.Desirialize(body);
                taskInfo.AddHours(1);
                var objAsBytes = serializer.Serialize(taskInfo);

                var client = new TcpClient();
                client.Connect(new IPEndPoint(IPAddress.Loopback, 3333));
                client.Send(objAsBytes);
                Console.WriteLine($"Sent: {objAsBytes.Length} bytes");


                var recvBytes = client.Receive();
                Console.WriteLine($"Received back: {recvBytes.Length} bytes");

                response = Encoding.UTF8.GetString(recvBytes);
            }
            catch (Exception e)
            {
                Console.WriteLine(" [.] " + e.Message);
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
