using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OrderService.Models
{
    public class OrderModel
    {
        public string OrderId { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; }
        public Dictionary<string, int> Cart { get; set; }  // ID and amount
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = "Pending";
        public string CreatedAt { get; set; }= DateTime.UtcNow.ToString("o");   // iso 8601 format

        [JsonIgnore]
        public string PK => $"USER#{UserId}";      // fetch orders by user (for calculating the cart)
        [JsonIgnore]
        public string SK => $"ORDER#{OrderId}";    // sort orders
    }

    public class CreateOrderRequest
    {
        public string UserId { get; set; }
        public Dictionary<string, int> Cart { get; set; }
    }
}
