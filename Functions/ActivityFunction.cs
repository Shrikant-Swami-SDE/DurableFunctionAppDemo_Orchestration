using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DurableFunctionAppDemo.Functions
{
    public static class ActivityFunction
    {
        [FunctionName("Approval")]
        public static string Run([ActivityTrigger] string decision, ILogger log)
        {
            log.LogInformation($"Processing approval decision: {decision}");
            return $"Your project design proposal has been: {decision}";
        }
    }
}
