using System.Net;
using azure_function_net7_demo.Models;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace azure_function_net7_demo.Api.TodoApi
{
    public class DeleteTodo
    {
        private readonly ILogger _logger;
        private readonly IRepository<Todo> repository;

        public DeleteTodo(ILoggerFactory loggerFactory, IRepository<Todo> repository)
        {
            _logger = loggerFactory.CreateLogger<DeleteTodo>();
            this.repository = repository;
        }

        [Function("DeleteTodo")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "todo/{id}")] HttpRequestData req, string id)
        {
            _logger.LogInformation("Deleting a todo.");

            if(string.IsNullOrEmpty(id)) return req.CreateResponse(HttpStatusCode.BadRequest);

            var todo = await repository.GetAsync(id);
            if(todo == null) return req.CreateResponse(HttpStatusCode.BadRequest);

            await repository.DeleteAsync(todo);

            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}
