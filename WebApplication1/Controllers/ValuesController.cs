using System;
using System.Collections.Generic;
using System.Diagnostics;
using ClassesLib;
using ClassesLib.Rabbit.Client;
using ClassesLib.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly ILogger<ValuesController> _logger;
        private readonly ISerializer<TaskInfo> _serializer;
        private readonly RabbitClientBase _rabbitClientBase;

        public ValuesController(ILogger<ValuesController> logger, ISerializer<TaskInfo> serializer, RabbitClientBase rabbitClientBase)
        {
            _logger = logger;
            _serializer = serializer;
            _rabbitClientBase = rabbitClientBase;
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new [] { "value1", "value2" };
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

            _logger.LogInformation("{methodName} request for {@taskInfo}", nameof(Post), value);
            string response;
       
            try
            {
                value.AddHours(1);
  
                var objAsJson = _serializer.SerializeToJson(value);
                response = _rabbitClientBase.Call(objAsJson);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "");
                Debug.WriteLine(e.Message);
                return new EmptyResult();
            }
            
            return new ObjectResult(_serializer.DesirializeToObj(response));
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
