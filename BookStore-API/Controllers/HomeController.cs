using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStore_API.Contracts;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BookStore_API.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly ILoggerService _logger;

        public HomeController(ILoggerService logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        // GET: api/<HomeController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            _logger.LogInfo("Accessed Home Controller");
            return new string[] { "value1", "value2" };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET api/<HomeController>/5
        [HttpGet("{id}")]
#pragma warning disable IDE0060 // Remove unused parameter
        public string Get(int id)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            _logger.LogDebug("Got a Value");
            return "value";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        // POST api/<HomeController>
        [HttpPost]
#pragma warning disable IDE0060 // Remove unused parameter
        public void Post([FromBody] string value)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            _logger.LogError("This is an error");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        // PUT api/<HomeController>/5
        [HttpPut("{id}")]
#pragma warning disable IDE0060 // Remove unused parameter
        public void Put(int id, [FromBody] string value)
#pragma warning restore IDE0060 // Remove unused parameter
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        // DELETE api/<HomeController>/5
        [HttpDelete("{id}")]
#pragma warning disable IDE0060 // Remove unused parameter
        public void Delete(int id)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            _logger.LogWarn("This is a warning");
        }
    }
}
