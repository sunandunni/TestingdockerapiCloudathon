using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Testingdockerapi.Repository;
using Testingdockerapi.Entities;
using Microsoft.Extensions.Caching.Distributed;
using TestingDockerApi;
using StackExchange.Redis;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;

namespace Testingdockerapi.Business
{
    public class PlanManager
    {
        public PlanRepository repository = new PlanRepository();

        //public static ILoggerFactory LoggerFactory1 { get; } = new LoggerFactory();
        //ILogger log = null;
       
        //public static ILogger CreateLogger<PlanManager>()
        //{
        //    var logger = LoggerFactory1.CreateLogger<PlanManager>();
        //    return logger;
        //}

      

        //ILogger<PlanManager> _logger = new Logger<PlanManager>()

       
        //public PlanManager()
        //{
        //    log = CreateLogger<PlanManager>();
        //}


        public List<Client> GetClient(string name, IDistributedCache _cache, ConnectionMultiplexer connection, bool getAllClients = false)
        {
            //Logger<PlanManager>.log.LogInformation("Getting Client");
            var clientList = repository.GetClient(name, _cache, connection,getAllClients).Result;
            foreach (var client in clientList)
            {
                var isRetired = client.retirementAge - client.currentAge < 0;
                client.goal.startYear = isRetired ? DateTime.Now.Year : DateTime.Now.Year + (client.retirementAge - client.currentAge + 1);
                client.goal.endYear = DateTime.Now.Year + (100 - client.currentAge + 1);
            }
            return clientList;
        }

        public List<Client> GetAllClients(IDistributedCache _cache, ConnectionMultiplexer connection)
        {
            //Logger<PlanManager>.log.LogInformation("Getting Client");
            var clientList = repository.GetAllClients(_cache, connection).Result;
            foreach (var client in clientList)
            {
                var isRetired = client.retirementAge - client.currentAge < 0;
                client.goal.startYear = isRetired ? DateTime.Now.Year : DateTime.Now.Year + (client.retirementAge - client.currentAge + 1);
                client.goal.endYear = DateTime.Now.Year + (100 - client.currentAge + 1);
            }
            return clientList;
        }

        public Client GetSingleClient(string name, IDistributedCache _cache)
        {
            //Logger<PlanManager>.log.LogInformation("Getting single client");
            var client = repository.GetSingleClient(name, _cache).Result;

            var isRetired = client.retirementAge - client.currentAge < 0;
            client.goal.startYear = isRetired ? DateTime.Now.Year : DateTime.Now.Year + (client.retirementAge - client.currentAge + 1);
            client.goal.endYear = DateTime.Now.Year + (100 - client.currentAge + 1);

            return client;
        }

        public Goal GetGoal(int clientId, IDistributedCache _cache)
        {

            return repository.GetGoal(clientId, _cache).Result;
        }

        public bool UpdateClient(string clientId, IDistributedCache _cache, double goalAmount, int retirementAge)
        {
            return repository.UpdateClient(clientId, _cache, goalAmount, retirementAge).Result;
        }


        public List<Cashflow> GetCashflows(string clientId, IDistributedCache _cache, ConnectionMultiplexer connection)
        {
            return repository.GetCashflows(clientId, _cache, connection);

        }

        public List<Cashflow> GetAllCashflows(IDistributedCache _cache, ConnectionMultiplexer connection, bool getCashflows = false)
        {
            return repository.GetAllCashflows(_cache, connection);

        }

        public bool UpdateCashflow(List<Cashflow> cashflows, IDistributedCache _cache)
        {
            return repository.UpdateCashflow(cashflows, _cache).Result;

        }

        public bool AddClients(List<Client> clientList, IDistributedCache _cache)
        {
            return repository.AddClients(clientList, _cache).Result;

        }

        public List<Account> GetAccounts(string clientId, BlobContainerClient blobContainerClient)
        {
            var accounts = repository.GetAccounts(clientId, blobContainerClient);
            return accounts;
        }

        public Plan GetPlan(string clientId, IDistributedCache _cache, ConnectionMultiplexer connection, BlobContainerClient blobContainerClient)
        {
            Plan plan = new Plan();
            plan.clientId = clientId;
            plan.client = GetSingleClient(clientId, _cache);
            //plan.goal = GetGoal(clientId, _cache).Result;
            plan.cashflows = GetCashflows(clientId, _cache, connection);
            plan.accounts = GetAccounts(clientId, blobContainerClient);
            plan.allocations = new Allocations();
            plan.allocations.cash = 50;
            plan.allocations.equities = 50;
            return plan;
        }

    }
}
