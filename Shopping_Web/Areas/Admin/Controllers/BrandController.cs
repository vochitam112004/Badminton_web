using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping_Web.Models;
using Shopping_Web.Repository;

namespace Shopping_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class BrandController : Controller
    {
        private readonly DataContext _dataContext;
        public BrandController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<IActionResult> Brand()
        {
            return View(await _dataContext.Brand.OrderByDescending(b => b.BrandId).ToListAsync());
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Brands brands)
        {
            if (ModelState.IsValid)
            {
                var brandName = await _dataContext.Brand.FirstOrDefaultAsync(b => b.BrandName == brands.BrandName);
                if (brandName != null)
                {
                    ModelState.AddModelError("", "Brand Name existed in Database");
                    return View(brands);
                }
                _dataContext.Add(brands);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Brand Created Successfully";
                return RedirectToAction("Brand");
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
                return BadRequest(errorMessgae);
            }
        }
        [HttpGet]
        public async Task<IActionResult>Edit(int BrandId)
        {
            Brands brands = await _dataContext.Brand.FindAsync(BrandId);
            return View(brands);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int BrandId , Brands brands)
        {
            if(!ModelState.IsValid)
            {
                return View(brands);
            }
            var brandName = await _dataContext.Brand.FirstOrDefaultAsync(b => b.BrandName == brands.BrandName && b.BrandId != brands.BrandId);
            if (brandName != null)
            {
                ModelState.AddModelError("", "Brand Name existed in Database");
                return View(brands);
            }
            var brand = await _dataContext.Brand.FindAsync(BrandId);
            if(brand == null)
            {
                return NotFound("Not found brand");
            }
            brand.BrandName = brands.BrandName;
            brand.Description = brands.Description;
            _dataContext.Update(brand);
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Brand Updated Successfully";
            return RedirectToAction("Brand");
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int BrandId)
        {
            var brands = await _dataContext.Brand.FindAsync(BrandId);
            if(brands == null)
            {
                return NotFound("Not found brand");
            }
            _dataContext.Brand.Remove(brands);
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Brand Deleted Successfully";
            return RedirectToAction("Brand");
        }
    }
}
