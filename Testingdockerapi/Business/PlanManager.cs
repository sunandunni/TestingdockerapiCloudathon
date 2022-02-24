using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Testingdockerapi.Repository;
using Testingdockerapi.Entities;
using Microsoft.Extensions.Caching.Distributed;
using TestingDockerApi;

namespace Testingdockerapi.Business
{
    public class PlanManager
    {
        public PlanRepository repository = new PlanRepository();

        public List<Client> GetClient(string name, IDistributedCache _cache){
            return repository.GetClient(name,_cache).Result;
        }

        public Goal GetGoal(int clientId, IDistributedCache _cache) {
            return repository.GetGoal(clientId, _cache).Result;
        }

        public List<Cashflow> GetCashflows(int clientId, IDistributedCache _cache) {
            return repository.GetCashflows(clientId,_cache);
        
        }

        public List<Account> GetAccounts(int clientId) {
            return null;
        }

        public Plan GetPlan(int clientId, IDistributedCache _cache) {
            return repository.GetPlan(clientId, _cache);
        }
      
    }
}
