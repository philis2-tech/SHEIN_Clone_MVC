using Microsoft.EntityFrameworkCore;
using SHEIN_Clone_MVC.Helpers;
using SHEIN_Clone_MVC.Models;
using SHEIN_Clone_MVC.Models.Seed;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace SHEIN_Clone_MVC.Data
{
    public static class DbInitializer
    {
        public static void Seed(ApplicationDbContext context)
        {
            // -------------------------
            // ENSURE CATEGORIES EXIST
            // -------------------------
            string[] categoryNames =
            {
                "Women",
                "Men",
                "Kids",
                "Shoes",
                "Beachwear",
                "Nightwear",
                "Activewear",
                "Jewellery & Accessories",
                "Bags & Luggages",
                "Toys",
                "Appliances"
            };

            foreach (var categoryName in categoryNames)
            {
                if (!context.Categories.Any(c => c.Name == categoryName))
                {
                    context.Categories.Add(
                        new Category
                        {
                            Name = categoryName
                        });
                }
            }

            context.SaveChanges();

            // -------------------------
            // LOAD CATEGORY CACHE
            // -------------------------
            CategoryResolver.LoadCategories(context);

            // -------------------------
            // IMAGE CHECKS
            // -------------------------
            ImageScanner.ScanForMissingImages();

            // -------------------------
            // WEB ROOT
            // -------------------------
            var webRootPath = Path.GetFullPath(
                Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot"
                )
            );

            // -------------------------
            // WATCH FOR CHANGES
            // -------------------------
            ImageFolderWatcher.Start(webRootPath);

            // -------------------------
            // DEBUG INFO
            // -------------------------
            int currentCount = context.Products.Count();

            Console.WriteLine(
                $"Current products in database: {currentCount}"
            );

            // -------------------------
            // LOAD PRODUCTS FROM IMAGES
            // -------------------------
            var seedProducts =
                ImageSyncService.GenerateProductsFromImages(webRootPath)
                ?? new List<SeedProductDto>();

            Console.WriteLine(
                $"ImageSync found {seedProducts.Count} products"
            );

            // -------------------------
            // CONVERT TO ENTITIES
            // -------------------------
            var products = new List<Product>();

            foreach (var item in seedProducts)
            {
                if (item.Variants == null ||
                    !item.Variants.Any())
                {
                    continue;
                }

                var product = new Product
                {
                    Name = item.Name,
                    Description = item.Description,

                    ProductVariants =
                        item.Variants.Select(v =>
                        {
                            var uniqueImages =
                                (v.Images ??
                                 new List<SeedImageDto>())
                                .GroupBy(i =>
                                    i.File.ToLower())
                                .Select(g =>
                                    g.First())
                                .ToList();

                            return new ProductVariant
                            {
                                Color = v.Color ?? "Default",
                                Size = v.Size ?? "One Size",
                                Price = v.Price,
                                Stock = v.Stock,

                                ProductImages =
                                    uniqueImages.Select(img =>
                                        new ProductImage
                                        {
                                            ImagePath =
                                                $"{item.ImageBasePath}/{img.File}",
                                            IsPrimary =
                                                img.IsPrimary
                                        })
                                    .ToList()
                            };
                        })
                        .ToList()
                };

                var firstImage =
                    product.ProductVariants
                        .FirstOrDefault()?
                        .ProductImages
                        .FirstOrDefault()?
                        .ImagePath;

                if (!string.IsNullOrWhiteSpace(firstImage))
                {
                    product.CategoryId =
                        CategoryResolver
                            .GetCategoryId(firstImage);
                }

                products.Add(product);
            }

            // -------------------------
            // SAVE AND UPDATE PRODUCTS
            // -------------------------

            foreach (var product in products)
            {
                var existingProduct =
                    context.Products
                    .Include(p => p.ProductVariants)
                    .FirstOrDefault(p =>
                        p.Name.ToLower() ==
                        product.Name.ToLower());

                if (existingProduct == null)
                {
                    context.Products.Add(product);
                }
                else
                {
                    // Add missing variants/sizes
                    foreach (var newVariant
                             in product.ProductVariants)
                    {
                        bool variantExists =
                            existingProduct.ProductVariants
                            .Any(v =>
                                v.Size ==
                                newVariant.Size);

                        if (!variantExists)
                        {
                            existingProduct.ProductVariants
                                .Add(newVariant);
                        }
                    }
                }
            }

            context.SaveChanges();

            Debug.WriteLine(
                $"Current products in database: {currentCount}"
            );
        }
    }
}
