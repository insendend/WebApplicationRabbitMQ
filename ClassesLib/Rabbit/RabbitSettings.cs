using System;
using System.Collections.Generic;
using System.Text;

namespace ClassesLib.Rabbit
{
    public class RabbitSettings
    {
        public string HostName { get; set; }

        public string Login { get; set; }
        public string Password { get; set; }

        public string QueueName { get; set; }
    }
}
