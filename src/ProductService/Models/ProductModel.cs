

namespace ProductService.Models
{
    public class ProductModel
    {
        public string PK {  get; set; }     // e.g. STORE#123
        public string SK { get; set; }      // e.g. PRODUCT#456
        public string EntityType { get; set; } = "PRODUCT";
        public string ProductName {  get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
    }
    public class AddProductDto
    {
        public string StoreId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public string? Category { get; set; }
        public string? Description { get; set; }
    }

    public class GetProductDto
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? Category { get; set; }
    }
}
