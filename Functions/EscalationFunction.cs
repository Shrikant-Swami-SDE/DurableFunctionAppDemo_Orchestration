using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace DurableFunctionAppDemo.Functions
{
    public static class EscalationFunction
    {
        [FunctionName("Escalation")]
        public static string Run([ActivityTrigger] string name, ILogger log)
        {
            log.LogInformation($"Escalating the approval process for {name}.");
            return $"ESCALATION: You have not approved the project design proposal - reassigning to your Manager, {name}!";
        }
    }
}
