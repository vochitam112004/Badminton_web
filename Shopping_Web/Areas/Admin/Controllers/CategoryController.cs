using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping_Web.Models;
using Shopping_Web.Repository;

namespace Shopping_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly DataContext _dataContext;
        public CategoryController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        //public async Task<IActionResult> Category() 
        //{
        //    return View(await _dataContext.Categories.OrderByDescending(c => c.CategoryId).ToListAsync());
             
        //}
        public async Task<IActionResult> Category(int page = 1)
        {
            List<Categories> categories = await _dataContext.Categories.ToListAsync(); // lay data
            const int pageSize = 10;
            if(page < 1)
            {
                page = 1;
            }
            int totalItems = categories.Count(); // dem so luong item
            var paginate = new Paginate(totalItems, page, pageSize);
            int currentPage = (page - 1) * pageSize;
            var data = categories.Skip(currentPage).Take(paginate.PageSize).ToList();
            ViewBag.Page= paginate;
            return View(data);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Categories categories)
        {
            if (ModelState.IsValid)
            {
                var categoryName = await _dataContext.Categories.FirstOrDefaultAsync(c => c.CategoryName == categories.CategoryName);
                if (categoryName != null)
                {
                    ModelState.AddModelError("", "category Name existed in Database");
                    return View(categories);
                }
                _dataContext.Add(categories);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Category Created Successfully";
                return RedirectToAction("Category");
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
        public async  Task<IActionResult> Edit(int CategoryId)
        {
            Categories category = await _dataContext.Categories.FindAsync(CategoryId);
            return View(category);
           
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int CategoryId , Categories categories)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var category = await _dataContext.Categories.FirstOrDefaultAsync(c => c.CategoryName == categories.CategoryName && c.CategoryId != categories.CategoryId);
            if(category != null)
            {
                ModelState.AddModelError("", "Category Name existed in Database");
                return View(categories);
            }
            var categoriExisted = await _dataContext.Categories.FindAsync(CategoryId);
            if(categoriExisted == null)
            {
                return NotFound("Category Not Found");
            }
            categoriExisted.CategoryName = categories.CategoryName;
            categoriExisted.Description = categories.Description;
            _dataContext.Update(categoriExisted);
            await _dataContext.SaveChangesAsync();
            TempData["EditCategorySucess"] = "Update Category success";
            return RedirectToAction("Category");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int CategoryId)
        {
            var category = await _dataContext.Categories.FindAsync(CategoryId);
            if(category == null)
            {
                return NotFound("Category Not Found");
            }
            _dataContext.Categories.Remove(category);
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Category Deleted Successfully";
            return RedirectToAction("Category");
        }
    }
}
