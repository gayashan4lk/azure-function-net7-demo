using System.Net;
using System.Text.Json;
using azure_function_net7_demo.Models;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace azure_function_net7_demo.Api
{
    public class CreateTodo
    {
        private readonly ILogger _logger;
        private readonly IRepository<Todo> todoRepository;

        public CreateTodo(ILoggerFactory loggerFactory, IRepository<Todo> todoRepository)
        {
            _logger = loggerFactory.CreateLogger<CreateTodo>();
            this.todoRepository = todoRepository;
        }

        [Function("CreateTodo")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "todo")] HttpRequestData req)
        {
            _logger.LogInformation("Creating a new todo list item.");

            var todo = await JsonSerializer.DeserializeAsync<Todo>(req.Body);

            if (todo == null) return req.CreateResponse(HttpStatusCode.BadRequest);

            var response = req.CreateResponse(HttpStatusCode.OK);
            var created = await todoRepository.CreateAsync(todo);
            await response.WriteAsJsonAsync(created);
            return response;
        }

        
    }
}
