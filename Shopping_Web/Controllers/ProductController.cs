using Microsoft.AspNetCore.Mvc;
using Shopping_Web.Repository;
using Shopping_Web.Models;
using Microsoft.EntityFrameworkCore;
using Shopping_Web.Models.ViewModels;
namespace Shopping_Web.Controllers
{
    public class ProductController : Controller 
    {
        private readonly DataContext _context;
        public ProductController(DataContext context)
        {
            _context = context;
        }
        [HttpPost]
        public async Task<IActionResult> Search(string searchProduct)
        {
            if(string.IsNullOrEmpty(searchProduct))
            {
                return RedirectToAction("Home", "Home");
            }
            var products = await _context.Product
                .Where(p => p.ProductName.Contains(searchProduct) || p.Description.Contains(searchProduct))
                .ToArrayAsync();
            ViewBag.keyword = searchProduct;
            return View(products);
        }
       
        public async Task<IActionResult> ProductDetail(int ProductId)
        {
            if(ProductId <= 0)
            {
                return RedirectToAction("Home");
            }
            var product = await _context.Product.Where(p => p.ProductId == ProductId)
                .Include(p => p.Ratings)
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .FirstOrDefaultAsync();
            if (product == null)
            {
                return RedirectToAction("Home");
            }
            var relatedProducts = await _context.Product
                .Where(p => p.CategoryId == product.CategoryId && ProductId != p.ProductId)
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Take(4)
                .ToListAsync();
            ViewBag.RelatedProducts = relatedProducts;

            var ModelView = new ProductDetailsViewModels
            {       
                ProductDetails = product,
            };
            return View(ModelView);
        }
        public IActionResult Product()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>Comment(Rating rating)
        {
            if (ModelState.IsValid)
            {
                var RatingNew = new Rating
                {
                    ProductId = rating.ProductId,
                    UserName = rating.UserName,
                    Email = rating.Email,
                    Score = rating.Score,
                    Review = rating.Review,
                    CreatedAt = DateTime.Now
                };
                _context.Ratings.Add(RatingNew);
                await _context.SaveChangesAsync();
                TempData["success"] = "Comment Added Successfully";
                return Redirect(Request.Headers["Referer"].ToString());
            }
            else
            {
                TempData["error"] = "Models have a error";
                List<String> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                String errorMessgae = String.Join("\n", errors);
                return RedirectToAction("ProductDetail", new { id = rating.ProductId });
            }
        }
    }
}
