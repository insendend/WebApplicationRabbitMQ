using System.Text;
using Newtonsoft.Json;

namespace ClassesLib.Serialization
{
    public class TaskInfoSerializer : ISerializer<TaskInfo>
    {
        public byte[] Serialize(TaskInfo obj)
        {
            var objAsJson = JsonConvert.SerializeObject(obj);
            return Encoding.UTF8.GetBytes(objAsJson);
        }

        public TaskInfo Desirialize(byte[] bytes)
        {
            var objAsJson = Encoding.UTF8.GetString(bytes);
            return JsonConvert.DeserializeObject<TaskInfo>(objAsJson);
        }
    }
}
