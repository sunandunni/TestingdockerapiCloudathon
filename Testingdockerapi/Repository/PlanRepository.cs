using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Testingdockerapi.Entities;

namespace Testingdockerapi.Repository
{
    public class PlanRepository
    {
        public Client GetClient(string name) {
            return null;
        }

        public Client GetClient(int clientId)
        {
            return null;
        }

        public Goal GetGoal(int clientId)
        {
            return null;
        }

        public List<Cashflow> GetCashflows(int clientId)
        {
            return null;
        }
        public List<Account> GetAccounts(int clientId)
        {
            return null;
        }

        public Plan GetPlan(int clientId) {
            Plan plan = new Plan();
            plan.clientId = clientId;
            plan.client = GetClient(clientId);
            plan.goal = GetGoal(clientId);
            plan.cashflows = GetCashflows(clientId);
            plan.accounts = GetAccounts(clientId);
            return plan;
        }
    }
}
