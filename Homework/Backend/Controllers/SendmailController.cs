using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SendmailController : ControllerBase
    {
        public SendmailController()
        {
        }

        [HttpGet("{value1}/{value2}")]
        public async Task<string> Get(int value1, int value2)
        {
            string result = "" ;
            result = (value1 + value2).ToString();
            Console.WriteLine(result);
            await Task.Delay(1000);
            return result;
        }
    }
}
