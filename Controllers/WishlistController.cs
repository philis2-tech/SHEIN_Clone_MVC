using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SHEIN_Clone_MVC.Data;
using SHEIN_Clone_MVC.Helpers;
using System.Linq;
using System.Threading.Tasks;

namespace SHEIN_Clone_MVC.Controllers
{
    public class WishlistController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WishlistController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ===========================
        // DISPLAY WISHLIST
        // ===========================
        public async Task<IActionResult> Index()
        {
            var wishlistIds = WishlistHelper.GetWishlist(HttpContext);

            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductVariants)
                    .ThenInclude(v => v.ProductImages)
                .Where(p => wishlistIds.Contains(p.Id))
                .ToListAsync();

            return View(products);
        }

        // ===========================
        // ADD / REMOVE FROM WISHLIST
        // ===========================
        public IActionResult Toggle(int id)
        {
            WishlistHelper.ToggleWishlist(HttpContext, id);

            var referer = Request.Headers["Referer"].ToString();

            if (string.IsNullOrEmpty(referer))
            {
                return RedirectToAction("Index", "Products");
            }

            return Redirect(referer);
        }

        // ===========================
        // REMOVE FROM WISHLIST
        // ===========================
        public IActionResult Remove(int id)
        {
            var wishlist = WishlistHelper.GetWishlist(HttpContext);

            if (wishlist.Contains(id))
            {
                wishlist.Remove(id);
                WishlistHelper.SaveWishlist(HttpContext, wishlist);
            }

            return RedirectToAction(nameof(Index));
        }

        // ===========================
        // MOVE PRODUCT TO CART
        // ===========================
        public async Task<IActionResult> MoveToCart(int id)
        {
            var product = await _context.Products
                .Include(p => p.ProductVariants)
                    .ThenInclude(v => v.ProductImages)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            var firstVariant = product.ProductVariants.FirstOrDefault();

            var firstImage = firstVariant?
                .ProductImages
                .FirstOrDefault()?
                .ImagePath ?? "/Images/no-image.png";

            CartHelper.AddToCart(
                HttpContext,
                product.Id,
                product.Name,
                firstVariant?.Price ?? 0,
                firstImage
            );

            var wishlist = WishlistHelper.GetWishlist(HttpContext);

            if (wishlist.Contains(id))
            {
                wishlist.Remove(id);
                WishlistHelper.SaveWishlist(HttpContext, wishlist);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}