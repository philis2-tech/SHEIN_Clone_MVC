using System.Collections.Generic;

namespace SHEIN_Clone_MVC.Models.Seed
{
    public class SeedProductDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageBasePath { get; set; }
        public List<SeedVariantDto> Variants { get; set; }
    }

    public class SeedVariantDto
    {
        public string Color { get; set; }
        public string Size { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public List<SeedImageDto> Images { get; set; }
    }

    public class SeedImageDto
    {
        public string File { get; set; }
        public bool IsPrimary { get; set; }
    }
}