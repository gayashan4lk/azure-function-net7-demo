using System.Net;
using System.Text.Json;
using azure_function_net7_demo.Models;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace azure_function_net7_demo.Api
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

        [Function("GetCustomers")]
        public async Task<HttpResponseData> GetCustomers([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "customer")] HttpRequestData req)
        {
            _logger.LogInformation("Get Customers");

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

            try
            {
                var customers = await customerRepository.PageAsync(pageNumber: page, pageSize: size);

                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync(customers.Items);
                return response;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return req.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        [Function("CreateCustomer")]
        public async Task<HttpResponseData> CreateCustomer([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "customer")] HttpRequestData req)
        {
            _logger.LogInformation("Create Customer");

            try
            {
                var newCustomer = await JsonSerializer.DeserializeAsync<Customer>(req.Body);
                if (newCustomer == null) return req.CreateResponse(HttpStatusCode.BadRequest);

                var existingCustomer = await customerRepository.GetAsync(x => x.Email == newCustomer.Email);
                if (existingCustomer.Any()) return req.CreateResponse(HttpStatusCode.Conflict);

                var created = await customerRepository.CreateAsync(newCustomer);
                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync(created);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return req.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        [Function("DeleteCustomer")]
        public async Task<HttpResponseData> DeleteCustomer([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "customer/{id}")] HttpRequestData req, string id)
        {
            _logger.LogInformation("Delete Customer");

            if (string.IsNullOrEmpty(id)) return req.CreateResponse(HttpStatusCode.BadRequest);

            try
            {
                var customer = await customerRepository.GetAsync(id);

                if (customer == null) return req.CreateResponse(HttpStatusCode.BadRequest);

                await customerRepository.DeleteAsync(customer);

                return req.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return req.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        [Function("UpdateCustomer")]
        public async Task<HttpResponseData> UpdateCustomer([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "customer/{id}")] HttpRequestData req, string id)
        {
            _logger.LogInformation("Update Customer");

            try
            {
                var updateCustomer = await JsonSerializer.DeserializeAsync<Customer>(req.Body);
                if (string.IsNullOrEmpty(id) || updateCustomer == null) return req.CreateResponse(HttpStatusCode.BadRequest);

                var customer = await customerRepository.GetAsync(id);
                if (customer == null) return req.CreateResponse(HttpStatusCode.NotFound);

                customer.FirstName = updateCustomer.FirstName;
                customer.LastName = updateCustomer.LastName;

                var updated = await customerRepository.UpdateAsync(customer);
                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync(updated);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return req.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }
    }
}
