using System.Text;
using ClassesLib.Rabbit.Settings;
using RabbitMQ.Client.Events;

namespace ClassesLib.Rabbit
{
    public class RpcClient : RabbitClientBase
    {
        public RpcClient(RabbitClientSettings settings) : base(settings)
        {
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
    }
}
