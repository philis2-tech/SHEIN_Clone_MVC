using System;
using System.Collections.Generic;
using System.Linq;
using SHEIN_Clone_MVC.Models;
using SHEIN_Clone_MVC.Data;

namespace SHEIN_Clone_MVC.Helpers
{
    public static class CategoryResolver
    {
        private static readonly Dictionary<string, string> Rules = new()
        {
            { "/images/women/", "Women" },
            { "/images/men/", "Men" },
            { "/images/kids/", "Kids" },
            { "/images/shoes/", "Shoes" },

            { "jewellery", "Jewellery & Accessories" },
            { "accessories", "Jewellery & Accessories" },

            { "bags", "Bags & Luggages" },
            { "luggages", "Bags & Luggages" },

            { "beachwear", "Beachwear" },
            { "nightwear", "Nightwear" },
            { "activewear", "Activewear" },

            { "toys", "Toys" },
            { "appliances", "Appliances" }
        };

        private static Dictionary<string, int> _categoryCache =
            new Dictionary<string, int>();

        public static void LoadCategories(ApplicationDbContext context)
        {
            _categoryCache = context.Categories
                .ToDictionary(
                    c => c.Name,
                    c => c.Id
                );
        }

        public static int GetCategoryId(string path)
        {
            path = path.ToLower();

            var match = Rules.FirstOrDefault(
                r => path.Contains(r.Key)
            );

            if (match.Key == null)
            {
                throw new Exception(
                    $"Unknown category for path: {path}"
                );
            }

            if (!_categoryCache.ContainsKey(match.Value))
            {
                throw new Exception(
                    $"Category '{match.Value}' not found in database."
                );
            }

            return _categoryCache[match.Value];
        }
    }
}