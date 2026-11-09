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
        // =========================================
        // GENERATE PRODUCT JSON
        // =========================================

        public static void Generate(string webRootPath)
        {
            try
            {
                // ---------------------------------
                // 1. LOCATE WWWROOT
                // ---------------------------------

                if (string.IsNullOrWhiteSpace(webRootPath))
                {
                    webRootPath = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot"
                    );

                    Console.WriteLine(
                        "WebRootPath was null. Using default wwwroot directory."
                    );
                }

                webRootPath = Path.GetFullPath(webRootPath);

                // ---------------------------------
                // 2. LOCATE IMAGES FOLDER
                // ---------------------------------

                var imagesRoot = Path.Combine(
                    webRootPath,
                    "Images"
                );

                if (!Directory.Exists(imagesRoot))
                {
                    Console.WriteLine(
                        $"Images folder not found: {imagesRoot}"
                    );

                    return;
                }

                // ---------------------------------
                // 3. GET IMAGE FILES
                // ---------------------------------

                var allowedExtensions = new[]
                {
                    ".png",
                    ".jpg",
                    ".jpeg",
                    ".webp"
                };

                var imageFiles = Directory
                    .GetFiles(
                        imagesRoot,
                        "*.*",
                        SearchOption.AllDirectories
                    )
                    .Where(file =>
                        allowedExtensions.Contains(
                            Path.GetExtension(file),
                            StringComparer.OrdinalIgnoreCase
                        )
                    )
                    .ToList();

                if (!imageFiles.Any())
                {
                    Console.WriteLine(
                        "No supported product images were found."
                    );

                    return;
                }

                // ---------------------------------
                // 4. INITIALISE PRODUCT COLLECTION
                // ---------------------------------

                var products =
                    new Dictionary<string, SeedProductDto>(
                        StringComparer.OrdinalIgnoreCase
                    );

                // Recognised image view suffixes

                var imageViews = new[]
                {
                    "front",
                    "back",
                    "side",
                    "detail",
                    "main"
                };

                // ---------------------------------
                // 5. PROCESS EACH IMAGE
                // ---------------------------------

                foreach (var file in imageFiles)
                {
                    var fileName =
                        Path.GetFileNameWithoutExtension(file);

                    if (string.IsNullOrWhiteSpace(fileName))
                        continue;

                    var parts = fileName.Split(
                        '-',
                        StringSplitOptions.RemoveEmptyEntries
                    );

                    if (parts.Length == 0)
                        continue;

                    // Determine product name and image view.

                    string productSlug;
                    string view;

                    var lastPart = parts.Last();

                    if (imageViews.Contains(
                        lastPart,
                        StringComparer.OrdinalIgnoreCase))
                    {
                        view = lastPart.ToLowerInvariant();

                        productSlug = string.Join(
                            "-",
                            parts.Take(parts.Length - 1)
                        );
                    }
                    else
                    {
                        // Keep the complete filename as the
                        // product name if no view suffix exists.

                        productSlug = fileName;
                        view = "front";
                    }

                    if (string.IsNullOrWhiteSpace(productSlug))
                        continue;

                    // ---------------------------------
                    // 6. GET RELATIVE IMAGE PATH
                    // ---------------------------------

                    var relativePath = Path
                        .GetRelativePath(
                            webRootPath,
                            file
                        )
                        .Replace("\\", "/");

                    // ---------------------------------
                    // 7. CREATE PRODUCT IF NECESSARY
                    // ---------------------------------

                    if (!products.ContainsKey(productSlug))
                    {
                        products[productSlug] =
                            new SeedProductDto
                            {
                                Name = ToTitle(productSlug),

                                Description =
                                    "Auto generated product",

                                ImageBasePath =
                                    GetBasePath(relativePath),

                                Variants =
                                    new List<SeedVariantDto>()
                            };
                    }

                    var product = products[productSlug];

                    // ---------------------------------
                    // 8. CREATE DEFAULT VARIANT
                    // ---------------------------------

                    if (!product.Variants.Any())
                    {
                        product.Variants.Add(
                            new SeedVariantDto
                            {
                                Color = "Default",

                                Size = "M",

                                Price = 0,

                                Stock = 0,

                                Images = new List<SeedImageDto>()
                            }
                        );
                    }

                    var variant = product.Variants.First();

                    // ---------------------------------
                    // 9. ADD PRODUCT IMAGE
                    // ---------------------------------

                    var image = new SeedImageDto
                    {
                        File = Path.GetFileName(file),

                        IsPrimary = view == "front" ||
                                    view == "main"
                    };

                    // Prevent duplicate image entries.

                    if (!variant.Images.Any(i =>
                        i.File.Equals(
                            image.File,
                            StringComparison.OrdinalIgnoreCase
                        )))
                    {
                        variant.Images.Add(image);
                    }
                }

                // ---------------------------------
                // 10. CHECK GENERATED PRODUCTS
                // ---------------------------------

                if (!products.Any())
                {
                    Console.WriteLine(
                        "No products were generated."
                    );

                    return;
                }

                // ---------------------------------
                // 11. CREATE SEEDDATA DIRECTORY
                // ---------------------------------

                var seedDirectory = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "SeedData"
                );

                if (!Directory.Exists(seedDirectory))
                {
                    Directory.CreateDirectory(seedDirectory);
                }

                // ---------------------------------
                // 12. GENERATE JSON FILE
                // ---------------------------------

                var outputPath = Path.Combine(
                    seedDirectory,
                    "products.json"
                );

                var json = JsonSerializer.Serialize(
                    products.Values.ToList(),
                    new JsonSerializerOptions
                    {
                        WriteIndented = true
                    }
                );

                File.WriteAllText(outputPath, json);

                // ---------------------------------
                // 13. SUCCESS MESSAGE
                // ---------------------------------

                Console.WriteLine(
                    "===================================="
                );

                Console.WriteLine(
                    "PRODUCT JSON GENERATION SUCCESSFUL"
                );

                Console.WriteLine(
                    $"Products generated: {products.Count}"
                );

                Console.WriteLine(
                    $"Images processed: {imageFiles.Count}"
                );

                Console.WriteLine(
                    $"Output location: {outputPath}"
                );

                Console.WriteLine(
                    "===================================="
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "An error occurred while generating product JSON:"
                );

                Console.WriteLine(ex.ToString());

                throw;
            }
        }

        // =========================================
        // GET IMAGE BASE PATH
        // =========================================

        private static string GetBasePath(string relativePath)
        {
            var directory = Path.GetDirectoryName(relativePath);

            if (string.IsNullOrWhiteSpace(directory))
                return "Images";

            return directory.Replace("\\", "/");
        }

        // =========================================
        // CONVERT PRODUCT SLUG TO TITLE
        // =========================================

        private static string ToTitle(string slug)
        {
            return System.Globalization.CultureInfo
                .CurrentCulture
                .TextInfo
                .ToTitleCase(
                    slug.Replace("-", " ")
                );
        }
    }
}