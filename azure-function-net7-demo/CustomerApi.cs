using System.Net;
using System.Text.Json;
using azure_function_net7_demo.Models;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace azure_function_net7_demo
{
    public class CustomerApi
    {
        private readonly ILogger _logger;
        private readonly IRepository<Customer> customerRepository;

        public CustomerApi(ILoggerFactory loggerFactory, IRepository<Customer> customerRepository)
        {
            _logger = loggerFactory.CreateLogger<CustomerApi>();
            this.customerRepository = customerRepository;
        }

        [Function("CustomerApi")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            _logger.LogInformation("Customer Api function processed a http request.");

            var customer = await JsonSerializer.DeserializeAsync<Customer>(req.Body);

            var response = req.CreateResponse(HttpStatusCode.OK);

            if(customer != null)
            {
                var created = await customerRepository.CreateAsync(customer);
                await response.WriteAsJsonAsync(created);
            }

            //response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            //response.WriteString("Welcome to Azure Functions!");

            return response;
        }
    }
}
