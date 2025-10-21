using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping_Web.Models;
using Shopping_Web.Repository;
namespace Shopping_Web.Controllers
{
    public class CategoryController : Controller
    {
        private readonly DataContext _context;
        public CategoryController(DataContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Category(String CategoryName)
        {
            Categories categories = _context.Categories.Where(c => c.CategoryName == CategoryName).FirstOrDefault();
            if(categories == null)
            {
                return RedirectToAction("CategoriesView");
            }
            var productByCategories = _context.Product.Where(p => p.CategoryId == categories.CategoryId);
            return View(await productByCategories.OrderByDescending(p => p.ProductId).ToListAsync());
        }
    }
}
