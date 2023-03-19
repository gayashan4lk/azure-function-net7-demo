using Microsoft.Azure.CosmosRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace azure_function_net7_demo.Models
{
    public class Customer : Item
    {
        public Guid CustomerId { get; set; } = Guid.NewGuid();
        public string FirstName { get; set; } = String.Empty;
        public string? LastName { get; set; }
        public string Email { get; set; } = String.Empty;
    }
}
