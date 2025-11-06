using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping_Web.Models;
using Shopping_Web.Repository;

namespace Shopping_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Slider")]
    public class SliderController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public SliderController(DataContext dataContext, IWebHostEnvironment webHostEnvironment)
        {
            _dataContext = dataContext;
            _webHostEnvironment = webHostEnvironment;
        }
        [Route("Slider")]
        public async Task<IActionResult> Slider()
        {
            var sliders = await _dataContext.Sliders.OrderByDescending(s => s.SliderId).ToListAsync();
            return View(sliders);
        }
        [Route("Create")]
        public IActionResult Create()
        {
            return View();
        }
        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Slider sliders)
        {
            if (ModelState.IsValid)
            {
                var sliderExists = await _dataContext.Sliders.FirstOrDefaultAsync(s => s.Name == sliders.Name);
                if (sliderExists != null)
                {
                    ModelState.AddModelError(" ", "Slider Name existed in Database");
                    return View(sliders);
                }
                if (sliders.ImageFile != null)
                {
                    string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/Slider");
                    string imageName = Guid.NewGuid().ToString() + "_" + sliders.ImageFile.FileName;
                    string filePath = Path.Combine(uploadsDir, imageName);

                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await sliders.ImageFile.CopyToAsync(fs);
                    fs.Close();
                    sliders.ImageUrl = imageName;
                }
                _dataContext.Sliders.Add(sliders);
                await _dataContext.SaveChangesAsync();
                TempData["Success"] = "Create Slider Success";
                return RedirectToAction("Slider");
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
        [Route("Edit")]
        public async Task<IActionResult> Edit(int SliderId)
        {
            var sliderById = await _dataContext.Sliders.FirstOrDefaultAsync(s => s.SliderId == SliderId);
            if (sliderById == null)
            {
                return RedirectToAction("Slider", "Slider not exited");
            }
            return View(sliderById);
        }
        [Route("Edit")]
        [HttpPost]
        public async Task<IActionResult> Edit(int SliderId, Slider sliders)
        {
            var sliderById = await _dataContext.Sliders.FindAsync(SliderId);
            if (sliderById == null)
            {
                return RedirectToAction("Slider", "Slider not exited");
            }
            sliderById.Name = sliders.Name;
            sliderById.Description = sliders.Description;
            sliderById.Status = sliders.Status;
            if (sliders.ImageFile != null)
            {
                string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/Slider");
                string imageName = Guid.NewGuid().ToString() + "_" + sliders.ImageFile.FileName;
                string filePath = Path.Combine(uploadsDir, imageName);

                using (var fs = new FileStream(filePath, FileMode.Create))
                {
                    await sliders.ImageFile.CopyToAsync(fs);
                }
                if (!string.IsNullOrEmpty(sliderById.ImageUrl))
                {
                    string oldPath = Path.Combine(uploadsDir, sliderById.ImageUrl);
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }
                sliderById.ImageUrl = imageName;
            }
            _dataContext.Sliders.Update(sliderById);
            await _dataContext.SaveChangesAsync();
            TempData["Success"] = "Update Slider Success";
            return RedirectToAction("Slider");
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int SliderId)
        {
            var sliderExites = await _dataContext.Sliders.FindAsync(SliderId);
            if (sliderExites != null && !string.IsNullOrEmpty(sliderExites.ImageUrl))
            {
                string pathImage = Path.Combine(_webHostEnvironment.WebRootPath, "media/Slider", sliderExites.ImageUrl);
                if (System.IO.File.Exists(pathImage))
                {
                    System.IO.File.Delete(pathImage);
                }
            }
            _dataContext.Sliders.Remove(sliderExites);
            await _dataContext.SaveChangesAsync();
            TempData["Success"] = "Delete Slider Success";
            return RedirectToAction("Slider");
        }
    }
}
