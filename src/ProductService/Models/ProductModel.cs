using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.Models
{
    public class ProductModel
    {
        public string ProductId {  get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string StoreId { get; set; }
    }
    public class AddProductDto
    {
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public string? Category { get; set; }
        public string? Description { get; set; }
        public string StoreId { get; set; }
    }
}
