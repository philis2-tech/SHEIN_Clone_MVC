using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SHEIN_Clone_MVC.Models
{
    public class ProductVariant
    {
        public int Id { get; set; }

        [Required]
        public string? Color { get; set; }

        [Required]
        public string? Size { get; set; }

        public decimal Price { get; set; }

        public int Stock { get; set; }

        public ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

        public int ProductId { get; set; }

        public Product Product { get; set; }
    }
}