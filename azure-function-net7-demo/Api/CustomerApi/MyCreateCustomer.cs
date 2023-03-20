using System.Net;
using System.Text.Json;
using azure_function_net7_demo.Models;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace azure_function_net7_demo.Api.CustomerApi
{
    public class MyCreateCustomer
    {
        private readonly ILogger _logger;
        private readonly IRepository<Customer> customerRepository;

        public MyCreateCustomer(ILoggerFactory loggerFactory, IRepository<Customer> customerRepository)
        {
            _logger = loggerFactory.CreateLogger<MyCreateCustomer>();
            this.customerRepository = customerRepository;
        }

        [Function("CreateCustomer")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "customer")] HttpRequestData req)
        {
            _logger.LogInformation("Customer Api function processed a http request.");

            var newCustomer = await JsonSerializer.DeserializeAsync<Customer>(req.Body);
            if (newCustomer == null) return req.CreateResponse(HttpStatusCode.BadRequest);

            var existingCustomer = await customerRepository.GetAsync(x => x.Email == newCustomer.Email);
            if(existingCustomer != null) return req.CreateResponse(HttpStatusCode.Conflict);
            
            var created = await customerRepository.CreateAsync(newCustomer);
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(created);
            return response;
        }
    }
}
