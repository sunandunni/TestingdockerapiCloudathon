using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Testingdockerapi.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using TestingDockerApi;
using StackExchange.Redis;
using Azure.Storage.Blobs;

namespace Testingdockerapi.Repository
{
    public class PlanRepository
    {

        // ALL Ids should be in string
        public async Task<List<Client>> GetClient(string name, IDistributedCache _cache, ConnectionMultiplexer connection, bool getAllClients = false)
        {

            //TO DO   -- get list of keys starting with CL..get values from that key and return clients containing the name
            //1. GetHashCode all keys
            //2. Get all values for that key
            //3. From that list return only clients, wfor which name contains the passed name.
            List<Client> clientList = new List<Client>();
            var endPoint = connection.GetEndPoints().First();
            RedisKey[] keys = connection.GetServer(endPoint).Keys(pattern: "*").ToArray();

            foreach (var key in keys)
            {
                if (!key.ToString().Contains("-CF") && key.ToString().Contains("CL"))
                {
                    if (key.ToString().StartsWith("CL"))
                    {
                        var client = _cache.GetStringAsync(key).Result;

                        if (client != null)
                        {
                            var ClientData = JsonConvert.DeserializeObject<Client>(client);
                            if(getAllClients)
                            {
                                clientList.Add(ClientData);
                            }
                            else if (ClientData.name.Contains(name, StringComparison.InvariantCultureIgnoreCase))
                            {
                                clientList.Add(ClientData);
                            }
                        }
                    }
                    else
                    {
                        var db = connection.GetDatabase();
                        db.KeyDelete(key);
                    }
                }
            }

            return clientList;
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
            //return null;
        }


        public async Task<List<Client>> GetAllClients(IDistributedCache _cache, ConnectionMultiplexer connection)
        {

            //TO DO   -- get list of keys starting with CL..get values from that key and return clients containing the name
            //1. GetHashCode all keys
            //2. Get all values for that key
            //3. From that list return only clients, wfor which name contains the passed name.
            List<Client> clientList = new List<Client>();
            var endPoint = connection.GetEndPoints().First();
            RedisKey[] keys = connection.GetServer(endPoint).Keys(pattern: "*").ToArray();

            foreach (var key in keys)
            {
                if (!key.ToString().Contains("-CF") && key.ToString().Contains("CL"))
                {
                    if (key.ToString().StartsWith("CL"))
                    {
                        var client = _cache.GetStringAsync(key).Result;

                        if (client != null)
                        {
                            var ClientData = JsonConvert.DeserializeObject<Client>(client);

                            clientList.Add(ClientData);

                        }
                    }
                    else
                    {
                        var db = connection.GetDatabase();
                        db.KeyDelete(key);
                    }
                }
            }

            return clientList;
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
            //return null;
        }
        public async Task<Client> GetSingleClient(string clientId, IDistributedCache _cache)
        {
            string clientDetails = string.Empty;
            clientDetails = await _cache.GetStringAsync(clientId.ToString());
            if (!string.IsNullOrEmpty(clientDetails))
            {
                var clientDetailsValue = JsonConvert.DeserializeObject<Client>(clientDetails);

                return clientDetailsValue;
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

        public async Task<bool> UpdateClient(string clientId, IDistributedCache _cache, double goalAmount, int retirementAge)
        {
            var key = clientId;
            //List<string> myTodos = new List<string>();
            //bool IsCached = false;
            string clientDetails = string.Empty;
            clientDetails = await _cache.GetStringAsync(key.ToString());
            if (string.IsNullOrEmpty(clientDetails))
            {
                return false;
                // loaded data from the redis cache.
                //myTodos = JsonSerializer.Deserialize<List<string>>(cachedTodosString);
                //IsCached = true;
            }
            else
            {
                var clientDetailsValue = JsonConvert.DeserializeObject<Client>(clientDetails);
                if (goalAmount > 0)
                {
                    clientDetailsValue.goal.amout = goalAmount;
                }
                if (retirementAge > 0)
                {
                    clientDetailsValue.retirementAge = retirementAge;
                }
                var updatedClient = JsonConvert.SerializeObject(clientDetailsValue);
                //1 Deserialize to linet object
                //2. Add goal amount
                //3. COnvert to string
                await _cache.SetStringAsync(key.ToString(), updatedClient);
                return true;
            }
        }

        public async Task<bool> UpdateCashflow(List<Cashflow> cashflows, IDistributedCache _cache)
        {
            foreach (var cashflow in cashflows)
            {
                var updatedCashdlow = JsonConvert.SerializeObject(cashflow);

                //    //1 Deserialize to linet object
                //    //2. Add goal amount
                //    //3. COnvert to string
                await _cache.SetStringAsync(cashflow.id.ToString(), updatedCashdlow);
            }
            return true;

        }

        public async Task<bool> AddClients(List<Client> clientList, IDistributedCache _cache)
        {
            foreach (var client in clientList)
            {
                var updatedClient = JsonConvert.SerializeObject(client);

                //    //1 Deserialize to linet object
                //    //2. Add goal amount
                //    //3. COnvert to string
                await _cache.SetStringAsync(client.id.ToString(), updatedClient);
            }
            return true;

        }


        public List<Cashflow> GetCashflows(string clientId, IDistributedCache _cache, ConnectionMultiplexer connection)
        {
            //TO DO   -- get list of keys starting with CL..get values from that key and return clients containing the name
            //1. GetHashCode all keys
            //2. Get all values for that key
            //3. From that list return only clients, wfor which name contains the passed name.
            List<Cashflow> cashflowList = new List<Cashflow>();
            var endPoint = connection.GetEndPoints().First();
            RedisKey[] keys = connection.GetServer(endPoint).Keys(pattern: clientId.ToString() + "-CF*").ToArray();

            foreach (var key in keys)
            {
                var cashflow = _cache.GetStringAsync(key).Result;
                if (cashflow != null)
                {
                    var cashflowData = JsonConvert.DeserializeObject<Cashflow>(cashflow);

                    cashflowList.Add(cashflowData);

                }
            }

            if (cashflowList.Count == 0)
            {
                cashflowList = null;
            }
            return cashflowList;
        }

        public List<Cashflow> GetAllCashflows(IDistributedCache _cache, ConnectionMultiplexer connection, bool getAllCashflows = false)
        {
            //TO DO   -- get list of keys starting with CL..get values from that key and return clients containing the name
            //1. GetHashCode all keys
            //2. Get all values for that key
            //3. From that list return only clients, wfor which name contains the passed name.
            List<Cashflow> cashflowList = new List<Cashflow>();
            var endPoint = connection.GetEndPoints().First();
            RedisKey[] keys = connection.GetServer(endPoint).Keys(pattern: "*-CF*").ToArray();

            foreach (var key in keys)
            {
                var cashflow = _cache.GetStringAsync(key).Result;
                if (cashflow != null)
                {
                    var cashflowData = JsonConvert.DeserializeObject<Cashflow>(cashflow);

                    cashflowList.Add(cashflowData);

                }
            }

            if (cashflowList.Count == 0)
            {
                cashflowList = null;
            }
            return cashflowList;
        }
        public List<Account> GetAccounts(string clientId, BlobContainerClient blobContainerClient)
        {
            var accounts = AzureBlobStorage.GetAccountBlob(clientId, blobContainerClient);
            //Logic to change the string to accounts for the client id

            //1 . Either read blob storage during startup or create in constructor
            return accounts.Result;
        }

        public Plan GetPlan(string clientId, IDistributedCache _cache, ConnectionMultiplexer connection, BlobContainerClient blobContainerClient)
        {
            Plan plan = new Plan();
            plan.clientId = clientId;
            plan.client = GetSingleClient(clientId, _cache).Result;
            //plan.goal = GetGoal(clientId, _cache).Result;
            plan.cashflows = GetCashflows(clientId, _cache, connection);
            plan.accounts = GetAccounts(clientId, blobContainerClient);
            return plan;
        }
    }
}
