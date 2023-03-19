using Microsoft.Azure.CosmosRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace azure_function_net7_demo.Models
{
    public class TodoCreateModel : Item
    {
        public string TaskDescription { get; set; } = string.Empty;
    }
}
