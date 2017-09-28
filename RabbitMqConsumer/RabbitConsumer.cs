using ClassesLib.Rabbit;

namespace RabbitMqConsumer
{
    class RabbitConsumer
    {
        static void Main(string[] args)
        {
            var rcpServer = new RcpServer();
            rcpServer.Start();
        }
    }
}
