using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;

namespace SHEIN_Clone_MVC.Helpers
{
    public static class WishlistHelper
    {
        private const string WishlistSessionKey = "Wishlist";

        public static List<int> GetWishlist(HttpContext context)
        {
            return context.Session
                .GetObjectFromJson<List<int>>(WishlistSessionKey)
                ?? new List<int>();
        }

        public static void SaveWishlist(
            HttpContext context,
            List<int> wishlist)
        {
            context.Session.SetObjectAsJson(
                WishlistSessionKey,
                wishlist);
        }

        public static void ToggleWishlist(
            HttpContext context,
            int productId)
        {
            var wishlist = GetWishlist(context);

            if (wishlist.Contains(productId))
            {
                wishlist.Remove(productId);
            }
            else
            {
                wishlist.Add(productId);
            }

            SaveWishlist(context, wishlist);
        }

        public static bool IsInWishlist(
            HttpContext context,
            int productId)
        {
            var wishlist = GetWishlist(context);

            return wishlist.Contains(productId);
        }
    }
}