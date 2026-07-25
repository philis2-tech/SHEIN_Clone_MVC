using SHEIN_Clone_MVC.Models.Seed;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace SHEIN_Clone_MVC.Helpers
{
    public static class ImageSyncService
    {
        public static List<SeedProductDto> GenerateProductsFromImages(
            string webRootPath)
        {
            var products =
                new Dictionary<string, SeedProductDto>();

            var imagesPath =
                Path.Combine(webRootPath, "Images");

            if (!Directory.Exists(imagesPath))
            {
                return new List<SeedProductDto>();
            }

            var imageFiles =
                Directory.GetFiles(
                    imagesPath,
                    "*.png",
                    SearchOption.AllDirectories);

            Console.WriteLine(
                $"Images folder: {imagesPath}");

            Console.WriteLine(
                $"Found {imageFiles.Length} PNG files");

            foreach (var file in imageFiles)
            {
                var fileName =
                    Path.GetFileName(file);

                var cleanName =
                    Path.GetFileNameWithoutExtension(file)
                    .ToLower()
                    .Replace("_", "-")
                    .Replace("'", "")
                    .Trim();

                if (string.IsNullOrWhiteSpace(cleanName))
                {
                    continue;
                }

                var parts =
                    cleanName.Split(
                        '-',
                        StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length < 2)
                {
                    continue;
                }

                string[] imageTags =
                {
                    "front",
                    "back",
                    "side",
                    "side1",
                    "side2",
                    "side3",
                    "side4",
                    "side5",

                    "detail",
                    "detail1",
                    "detail2",
                    "detail3",

                    "closeup",
                    "zoom",
                    "view",
                    "main",
                    "model",
                    "look",

                    "1",
                    "2",
                    "3",
                    "4",
                    "5",
                    "6",
                    "7",
                    "8",
                    "9",
                    "10"
                };

                int endIndex =
                    parts.Length;

                while (
                    endIndex > 0 &&
                    imageTags.Contains(
                        parts[endIndex - 1]))
                {
                    endIndex--;
                }

                var productKey =
                    string.Join(
                        "-",
                        parts.Take(endIndex));

                if (string.IsNullOrWhiteSpace(productKey))
                {
                    productKey = cleanName;
                }

                var productName =
                    productKey
                    .Replace("-", " ")
                    .Trim();

                var relativePath =
                    file
                    .Replace(webRootPath, "")
                    .Replace("\\", "/");

                var pathParts =
                    relativePath.Split(
                        '/',
                        StringSplitOptions.RemoveEmptyEntries);

                var imagesIndex =
                    Array.IndexOf(
                        pathParts,
                        "Images");

                if (
                    imagesIndex < 0 ||
                    imagesIndex + 1 >=
                    pathParts.Length)
                {
                    continue;
                }

                var folder =
                    pathParts[imagesIndex + 1];

                var imageBasePath =
                    $"/Images/{folder}";

                if (!products.ContainsKey(productKey))
                {
                    products[productKey] =
                        new SeedProductDto
                        {
                            Name =
                                CapitalizeWords(
                                    productName),

                            Description =
                                "Fashion product",

                            ImageBasePath =
                                imageBasePath,

                            Variants =
                                new List<SeedVariantDto>()
                        };
                }

                var product =
                    products[productKey];

                // --------------------------------
                // CREATE MULTIPLE SIZES
                // --------------------------------

                string[] sizes =
                {
                    "XS",
                    "S",
                    "M",
                    "L",
                    "XL",
                    "XXL"
                };

                foreach (var size in sizes)
                {
                    var variant =
                        product.Variants
                        .FirstOrDefault(
                            v => v.Size == size);

                    if (variant == null)
                    {
                        variant =
                            new SeedVariantDto
                            {
                                Color =
                                    "Default",

                                Size =
                                    size,

                                Price =
                                    GeneratePrice(
                                        productKey),

                                Stock = 10,

                                Images =
                                    new List<SeedImageDto>()
                            };

                        product.Variants.Add(
                            variant);
                    }

                    if (
                        !variant.Images.Any(
                            i => i.File.Equals(
                                fileName,
                                StringComparison
                                    .OrdinalIgnoreCase)))
                    {
                        variant.Images.Add(
                            new SeedImageDto
                            {
                                File =
                                    fileName,

                                IsPrimary =
                                    !variant.Images.Any()
                            });
                    }
                }
            }

            Console.WriteLine(
                $"Generated {products.Count} unique products");

            return products.Values.ToList();
        }

        private static string CapitalizeWords(
            string input)
        {
            return string.Join(
                " ",

                input
                .Split(' ')
                .Where(
                    x =>
                    !string.IsNullOrWhiteSpace(x))
                .Select(
                    w =>
                        char.ToUpper(w[0]) +
                        w.Substring(1))
            );
        }

        private static decimal GeneratePrice(
            string productKey)
        {
            return Math.Abs(
                productKey.GetHashCode()
            ) % 700 + 100;
        }
    }
}
