using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Net.Sockets;
using System.Threading;
using StackExchange.Redis;
using Newtonsoft.Json;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using Entities;
using Azure.Storage.Blobs;
using System.IO;
using System.Threading.Tasks;
using TestingDockerApi;
using Testingdockerapi.Entities;
using Testingdockerapi.Business;

namespace Testingdockerapi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

    //    private readonly List<Employee> fakeDatabase = new List<Employee>
    //{
    //    new Employee("1","Sunand",25),
    //    new Employee("2","Saumya",23)
    //};
        private readonly IDistributedCache _cache;
        private readonly ICosmosDbService _cosmosDbService;

        private readonly ILogger<WeatherForecastController> _logger;

        private PlanManager manager = new PlanManager();

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IDistributedCache cache, ICosmosDbService cosmosDbService)
        {
            _logger = logger;
            _cache = cache;
            _cosmosDbService = cosmosDbService ?? throw new ArgumentNullException(nameof(cosmosDbService));
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet]
        [Route("Name")]
        public string GetName()
        {
            return "Sunand";
        }

        [HttpGet]
        [Route("GetCache")]
        public async Task<Employee> GetCache(string key)
        {
            //List<string> myTodos = new List<string>();
            //bool IsCached = false;
            string employeeDetails = string.Empty;
            employeeDetails = await _cache.GetStringAsync(key);
            if (!string.IsNullOrEmpty(employeeDetails))
            {
                var employeeDetailsValue = JsonConvert.DeserializeObject<Employee>(employeeDetails);
               
                return employeeDetailsValue;
                // loaded data from the redis cache.
                //myTodos = JsonSerializer.Deserialize<List<string>>(cachedTodosString);
                //IsCached = true;
            }
            else
            {
                return null;
            }
        }

        [HttpPost]
        [Route("SetCache")]
        public async Task<Employee> SetCache([FromBody] Employee employee)
        {
            var key = employee.Id;
            //List<string> myTodos = new List<string>();
            //bool IsCached = false;
            string employeeDetails = string.Empty;
            employeeDetails = await _cache.GetStringAsync(key);
            if (string.IsNullOrEmpty(employeeDetails))
            {
                var employeeDetailsValue = JsonConvert.SerializeObject(employee);
                await _cache.SetStringAsync(key, employeeDetailsValue);
                return employee;
                // loaded data from the redis cache.
                //myTodos = JsonSerializer.Deserialize<List<string>>(cachedTodosString);
                //IsCached = true;
            }
            else
            {
                return null;
            }
        }

        // GET api/items
        [HttpGet]
        [Route("GetCosmos")]
        public async Task<IActionResult> List()
        {
            return Ok(await _cosmosDbService.GetMultipleAsync("SELECT * FROM c"));
        }
        // GET api/items/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            return Ok(await _cosmosDbService.GetAsync(id));
        
        }
        // POST api/items
        [HttpPost]
        [Route("UpdateItem")]
        public async Task<IActionResult> Create([FromBody] Item item)
        {
            item.Id = Guid.NewGuid().ToString();
            await _cosmosDbService.AddAsync(item);
            return CreatedAtAction(nameof(Get), new { id = item.Id }, item);
        }

        [HttpPost]
        [Route("UploadBlob")]
        public async Task<string> UploadBlob()
        {
            await AzureBlobStorage.GetBlob();
            return "ok";
           
        }
        // PUT api/items/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit([FromBody] Item item)
        {
            await _cosmosDbService.UpdateAsync(item.Id, item);
            return NoContent();
        }
        // DELETE api/items/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _cosmosDbService.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet]
        [Route("GetClient")]
        public List<Client> GetClient(string name)
        {
            return manager.GetClient(name,_cache);
        }

        [HttpGet]
        [Route("GetGoal")]
        public Goal GetGoal(int clientId)
        {
            return manager.GetGoal(clientId,_cache);
        }

        [HttpGet]
        [Route("GetCashflows")]
        public List<Cashflow> getCashflows(int clientId)
        {
            return manager.GetCashflows(clientId,_cache);

        }

        [HttpGet]
        [Route("GetAccounts")]
        public List<Account> getAccounts(int clientId)
        {
            return manager.GetAccounts(clientId);
        }

    }
}
