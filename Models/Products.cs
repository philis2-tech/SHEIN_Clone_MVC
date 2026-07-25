
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SHEIN_Clone_MVC.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

        public List<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();

        [Required]
        public int CategoryId { get; set; }

        public Category Category { get; set; }
    }
}