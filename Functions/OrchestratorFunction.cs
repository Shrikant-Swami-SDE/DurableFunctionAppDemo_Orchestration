using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace DurableFunctionAppDemo.Functions
{
    public static class OrchestratorFunction
    {
        [FunctionName("OrchestratorFunction")]
        public static async Task<List<string>> RunOrchestrator(
        [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var outputs = new List<string>();

            // Set a 20-second deadline for the approval process
            DateTime deadline = context.CurrentUtcDateTime.AddSeconds(20);

            // Create a timer task
            var timeoutTask = context.CreateTimer(deadline, CancellationToken.None);

            // Wait for an external "Approval" event
            var approvalTask = context.WaitForExternalEvent<string>("Approval");

            // Wait for either approval or the timeout
            var winner = await Task.WhenAny(approvalTask, timeoutTask);

            // Check which task completed first
            if (winner == approvalTask)
            {
                // Call the approval activity if approval is received
                outputs.Add(await context.CallActivityAsync<string>("Approval", "Approved"));
            }
            else
            {
                // Call the escalation activity if timeout happens
                outputs.Add(await context.CallActivityAsync<string>("Escalation", "Head of Department"));
            }

            // Cancel the timeout task if it didn't complete
            if (!timeoutTask.IsCompleted)
            {
                timeoutTask.Dispose(); // Properly disposing of the timer
            }

            return outputs;
        }
    }
}

#region Default Code 


//    public static class OrchestratorFunction
//    {
//        [FunctionName("OrchestratorFunction")]
//        public static async Task<List<string>> RunOrchestrator(
//            [OrchestrationTrigger] IDurableOrchestrationContext context)
//        {
//            var outputs = new List<string>();

//            // Replace "hello" with the name of your Durable Activity Function.
//            outputs.Add(await context.CallActivityAsync<string>(nameof(SayHello), "Tokyo"));
//            outputs.Add(await context.CallActivityAsync<string>(nameof(SayHello), "Seattle"));
//            outputs.Add(await context.CallActivityAsync<string>(nameof(SayHello), "London"));

//            // returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
//            return outputs;
//        }

//        [FunctionName(nameof(SayHello))]
//        public static string SayHello([ActivityTrigger] string name, ILogger log)
//        {
//            log.LogInformation("Saying hello to {name}.", name);
//            return $"Hello {name}!";
//        }

//        [FunctionName("OrchestratorFunction_HttpStart")]
//        public static async Task<HttpResponseMessage> HttpStart(
//            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
//            [DurableClient] IDurableOrchestrationClient starter,
//            ILogger log)
//        {
//            // Function input comes from the request content.
//            string instanceId = await starter.StartNewAsync("OrchestratorFunction", null);

//            log.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);

//            return starter.CreateCheckStatusResponse(req, instanceId);
//        }
//    }
//}
#endregion