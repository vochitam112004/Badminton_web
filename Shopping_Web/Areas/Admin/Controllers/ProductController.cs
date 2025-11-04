using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shopping_Web.Models;
using Shopping_Web.Repository;

namespace Shopping_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class ProductController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(DataContext dataContext , IWebHostEnvironment webHostEnvironment)
        {
            _dataContext = dataContext;
            _webHostEnvironment = webHostEnvironment;
        }
 
        public async Task<IActionResult> Product(int page =1)
        {
            List<Product> products = await _dataContext.Product
                .Include(p =>p.Brand)
                .Include(p => p.Category)
                .OrderByDescending(p => p.ProductId)
                .ToListAsync();
            const int pageSize = 10;
            if(page < 1)
            {
                page = 1;
            }
            int TotalItems = products.Count();
            var paginate = new Paginate(TotalItems, page, pageSize);
            int currentPage = (page - 1) * pageSize;
            var data = products.Skip(currentPage).Take(paginate.PageSize).ToList();
            ViewBag.Page = paginate;
            return View(data);
        }
        [HttpGet]
        public IActionResult Create() {
            ViewBag.Category = new SelectList(_dataContext.Categories, "CategoryId", "CategoryName");
            ViewBag.Brand = new SelectList(_dataContext.Brand, "BrandId", "BrandName");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            ViewBag.Category = new SelectList(_dataContext.Categories, "CategoryId", "CategoryName" ,product.CategoryId);
            ViewBag.Brand = new SelectList(_dataContext.Brand, "BrandId", "BrandName" , product.BrandId);
            if (ModelState.IsValid)
            {
                var ProductExists = await _dataContext.Product.FirstOrDefaultAsync(p => p.ProductName == product.ProductName);
                if(ProductExists != null)
                {
                    ModelState.AddModelError("","Product Name existed in Database");
                    return View(product);
                }
               
                if(product.ImageUpload != null)
                {
                    String uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/Products");
                    String imageName = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;
                    String filePath = Path.Combine(uploadsDir, imageName);

                    FileStream fs = new FileStream(filePath , FileMode.Create);
                    await product.ImageUpload.CopyToAsync(fs);
                    fs.Close();
                    product.ImageUrl = imageName;
                }
               
                _dataContext.Add(product);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Create Product Success";
                return RedirectToAction("Product");
            }
            else
            {
                TempData["error"] = "Models have a error";
                List<String> errors = new List<string>();
                foreach(var value in ModelState.Values)
                {
                    foreach(var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                String errorMessgae = String.Join("\n", errors);
                return BadRequest(errorMessgae);
            }
        }
        public async Task<IActionResult> Edit(int ProductId)
        {
            var product = await _dataContext.Product.FirstOrDefaultAsync(p => p.ProductId == ProductId);
            ViewBag.Category = new SelectList(_dataContext.Categories, "CategoryId", "CategoryName", product.CategoryId);
            ViewBag.Brand = new SelectList(_dataContext.Brand, "BrandId", "BrandName", product.BrandId);
            return View(product);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int ProductId ,Product product)
        {
            ViewBag.Category = new SelectList(_dataContext.Categories, "CategoryId", "CategoryName", product.CategoryId);
            ViewBag.Brand = new SelectList(_dataContext.Brand, "BrandId", "BrandName", product.BrandId);
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                     .Select(e => e.ErrorMessage)
                                     .ToList();
                TempData["error"] = string.Join(" ", errors);
                return View(product);
            }
            var duplicate = await _dataContext.Product.FirstOrDefaultAsync( p => p.ProductName == product.ProductName && p.ProductId != ProductId);
            if(duplicate != null)
            {
                ModelState.AddModelError("", "Product Name already exist");
                return View(product);
            }
            var productExisted = await _dataContext.Product.FindAsync(ProductId);
            if(productExisted == null)
            {
                return NotFound("Product not Found");
            }
            productExisted.ProductName = product.ProductName;
            productExisted.ProductCode = product.ProductCode;
            productExisted.Price =product.Price;
            productExisted.Description = product.Description;
            productExisted.CategoryId = product.CategoryId;
            productExisted.BrandId = product.BrandId;
            productExisted.Quantity = product.Quantity;
            productExisted.Size = product.Size;
            
            if(product.ImageUpload != null)
            {
                String uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/Products");
                String imageName = Guid.NewGuid().ToString() +"_" + product.ImageUpload.FileName;
                String filePath = Path.Combine(uploadDir, imageName);

                using(var fs = new FileStream(filePath, FileMode.Create))
                {
                    await product.ImageUpload.CopyToAsync(fs);
                }
                if (!string.IsNullOrEmpty(productExisted.ImageUrl))
                {
                    String oldPath = Path.Combine(uploadDir , productExisted.ImageUrl);
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }
                productExisted.ImageUrl = imageName;
            }
            _dataContext.Update(productExisted);
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Update Product success";
            return RedirectToAction("Product");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>Delete(int ProductId)
        {
            var product = await _dataContext.Product.FindAsync(ProductId);
            if(product != null && !string.IsNullOrEmpty(product.ImageUrl))
            {
                String imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "media/Products" ,product.ImageUrl);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO .File.Delete(imagePath);
                }
            }
            _dataContext.Remove(product);
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Product deleted successfully";
            return RedirectToAction("Product");
        }
    }
}
