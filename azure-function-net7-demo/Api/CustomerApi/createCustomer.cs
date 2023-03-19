using System.Net;
using System.Text.Json;
using azure_function_net7_demo.Models.CustomerModel;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace azure_function_net7_demo.Api.CustomerApi
{
    public class CreateCustomer
    {
        private readonly ILogger _logger;
        private readonly IRepository<Customer> customerRepository;

        public CreateCustomer(ILoggerFactory loggerFactory, IRepository<Customer> customerRepository)
        {
            _logger = loggerFactory.CreateLogger<CreateCustomer>();
            this.customerRepository = customerRepository;
        }

        [Function("CreateCustomer")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "customer")] HttpRequestData req)
        {
            _logger.LogInformation("Customer Api function processed a http request.");

            var customer = await JsonSerializer.DeserializeAsync<Customer>(req.Body);

            var response = req.CreateResponse(HttpStatusCode.OK);

            if (customer != null)
            {
                var created = await customerRepository.CreateAsync(customer);
                await response.WriteAsJsonAsync(created);
            }

            return response;
        }
    }
}
