using System.ComponentModel.DataAnnotations;

namespace SHEIN_Clone_MVC.Models
{
    public class ProductImage
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string ImagePath { get; set; }

        public bool IsPrimary { get; set; } 

        public int ProductVariantId { get; set; }
        public ProductVariant ProductVariant { get; set; }
        
    }

}

