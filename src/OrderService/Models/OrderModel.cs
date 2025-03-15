using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Models
{
    public class OrderModel
    {
        public string OrderId { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; }
        public Dictionary<string, int> Items { get; set; }  // ID and amount
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = "Pending";
        public string CreatedAt { get; set; }= DateTime.UtcNow.ToString("o");   // iso 8601 format
    }

    public class CreateOrderRequest
    {
        public string UserId { get; set; }
        public Dictionary<string, int> Items { get; set; }
    }
}
