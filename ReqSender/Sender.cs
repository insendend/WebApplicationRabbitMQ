using System;
using System.Net.Http;
using System.Text;
using Autofac;
using ClassesLib;
using ClassesLib.Autofac;
using ClassesLib.Http;
using ClassesLib.Serialization;

namespace ReqSender
{
    class Sender
    {
        static void Main(string[] args)
        {
            var taskInfo = new TaskInfo
            {
                Title = "Main",
                Description = "Text, more text",
                Time = DateTime.Now
            };

            Console.WriteLine($"Started time: {taskInfo.Time}");

            try
            {
                var provider = IocContainer.Container.Resolve<IHttpProvider>();
                var serializer = IocContainer.Container.Resolve<ISerializer<TaskInfo>>();

                var objAsJson = serializer.SerializeToJson(taskInfo);
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri("http://localhost:59856/api/values/"),
                    Method = HttpMethod.Post,
                    Content = new StringContent(objAsJson, Encoding.UTF8, "application/json"),
                };

                var res = provider.ProcessRequest(request);

                if (!res.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Bad request, Code: {res.HttpStatusCode}");
                    return;
                }
           
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
