using System;

namespace ClassesLib
{
    public class TaskInfo
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime Time { get; set; }

        public void AddHours(int count)
        {
            var differentSeconds = DateTime.Now.Second - Time.Second;
            Time += TimeSpan.FromHours(1) + TimeSpan.FromSeconds(differentSeconds);
        }
    }
}
