using System.Text;
using Newtonsoft.Json;

namespace ClassesLib.Serialization
{
    public class TaskInfoSerializer : ISerializer<TaskInfo>
    {
        public byte[] SerializeToBytes(TaskInfo obj)
        {
            var objAsJson = SerializeToJson(obj);
            return Encoding.UTF8.GetBytes(objAsJson);
        }

        public TaskInfo DesirializeToObj(byte[] bytes)
        {
            var objAsJson = Encoding.UTF8.GetString(bytes);
            return DesirializeToObj(objAsJson);
        }

        public string SerializeToJson(TaskInfo obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public TaskInfo DesirializeToObj(string objAsJson)
        {
            return JsonConvert.DeserializeObject<TaskInfo>(objAsJson);
        }
    }
}
