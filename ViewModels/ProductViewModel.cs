using Microsoft.AspNetCore.Http;
using SHEIN_Clone_MVC.Models;
using System.Collections.Generic;

namespace SHEIN_Clone_MVC.ViewModels
{
    public class ProductViewModel
    {
        public Product Product { get; set; }

        public List<IFormFile> ImageFiles { get; set; } = new();
        public string ImagePath { get; set; }
    }
}


