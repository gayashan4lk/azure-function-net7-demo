using System.Net;
using azure_function_net7_demo.Models;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace azure_function_net7_demo.Api.TodoApi
{
    public class DeleteSubTask
    {
        private readonly ILogger _logger;
        private readonly IRepository<Todo> todoRepository;

        public DeleteSubTask(ILoggerFactory loggerFactory, IRepository<Todo> todoRepository)
        {
            _logger = loggerFactory.CreateLogger<DeleteSubTask>();
            this.todoRepository = todoRepository;
        }


        [Function("DeleteSubTask")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "todo/{todoId}/subtask/{subTaskId}")] HttpRequestData req, string todoId, string subTaskId)
        {
            _logger.LogInformation("Deleting a subTask.");

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
