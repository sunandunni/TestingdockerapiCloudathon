using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Testingdockerapi.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using TestingDockerApi;

namespace Testingdockerapi.Repository
{
    public class PlanRepository
    {
        public async Task<List<Client>> GetClient(string name, IDistributedCache _cache) {

            //TO DO   -- get list of keys starting with CL..get values from that key and return clients containing the name
            //1. GetHashCode all keys
            //2. Get all values for that key
            //3. From that list return only clients, wfor which name contains the passed name.

            //string employeeDetails = string.Empty;
            //employeeDetails = await _cache.GetStringAsync(name);
            //if (!string.IsNullOrEmpty(employeeDetails))
            //{
            //    var employeeDetailsValue = JsonConvert.DeserializeObject<Client>(employeeDetails);

            //    return employeeDetailsValue;
            //    // loaded data from the redis cache.
            //    //myTodos = JsonSerializer.Deserialize<List<string>>(cachedTodosString);
            //    //IsCached = true;
            //}
            //else
            //{
            //    return null;
            //}
            return null;
        }

        public async Task<Client> GetClient(int clientId, IDistributedCache _cache)
        {
            string employeeDetails = string.Empty;
            employeeDetails = await _cache.GetStringAsync(clientId.ToString());
            if (!string.IsNullOrEmpty(employeeDetails))
            {
                var employeeDetailsValue = JsonConvert.DeserializeObject<Client>(employeeDetails);

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

        public async Task<Goal> GetGoal(int clientId, IDistributedCache _cache)
        {
            string employeeDetails = string.Empty;
            employeeDetails = await _cache.GetStringAsync(clientId.ToString());
            if (!string.IsNullOrEmpty(employeeDetails))
            {
                var employeeDetailsValue = JsonConvert.DeserializeObject<Client>(employeeDetails);

                //To Do map to goal object and return
                return null;
                // loaded data from the redis cache.
                //myTodos = JsonSerializer.Deserialize<List<string>>(cachedTodosString);
                //IsCached = true;
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> AddGoal(int clientId, IDistributedCache _cache)
        {
            var key = clientId;
            //List<string> myTodos = new List<string>();
            //bool IsCached = false;
            string employeeDetails = string.Empty;
            employeeDetails = await _cache.GetStringAsync(key.ToString());
            if (string.IsNullOrEmpty(employeeDetails))
            {
                return false;
                // loaded data from the redis cache.
                //myTodos = JsonSerializer.Deserialize<List<string>>(cachedTodosString);
                //IsCached = true;
            }
            else
            {
                var employeeDetailsValue = JsonConvert.DeserializeObject<Client>(employeeDetails);
                //1 Deserialize to linet object
                //2. Add goal amount
                //3. COnvert to string
                await _cache.SetStringAsync(key.ToString(), "employee value");
                return true;
            }
        }

        public List<Cashflow> GetCashflows(int clientId, IDistributedCache _cache)
        {
            //TO DO   -- get list of keys starting with CF..get values from that key and return clients containing the name
            //1. GetHashCode all keys
            //2. Get all values for that key
            //3. From that list return only cashflows, wfor which clientid equals the passed clientid.

            //string employeeDetails = string.Empty;
            //employeeDetails = await _cache.GetStringAsync(name);
            //if (!string.IsNullOrEmpty(employeeDetails))
            //{
            //    var employeeDetailsValue = JsonConvert.DeserializeObject<Client>(employeeDetails);

            //    return employeeDetailsValue;
            //    // loaded data from the redis cache.
            //    //myTodos = JsonSerializer.Deserialize<List<string>>(cachedTodosString);
            //    //IsCached = true;
            //}
            //else
            //{
            //    return null;
            //}
            return null;
        }
        public List<Account> GetAccounts(int clientId, IDistributedCache _cache)
        {
            var accounts  = AzureBlobStorage.GetBlob("AccountfileName");
            //Logic to change the string to accounts for the client id

            //1 . Either read blob storage during startup or create in constructor
            return null;
        }

        public Plan GetPlan(int clientId, IDistributedCache _cache) {
            Plan plan = new Plan();
            plan.clientId = clientId;
            plan.client = GetClient(clientId,_cache).Result;
            plan.goal = GetGoal(clientId,_cache).Result;
            plan.cashflows = GetCashflows(clientId,_cache);
            plan.accounts = GetAccounts(clientId,_cache);
            return plan;
        }
    }
}
