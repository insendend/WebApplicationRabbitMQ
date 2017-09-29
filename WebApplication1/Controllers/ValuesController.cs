using System;
using System.Collections.Generic;
using ClassesLib;
using ClassesLib.Rabbit;
using ClassesLib.Rabbit.Settings;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public string Post(TaskInfo value)
        {
            var response = string.Empty;

            if (value is null)
                return response;
            try
            {
                value.AddHours(1);

                var data = JsonConvert.SerializeObject(value);

                var settings = new RabbitClientSettings {HostName = "localhost", QueueName = "rpc_queue", Exchange = "exch"};

                var rpcClient = new RpcClient(settings);
                response = rpcClient.Call(data);
            }
            catch (Exception)
            {
                // 
            }

            return response;
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
