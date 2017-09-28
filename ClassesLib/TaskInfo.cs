using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ClassesLib
{
    [DataContract]
    public class TaskInfo
    {
        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public DateTime Time { get; set; }

        public void AddHours(int count) => Time = Time.AddHours(count);
    }
}
