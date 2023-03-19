using System.Net;
using azure_function_net7_demo.Models.CustomerModel;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace azure_function_net7_demo.Api.CustomerApi
{
    public class GetCustomers
    {
        private readonly ILogger _logger;
        private readonly IRepository<Customer> customerRepository;

        public GetCustomers(ILoggerFactory loggerFactory, IRepository<Customer> customerRepository)
        {
            _logger = loggerFactory.CreateLogger<GetCustomers>();
            this.customerRepository = customerRepository;
        }

        [Function("GetCustomers")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "customer")] HttpRequestData req)
        {
            _logger.LogInformation("Getting customers");

            var queryDictionary = QueryHelpers.ParseQuery(req.Url.Query);
            var pageNumber = queryDictionary["pageNumber"];
            var pageSize = queryDictionary["pageSize"];

            if (string.IsNullOrWhiteSpace(pageNumber) || !int.TryParse(pageNumber, out var page) || page <= 0)
            {
                _logger.LogWarning("No pageNumber provided.");
                return req.CreateResponse(HttpStatusCode.BadRequest);
            }

            if (string.IsNullOrWhiteSpace(pageSize) || !int.TryParse(pageSize, out var size) || size <= 0)
            {
                _logger.LogWarning("No pageSize provided.");
                return req.CreateResponse(HttpStatusCode.BadRequest);
            }

            var customers = await customerRepository.PageAsync(pageNumber: page, pageSize: size);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(customers.Items);
            return response;
        }
    }
}
