using Microsoft.AspNetCore.Mvc;
using Shopping_Web.Repository;
using Shopping_Web.Models;
namespace Shopping_Web.Controllers
{
    public class ProductController : Controller 
    {
        private readonly DataContext _context;
        public ProductController(DataContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> ProductDetail(int ProductId)
        {
            if(ProductId <= 0)
            {
                return RedirectToAction("Home");
            }
            Product product = _context.Product.Where(p => p.ProductId == ProductId).FirstOrDefault();
            if (product == null)
            {
                return RedirectToAction("Home");
            }
            return View(product);
        }
        public IActionResult Product()
        {
            return View();
        }
    }
}
