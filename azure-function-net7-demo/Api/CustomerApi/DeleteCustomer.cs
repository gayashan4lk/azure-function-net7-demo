using System.Net;
using System.Text.Json;
using azure_function_net7_demo.Models;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace azure_function_net7_demo.Api.CustomerApi
{
    public class DeleteCustomer
    {
        private readonly ILogger _logger;
        private readonly IRepository<Customer> customerRepository;

        public DeleteCustomer(ILoggerFactory loggerFactory, IRepository<Customer> customerRepository)
        {
            _logger = loggerFactory.CreateLogger<MyCreateCustomer>();
            this.customerRepository = customerRepository;
        }

        [Function("DeleteCustomer")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "customer/{id}")] HttpRequestData req, string id)
        {
            _logger.LogInformation("Deleting a customer.");

            if(string.IsNullOrEmpty(id)) return req.CreateResponse(HttpStatusCode.BadRequest);

            var customer = await customerRepository.GetAsync(id);

            if(customer == null) return req.CreateResponse(HttpStatusCode.BadRequest);

            await customerRepository.DeleteAsync(customer);

            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}
