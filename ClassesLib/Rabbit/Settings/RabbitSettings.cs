namespace ClassesLib.Rabbit.Settings
{
    public abstract class RabbitSettings
    {
        public string HostName { get; set; } = "localhost";

        public string Login { get; set; } = "guest";
        public string Password { get; set; } = "guest";

        public string QueueName { get; set; } = "";

        public string Exchange { get; set; } = "";
        public string RoutingKey { get; set; } = "";
    }
}
