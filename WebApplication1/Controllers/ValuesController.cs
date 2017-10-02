using System;
using System.Collections.Generic;
using System.Diagnostics;
using ClassesLib;
using ClassesLib.Rabbit.Client;
using ClassesLib.Rabbit.Settings;
using ClassesLib.Serialization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Microsoft.Extensions.Logging;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly ILogger<ValuesController> logger;

        public ValuesController(ILogger<ValuesController> logger)
        {
            this.logger = logger;
        }

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
        public IActionResult Post([FromBody]TaskInfo value)
        {
            if (value is null)
                return BadRequest();

            logger.LogInformation("{methodName} request for {@taskInfo}", nameof(Post), value);
            string response;
            ISerializer<TaskInfo> serializer = new TaskInfoSerializer();
            
            try
            {
                value.AddHours(1);
  
                var objAsJson = serializer.SerializeToJson(value);

                var settings = new RabbitClientSettings {HostName = "localhost", QueueName = "rpc_queue", Exchange = "exch-rpc" };
                var rpcClient = new RpcClient(settings);
                response = rpcClient.Call(objAsJson);
            }
            catch (Exception e)
            {
                logger.LogError(e, "");
                Debug.WriteLine(e.Message);
                return new EmptyResult();
            }
            
            return new ObjectResult(serializer.DesirializeToObj(response));
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
