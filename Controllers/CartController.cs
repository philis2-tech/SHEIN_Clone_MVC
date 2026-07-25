using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SHEIN_Clone_MVC.Data;
using SHEIN_Clone_MVC.Helpers;
using SHEIN_Clone_MVC.Models;

namespace SHEIN_Clone_MVC.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }


        // =====================================================
        // ADD PRODUCT TO CART
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddToCart(
            int productId,
            string? size,
            string? color)
        {
            var product = _context.Products
                .Include(p => p.ProductVariants)
                    .ThenInclude(v => v.ProductImages)
                .FirstOrDefault(p => p.Id == productId);


            if (product == null)
            {
                return NotFound();
            }


            // Find the selected variant
            var variant = product.ProductVariants
                .FirstOrDefault(v =>
                    (string.IsNullOrEmpty(size) ||
                     v.Size == size)

                    &&

                    (string.IsNullOrEmpty(color) ||
                     v.Color == color)
                );


            // If no exact variant is found,
            // use the first available variant
            if (variant == null)
            {
                variant = product.ProductVariants
                    .FirstOrDefault();
            }


            if (variant == null)
            {
                return RedirectToAction(
                    "Details",
                    "Products",
                    new { id = productId }
                );
            }


            // Get cart from session
            var cart = HttpContext.Session
                .GetObjectFromJson<List<CartItem>>("Cart")
                ?? new List<CartItem>();


            // Get the main image
            var imageUrl = variant.ProductImages?
                .FirstOrDefault(i => i.IsPrimary)?
                .ImagePath

                ??

                variant.ProductImages?
                .FirstOrDefault()?
                .ImagePath

                ??

                string.Empty;


            // Check whether the exact same
            // product + size + colour is already in cart
            var existingItem = cart.FirstOrDefault(x =>
                x.ProductId == product.Id
                &&
                x.Size == variant.Size
                &&
                x.Color == variant.Color
            );


            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                cart.Add(new CartItem
                {
                    ProductId = product.Id,

                    ProductName = product.Name,

                    Price = variant.Price,

                    Quantity = 1,

                    ImageUrl = imageUrl,

                    Size = variant.Size,

                    Color = variant.Color
                });
            }


            // Save cart back into session
            HttpContext.Session.SetObjectAsJson(
                "Cart",
                cart
            );


            return RedirectToAction(nameof(Index));
        }


        // =====================================================
        // REMOVE ITEM FROM CART
        // =====================================================

        public IActionResult RemoveFromCart(
            int id,
            string? size,
            string? color)
        {
            var cart = HttpContext.Session
                .GetObjectFromJson<List<CartItem>>("Cart")
                ?? new List<CartItem>();


            var item = cart.FirstOrDefault(x =>
                x.ProductId == id
                &&
                x.Size == size
                &&
                x.Color == color
            );


            if (item != null)
            {
                cart.Remove(item);
            }


            HttpContext.Session.SetObjectAsJson(
                "Cart",
                cart
            );


            return RedirectToAction(nameof(Index));
        }


        // =====================================================
        // INCREASE QUANTITY
        // =====================================================

        public IActionResult IncreaseQuantity(
            int id,
            string? size,
            string? color)
        {
            var cart = HttpContext.Session
                .GetObjectFromJson<List<CartItem>>("Cart")
                ?? new List<CartItem>();


            var item = cart.FirstOrDefault(x =>
                x.ProductId == id
                &&
                x.Size == size
                &&
                x.Color == color
            );


            if (item != null)
            {
                item.Quantity++;
            }


            HttpContext.Session.SetObjectAsJson(
                "Cart",
                cart
            );


            return RedirectToAction(nameof(Index));
        }


        // =====================================================
        // DECREASE QUANTITY
        // =====================================================

        public IActionResult DecreaseQuantity(
            int id,
            string? size,
            string? color)
        {
            var cart = HttpContext.Session
                .GetObjectFromJson<List<CartItem>>("Cart")
                ?? new List<CartItem>();


            var item = cart.FirstOrDefault(x =>
                x.ProductId == id
                &&
                x.Size == size
                &&
                x.Color == color
            );


            if (item != null)
            {
                item.Quantity--;


                if (item.Quantity <= 0)
                {
                    cart.Remove(item);
                }
            }


            HttpContext.Session.SetObjectAsJson(
                "Cart",
                cart
            );


            return RedirectToAction(nameof(Index));
        }


        // =====================================================
        // DISPLAY CART
        // =====================================================

        public IActionResult Index()
        {
            var cart = HttpContext.Session
                .GetObjectFromJson<List<CartItem>>("Cart")
                ?? new List<CartItem>();


            return View(cart);
        }
    }
}