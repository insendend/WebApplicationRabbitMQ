namespace ClassesLib.Rabbit.Settings
{
    public abstract class RabbitSettings 
    {
        public string HostName { get; set; }

        public string Login { get; set; }
        public string Password { get; set; }
         
        public string QueueName { get; set; }
    }
}
