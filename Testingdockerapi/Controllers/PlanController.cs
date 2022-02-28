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
using OpenTracing;
using System.Net.Http;
using System;


namespace Testingdockerapi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlanController : ControllerBase
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
       
        private readonly ILogger<PlanController> _logger;
        private ITracer _tracer;

        private PlanManager manager = new PlanManager();
        ConfigurationOptions options = null;
        ConnectionMultiplexer connection = null;
        IDatabase db = null;
        EndPoint endPoint = null;
        BlobContainerClient blobContainerClient = null;

        public PlanController(ILogger<PlanController> logger, IDistributedCache cache, ITracer tracer)
        {
            _logger = logger;
            _cache = cache;
            options = ConfigurationOptions.Parse("otelowlsrediscache.redis.cache.windows.net:6380,password=fAm8LA1EnfBpRNQ3Yvrsfkofp9860K6HPAzCaE2gR9A=,ssl=True,abortConnect=False");
            connection = ConnectionMultiplexer.Connect(options);
            db = connection.GetDatabase();
            endPoint = connection.GetEndPoints().First();
            blobContainerClient = AzureBlobStorage.GetBlobContainer();
            _tracer = tracer;
        }

        //[HttpGet]
        //public IEnumerable<WeatherForecast> Get()
        //{
        //    var rng = new Random();
        //    return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        //    {
        //        Date = DateTime.Now.AddDays(index),
        //        TemperatureC = rng.Next(-20, 55),
        //        Summary = Summaries[rng.Next(Summaries.Length)]
        //    })
        //    .ToArray();
        //}

        [HttpGet]
        [Route("GetClient")]
        public List<Client> GetClient(string name, bool getAllClients = false)
        {
            _logger.LogInformation("Fetching client names matching {0}", name);

            var operationName = "GetClient";
            var builder = _tracer.BuildSpan(operationName);

            using (var scope = builder.StartActive(true))
            {
                var span = scope.Span;

                var log = $"Fetching client names matching" +  name;
                span.Log(log);
            }

            return manager.GetClient(name,_cache,connection, getAllClients);
        }

        [HttpPost]
        [Route("UpdateClient/{clientId}")]
        public bool UpdateClient(string clientId, double goalAmount, int retirementAge)
        {
            _logger.LogInformation("Update client details for {0}", clientId);
            // Done and Tested OK
            return manager.UpdateClient(clientId,_cache,goalAmount,retirementAge);
        }

        [HttpPost]
        [Route("UpdateCashflows")]
        public bool UpdateCashflow(List<Cashflow> cashflow)
        {
            _logger.LogInformation("Update Cashflows");
            // DOne and Tested OK.
            return manager.UpdateCashflow(cashflow, _cache);
            
        }

        [HttpPost]
        [Route("SetClients")]
        public bool SetClients(List<Client> clientList)
        {
            _logger.LogInformation("Adding Clients");
            // DOne and Tested OK.
            return manager.AddClients(clientList, _cache);

        }

        [HttpGet]
        [Route("GetCashflows/{clientId}")]
        public List<Cashflow> getCashflows(string clientId)
        {
            _logger.LogInformation("Getting Cashflows for - {0}", clientId);
            // Done and Tested OK.
            return manager.GetCashflows(clientId,_cache,connection);

        }

        [HttpGet]
        [Route("GetAllCashflows")]
        public List<Cashflow> getAllCashflows()
        {
           
            // Done and Tested OK.
            return manager.GetAllCashflows( _cache, connection);

        }

        [HttpGet]
        [Route("GetAccounts/{clientId}")]
        public List<Account> getAccounts(string clientId)
        {
            _logger.LogInformation("Getting Accounts for - {0}", clientId);
            return manager.GetAccounts(clientId,blobContainerClient);
        }

        [HttpGet]
        [Route("GetPlan/{clientId}")]
        public Plan getPlan(string clientId)
        {
            _logger.LogInformation("Getting Plan Details for - {0}", clientId);
            return manager.GetPlan(clientId,_cache,connection,blobContainerClient);
        }

        [HttpPost]
        [Route("AnalyzePlan")]
        public int analyzePlan(Plan plan)
        {
            _logger.LogInformation("Returning POS");
            string pos = string.Empty;
            string endpoint = "http://40.88.230.48/Analytics/AnalyzePlan";
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(endpoint);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                var content = new StringContent(JsonConvert.SerializeObject(plan,new JsonSerializerSettings { ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()}),Encoding.UTF8,"application/json");
                var response = client.PostAsync("", content);
                if(response.Result.IsSuccessStatusCode)
                {
                    Task<string> contentTask = response.Result.Content.ReadAsStringAsync();
                    pos = contentTask.Result;
                }
                else
                {
                    _logger.LogError("POS analysis failed!!!");
                }
            }
            return Convert.ToInt32(pos);
        }

    }
}
