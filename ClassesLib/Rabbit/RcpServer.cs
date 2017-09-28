using System;
using System.Net;
using System.Text;
using ClassesLib.Sockets;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;

namespace ClassesLib.Rabbit
{
    public class RcpServer
    {
        public void Start()
        {
            var factory = new ConnectionFactory() {HostName = "localhost"};
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(
                    queue: "rpc_queue",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                channel.BasicQos(0, 1, false);

                var consumer = new EventingBasicConsumer(channel);

                channel.BasicConsume(
                    queue: "rpc_queue",
                    autoAck: false, 
                    consumer: consumer);

                Console.WriteLine(" [x] Awaiting RPC requests");

                consumer.Received += (model, ea) =>
                {
                    string response = null;

                    var body = ea.Body;
                    var props = ea.BasicProperties;
                    var replyProps = channel.CreateBasicProperties();
                    replyProps.CorrelationId = props.CorrelationId;

                    try
                    {
                        var msg = Encoding.UTF8.GetString(body);

                        var taskInfo = JsonConvert.DeserializeObject<TaskInfo>(msg);
                        taskInfo.AddHours(1);

                        var client = new TcpClient(IPAddress.Loopback, 3333);
                        client.Send(JsonConvert.SerializeObject(taskInfo));

                        response = client.Receive();

                        //response = JsonConvert.SerializeObject(taskInfo);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(" [.] " + e.Message);
                        response = "";
                    }
                    finally
                    {
                        var responseBytes = Encoding.UTF8.GetBytes(response);

                        channel.BasicPublish(
                            exchange: "", 
                            routingKey: props.ReplyTo,
                            basicProperties: replyProps, 
                            body: responseBytes);

                        channel.BasicAck(
                            deliveryTag: ea.DeliveryTag,
                            multiple: false);
                    }
                };

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }



    }
}
