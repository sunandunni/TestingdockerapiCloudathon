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
using System.Net;


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
        ConfigurationOptions options = null;
        ConnectionMultiplexer connection = null;
        IDatabase db = null;
        EndPoint endPoint = null;
        BlobContainerClient blobContainerClient = null;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IDistributedCache cache, ICosmosDbService cosmosDbService)
        {
            _logger = logger;
            _cache = cache;
            _cosmosDbService = cosmosDbService ?? throw new ArgumentNullException(nameof(cosmosDbService));
            options = ConfigurationOptions.Parse("otelowlsrediscache.redis.cache.windows.net:6380,password=fAm8LA1EnfBpRNQ3Yvrsfkofp9860K6HPAzCaE2gR9A=,ssl=True,abortConnect=False");
            connection = ConnectionMultiplexer.Connect(options);
            db = connection.GetDatabase();
            endPoint = connection.GetEndPoints().First();
            blobContainerClient = AzureBlobStorage.GetBlobContainer();
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
        [Route("GetClient")]
        public List<Client> GetClient(string name)
        {
            // Done and Tested OK.
            return manager.GetClient(name,_cache,connection);
        }

        [HttpPost]
        [Route("UpdateClient/{clientId}")]
        public bool UpdateClient(string clientId, double goalAmount, int retirementAge)
        {
            // Done and Tested OK
            return manager.UpdateClient(clientId,_cache,goalAmount,retirementAge);
        }

        [HttpPost]
        [Route("UpdateCashflows")]
        public bool UpdateCashflow(List<Cashflow> cashflow)
        {
            // DOne and Tested OK.
            return manager.UpdateCashflow(cashflow, _cache);
            
        }

        [HttpGet]
        [Route("GetCashflows/{clientId}")]
        public List<Cashflow> getCashflows(string clientId)
        {
            // Done and Tested OK.
            return manager.GetCashflows(clientId,_cache,connection);

        }

        [HttpGet]
        [Route("GetAccounts/{clientId}")]
        public List<Account> getAccounts(string clientId)
        {
            return manager.GetAccounts(clientId,blobContainerClient);
        }

        [HttpGet]
        [Route("GetPlan/{clientId}")]
        public Plan getPlan(string clientId)
        {
            return manager.GetPlan(clientId,_cache,connection,blobContainerClient);
        }

    }
}
