using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace azure_function_net7_demo
{
    public class Test
    {
        private readonly ILogger _logger;

        public Test(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Test>();
        }

        [Function("Function1")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route ="test")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.BadRequest);
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");

            response.WriteString("Bad request!");

            return response;
        }
    }
}
