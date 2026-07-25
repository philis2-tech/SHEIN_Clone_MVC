using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SHEIN_Clone_MVC.Data;
using SHEIN_Clone_MVC.Helpers;
using SHEIN_Clone_MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SHEIN_Clone_MVC.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================================
        // GET: Products
        // =========================================
        public async Task<IActionResult> Index(
            string searchString,
            int? categoryId)
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentCategory"] = categoryId;

            // categories dropdown
            ViewBag.Categories =
                await _context.Categories.ToListAsync();

            var products = _context.Products

                // category
                .Include(p => p.Category)

                // variants + images
                .Include(p => p.ProductVariants)
                    .ThenInclude(v => v.ProductImages)

                .AsQueryable();

            // ---------------------------------
            // SEARCH
            // ---------------------------------
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();

                products = products.Where(p =>
                    p.Name.ToLower().Contains(searchString) ||

                    p.Description.ToLower().Contains(searchString) ||

                    p.Category.Name.ToLower().Contains(searchString)
                );
            }

            // ---------------------------------
            // CATEGORY FILTER
            // ---------------------------------
            if (categoryId.HasValue)
            {
                products = products.Where(p =>
                    p.CategoryId == categoryId.Value
                );
            }

            return View(await products.ToListAsync());
        }

        // =========================================
        // GET: Products/Details/5
        // =========================================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var product = await _context.Products

                .Include(p => p.Category)

                .Include(p => p.ProductVariants)
                    .ThenInclude(v => v.ProductImages)

                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null)
                return NotFound();

            return View(product);
        }

        // =========================================
        // GET: Products/Create
        // =========================================
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(
                _context.Categories,
                "Id",
                "Name"
            );

            return View();
        }

        // =========================================
        // POST: Products/Create
        // =========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(
                _context.Categories,
                "Id",
                "Name",
                product.CategoryId
            );

            return View(product);
        }

        // =========================================
        // GET: Products/Edit/5
        // =========================================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var product = await _context.Products.FindAsync(id);

            if (product == null)
                return NotFound();

            ViewData["CategoryId"] = new SelectList(
                _context.Categories,
                "Id",
                "Name",
                product.CategoryId
            );

            return View(product);
        }

        // =========================================
        // POST: Products/Edit/5
        // =========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            Product product)
        {
            if (id != product.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                        return NotFound();

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(
                _context.Categories,
                "Id",
                "Name",
                product.CategoryId
            );

            return View(product);
        }

        // =========================================
        // GET: Products/Delete/5
        // =========================================
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var product = await _context.Products

                .Include(p => p.Category)

                .Include(p => p.ProductVariants)
                    .ThenInclude(v => v.ProductImages)

                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null)
                return NotFound();

            return View(product);
        }

        // =========================================
        // POST: Products/Delete/5
        // =========================================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product =
                await _context.Products.FindAsync(id);

            if (product != null)
            {
                _context.Products.Remove(product);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
        // ===========================
        // TOGGLE WISHLIST
        // ===========================
        public IActionResult Toggle(int id)
        {
            WishlistHelper.ToggleWishlist(
                HttpContext,
                id);

            return RedirectToAction(
                "Index",
                "Products");
        }

        // =========================================
        // EXISTS CHECK
        // =========================================
        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
