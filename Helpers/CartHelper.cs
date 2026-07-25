using Microsoft.AspNetCore.Http;
using SHEIN_Clone_MVC.Models;
using System.Collections.Generic;
using System.Linq;

namespace SHEIN_Clone_MVC.Helpers
{
    public static class CartHelper
    {
        private const string CartKey = "Cart";

        // ===========================
        // GET CART
        // ===========================
        public static List<CartItem> GetCart(HttpContext context)
        {
            return context.Session
                .GetObjectFromJson<List<CartItem>>(CartKey)
                ?? new List<CartItem>();
        }

        // ===========================
        // SAVE CART
        // ===========================
        public static void SaveCart(HttpContext context, List<CartItem> cart)
        {
            context.Session.SetObjectAsJson(CartKey, cart);
        }

        // ===========================
        // ADD PRODUCT TO CART
        // ===========================
        public static void AddToCart(
            HttpContext context,
            int productId,
            string productName,
            decimal price,
            string imageUrl)
        {
            var cart = GetCart(context);

            var item = cart.FirstOrDefault(x => x.ProductId == productId);

            if (item != null)
            {
                item.Quantity++;
            }
            else
            {
                cart.Add(new CartItem
                {
                    ProductId = productId,
                    ProductName = productName,
                    Price = price,
                    Quantity = 1,
                    ImageUrl = imageUrl
                });
            }

            SaveCart(context, cart);
        }

        // ===========================
        // REMOVE PRODUCT
        // ===========================
        public static void RemoveFromCart(HttpContext context, int productId)
        {
            var cart = GetCart(context);

            var item = cart.FirstOrDefault(x => x.ProductId == productId);

            if (item != null)
            {
                cart.Remove(item);
            }

            SaveCart(context, cart);
        }

        // ===========================
        // CLEAR CART
        // ===========================
        public static void ClearCart(HttpContext context)
        {
            SaveCart(context, new List<CartItem>());
        }

        // ===========================
        // CART COUNT
        // ===========================
        public static int GetCartCount(HttpContext context)
        {
            return GetCart(context).Sum(x => x.Quantity);
        }
    }
}