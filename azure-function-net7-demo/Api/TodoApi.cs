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
    public class TodoApi
    {
        private readonly ILogger _logger;
        private readonly IRepository<Todo> todoRepository;

        public TodoApi(ILoggerFactory loggerFactory, IRepository<Todo> todoRepository)
        {
            _logger = loggerFactory.CreateLogger<TodoApi>();
            this.todoRepository = todoRepository;
        }

        [Function("GetTodos")]
        public async Task<HttpResponseData> GetTodos([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todo")] HttpRequestData req)
        {
            _logger.LogInformation("Get Todos");

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
                var todos = await todoRepository.PageAsync(pageNumber: page, pageSize: size);
                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync(todos.Items);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return req.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        [Function("CreateTodo")]
        public async Task<HttpResponseData> CreateTodo([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "todo")] HttpRequestData req)
        {
            _logger.LogInformation("Create Todo");

            try
            {
                var todo = await JsonSerializer.DeserializeAsync<Todo>(req.Body);
                if (todo == null) return req.CreateResponse(HttpStatusCode.BadRequest);

                var created = await todoRepository.CreateAsync(todo);

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

        [Function("DeleteTodo")]
        public async Task<HttpResponseData> DeleteTodo([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "todo/{id}")] HttpRequestData req, string id)
        {
            _logger.LogInformation("Delete Todo");

            if (string.IsNullOrEmpty(id)) return req.CreateResponse(HttpStatusCode.BadRequest);

            try
            {
                var todo = await todoRepository.GetAsync(id);
                if (todo == null) return req.CreateResponse(HttpStatusCode.BadRequest);

                await todoRepository.DeleteAsync(todo);

                return req.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return req.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        [Function("DeleteSubTask")]
        public async Task<HttpResponseData> DeleteSubTask([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "todo/{todoId}/subtask/{subTaskId}")] HttpRequestData req, string todoId, string subTaskId)
        {
            _logger.LogInformation("Delete SubTask");

            if (string.IsNullOrEmpty(subTaskId) || string.IsNullOrEmpty(todoId)) return req.CreateResponse(HttpStatusCode.BadRequest);

            try
            {
                var todo = await todoRepository.GetAsync(todoId);
                if (todo == null) return req.CreateResponse(HttpStatusCode.NotFound);

                var subTaskIndex = todo.SubTasks?.FindIndex(subTask => subTask.Id == subTaskId);

                if (!subTaskIndex.HasValue || subTaskIndex.Value < 0) return req.CreateResponse(HttpStatusCode.NotFound);

                todo.SubTasks?.RemoveAt(subTaskIndex.Value);
                await todoRepository.UpdateAsync(todo);
                return req.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return req.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }
    }
}
