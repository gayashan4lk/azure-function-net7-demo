using System.Net;
using System.Text.Json;
using azure_function_net7_demo.Models;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace azure_function_net7_demo.Api.CustomerApi
{
    public class UpdateCustomer
    {
        private readonly ILogger _logger;
        private readonly IRepository<Customer> customerRepository;

        public UpdateCustomer(ILoggerFactory loggerFactory, IRepository<Customer> customerRepository)
        {
            _logger = loggerFactory.CreateLogger<UpdateCustomer>();
            this.customerRepository = customerRepository;
        }

        [Function("UpdateCustomer")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route ="customer/{id}")] HttpRequestData req, string id)
        {
            _logger.LogInformation("Updating a customer.");

            var updateCustomer = await JsonSerializer.DeserializeAsync<Customer>(req.Body);
            if (string.IsNullOrEmpty(id) || updateCustomer == null) return req.CreateResponse(HttpStatusCode.BadRequest);

            var customer = await customerRepository.GetAsync(id);
            if(customer == null) return req.CreateResponse(HttpStatusCode.NotFound);

            customer.FirstName = updateCustomer.FirstName;
            customer.LastName = updateCustomer.LastName;

            var updated = await customerRepository.UpdateAsync(customer);
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(updated);
            return response;
        }
    }
}
