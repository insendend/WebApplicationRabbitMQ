using System;
using System.Net.Http;
using System.Text;
using ClassesLib;
using ClassesLib.Http;
using Newtonsoft.Json;

namespace ReqSender
{
    class Sender
    {
        private static CustomHttpResult Post(TaskInfo value)
        {
            const string url = "http://localhost:59856/api/values/";

            IHttpProvider httpProvider = new HttpProvider();

            if (value is null)
                throw new ArgumentNullException(nameof(value));

            var jsonString = JsonConvert.SerializeObject(value);

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Post,
                Content = new StringContent(jsonString, Encoding.UTF8, "application/json"),
            };

            return httpProvider.ProcessRequest(request);
        }


        static void Main(string[] args)
        {
            var taskInfo = new TaskInfo
            {
                Title = "Main",
                Description = "Text, more Text, text, tex",
                Time = DateTime.Now
            };

            try
            {
                var res = Post(taskInfo);
                var updatedTaskInfo = JsonConvert.DeserializeObject<TaskInfo>(res.Content);

                Console.WriteLine(updatedTaskInfo.Time);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
