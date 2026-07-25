namespace SHEIN_Clone_MVC.Models
{
    public class ProductSize
    {
        public int Id { get; set; }
        public string Size { get; set; }

        public int ProductVariantId { get; set; }
        public ProductVariant ProductVariant { get; set; }
    }
}
