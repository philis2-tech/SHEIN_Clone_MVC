using System.Collections.Generic;

namespace SHEIN_Clone_MVC.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Navigation: One category has many products
        public List<Product> Products { get; set; }
    }
}

