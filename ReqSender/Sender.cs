using System;
using System.Net.Http;
using System.Text;
using ClassesLib;
using ClassesLib.Http;
using ClassesLib.Serialization;

namespace ReqSender
{
    class Sender
    {
        private static CustomHttpResult Post(Uri uri, TaskInfo value)
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));

            IHttpProvider httpProvider = new HttpProvider();
            ISerializer<TaskInfo> serializer = new TaskInfoSerializer();

            var objAsJson = serializer.SerializeToJson(value);

            var request = new HttpRequestMessage
            {
                RequestUri = uri,
                Method = HttpMethod.Post,
                Content = new StringContent(objAsJson, Encoding.UTF8, "application/json"),
            };

            return httpProvider.ProcessRequest(request);
        }

        static void Main(string[] args)
        {
            var uri = new Uri("http://localhost:59856/api/values/");

            var taskInfo = new TaskInfo
            {
                Title = "Main",
                Description = "Text, more text",
                Time = DateTime.Now
            };

            Console.WriteLine($"Started time: {taskInfo.Time}");

            try
            {
                var res = Post(uri, taskInfo);

                if (!res.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Bad request, Code: {res.HttpStatusCode}");
                    return;
                }

                ISerializer<TaskInfo> serializer = new TaskInfoSerializer();
                var updatedTaskInfo = serializer.DesirializeToObj(res.Content);
                Console.WriteLine($"Updated time: {updatedTaskInfo?.Time}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
