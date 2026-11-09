using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SHEIN_Clone_MVC.Data;

namespace SHEIN_Clone_MVC.ViewComponents
{
    public class CategoryMegaMenuViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;


    public CategoryMegaMenuViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await _context.Categories
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .ToListAsync();

            var products = await _context.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.ProductVariants)
                    .ThenInclude(v => v.ProductImages)
                .OrderByDescending(p => p.Id)
                .Take(16)
                .ToListAsync();

            ViewBag.Categories = categories;

            return View(products);
        }
    }


}
