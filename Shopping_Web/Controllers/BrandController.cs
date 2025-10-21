using Microsoft.AspNetCore.Mvc;
using Shopping_Web.Repository;
using Shopping_Web.Models;
using Microsoft.EntityFrameworkCore;
namespace Shopping_Web.Controllers
{
    public class BrandController : Controller
    {
        private readonly DataContext _context;
        public BrandController(DataContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Brands(String BrandName)
        {
            Brands brands = _context.Brand.Where(b => b.BrandName == BrandName).FirstOrDefault();
            if(brands == null)
            {
                return RedirectToAction("BrandsView");
            }
            var ProductBybrands = _context.Product.Where(p => p.BrandId == brands.BrandId).ToListAsync();
            return View(await ProductBybrands);
        }
    }
}
