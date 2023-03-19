using Microsoft.Azure.CosmosRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace azure_function_net7_demo.Models.TodoModel
{
    public class Todo : Item
    {
        public Guid TodoId { get; set; } = Guid.NewGuid();
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
        public string TaskDescription { get; set; } = string.Empty;
        public bool IsCompleted { get; set; } = false;
    }
}
