using System.Net;
using azure_function_net7_demo.Models.TodoModel;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace azure_function_net7_demo.Api.TodoApi
{
    public class GetTodos
    {
        private readonly ILogger _logger;
        private readonly IRepository<Todo> todoRepository;

        public GetTodos(ILoggerFactory loggerFactory, IRepository<Todo> todoRepository)
        {
            _logger = loggerFactory.CreateLogger<GetTodos>();
            this.todoRepository = todoRepository;
        }

        [Function("GetTodos")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todo")] HttpRequestData req)
        {
            _logger.LogInformation("Getting todos");

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

            var todos = await todoRepository.PageAsync(pageNumber: page, pageSize: size);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(todos.Items);
            return response;
        }
    }
}
