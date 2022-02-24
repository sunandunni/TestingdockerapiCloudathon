using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Testingdockerapi.Repository;
using Testingdockerapi.Entities;

namespace Testingdockerapi.Business
{
    public class PlanManager
    {
        public PlanRepository repository = new PlanRepository();

        public Client GetClient(string name) {
            return repository.GetClient(name);
        }

        public Goal GetGoal(int clientId) {
            return repository.GetGoal(clientId);
        }

        public List<Cashflow> getCashflows(int clientId) {
            return repository.GetCashflows(clientId);
        
        }

        public List<Account> getAccounts(int clientId) {
            return repository.GetAccounts(clientId);
        }

        public Plan getPlan(int clientId) {
            return repository.GetPlan(clientId);
        }
      
    }
}
