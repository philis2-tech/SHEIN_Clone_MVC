using SHEIN_Clone_MVC.Models.Seed;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace SHEIN_Clone_MVC.Helpers
{
    public static class FakeProductGenerator
    {
        public static List<SeedProductDto> Generate(int count, string webRootPath)
        {
            var products = new List<SeedProductDto>();

            string[] names =
            {
                "Casual Hoodie",
                "Summer Dress",
                "Sneakers",
                "Denim Jacket",
                "Sport Leggings",
                "Leather Bag",
                "Crop Top",
                "Cargo Pants"
            };

            // FIX 1: true randomness (IMPORTANT)
            var random = new Random();

            var imagesPath = Path.Combine(webRootPath, "images");

            var imageFiles = Directory.Exists(imagesPath)
                ? Directory.GetFiles(imagesPath, "*.png", SearchOption.AllDirectories).ToList()
                : new List<string>();

            if (!imageFiles.Any())
                return products;

            for (int i = 1; i <= count; i++)
            {
                var randomImage = imageFiles[random.Next(imageFiles.Count)];

                var fileName = Path.GetFileName(randomImage);

                var folder = new DirectoryInfo(Path.GetDirectoryName(randomImage)).Name;

                products.Add(new SeedProductDto
                {
                    Name = names[random.Next(names.Length)] + " " + i,
                    Description = "Trending fashion item",
                    ImageBasePath = "/images/" + folder,

                    Variants = new List<SeedVariantDto>
                    {
                        new SeedVariantDto
                        {
                            Color = "Black",
                            Size = "M",
                            Price = random.Next(100, 800),
                            Stock = random.Next(5, 50),

                            Images = new List<SeedImageDto>
                            {
                                new SeedImageDto
                                {
                                    File = fileName,
                                    IsPrimary = true
                                }
                            }
                        }
                    }
                });
            }

            return products;
        }
    }
}