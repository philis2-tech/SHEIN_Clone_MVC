using SHEIN_Clone_MVC.Models.Seed;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace SHEIN_Clone_MVC.Helpers
{
    public static class AutoJsonGenerator
    {
        public static void Generate(string webRootPath)
        {
            var imagesRoot = Path.Combine(webRootPath, "Images");

            if (!Directory.Exists(imagesRoot))
            {
                Console.WriteLine("Images folder not found.");
                return;
            }

            var imageFiles = Directory.GetFiles(imagesRoot, "*.*", SearchOption.AllDirectories);

            var products = new Dictionary<string, SeedProductDto>();

            foreach (var file in imageFiles)
            {
                var fileName = Path.GetFileNameWithoutExtension(file);
                var ext = Path.GetExtension(file);
                var relativePath = file.Replace(webRootPath, "").Replace("\\", "/");

                // Expect: product-name-front.png
                var parts = fileName.Split('-');
                if (parts.Length < 2)
                    continue;

                var view = parts.Last(); // front, back, side
                var productSlug = string.Join("-", parts.Take(parts.Length - 1));

                if (!products.ContainsKey(productSlug))
                {
                    products[productSlug] = new SeedProductDto
                    {
                        Name = ToTitle(productSlug),
                        Description = "Auto generated product",
                        ImageBasePath = GetBasePath(relativePath),
                        Variants = new List<SeedVariantDto>()
                    };
                }

                var product = products[productSlug];

                if (!product.Variants.Any())
                {
                    product.Variants.Add(new SeedVariantDto
                    {
                        Color = "Default",
                        Size = "M",
                        Price = 0,
                        Stock = 0,
                        Images = new List<SeedImageDto>()
                    });
                }

                var variant = product.Variants.First();

                variant.Images.Add(new SeedImageDto
                {
                    File = Path.GetFileName(file),
                    IsPrimary = view == "front"
                });
            }

            var outputPath = Path.Combine(Directory.GetCurrentDirectory(), "SeedData", "products.json");

            var json = JsonSerializer.Serialize(products.Values.ToList(), new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(outputPath, json);

            Console.WriteLine($"✅ Generated {products.Count} products into JSON");
        }

        private static string GetBasePath(string relativePath)
        {
            var parts = relativePath.Split('/');
            return string.Join("/", parts.Take(parts.Length - 1));
        }

        private static string ToTitle(string slug)
        {
            return System.Globalization.CultureInfo.CurrentCulture.TextInfo
                .ToTitleCase(slug.Replace("-", " "));
        }
    }
}