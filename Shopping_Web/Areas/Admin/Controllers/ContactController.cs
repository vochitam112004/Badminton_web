using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping_Web.Models;
using Shopping_Web.Repository;

namespace Shopping_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Contact")]
    public class ContactController : Controller
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ContactController(DataContext context , IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpGet]
        public async Task<IActionResult> Contact()
        {
            return View(await _context.Contacts.OrderByDescending(c => c.ContactId).ToListAsync());
        }
        [HttpGet("Edit")]
        public async Task<IActionResult> Edit(int ContactId)
        {
            return View( await _context.Contacts.FirstOrDefaultAsync(c => c.ContactId == ContactId));
        }
        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(int ContactId , Contact  contact)
        {
            if (ModelState.IsValid)
            {
                var contactExisted = await _context.Contacts.FindAsync(ContactId);
                if(contactExisted == null)
                {
                    return RedirectToAction("Contact" , "Contact");
                }
                contactExisted.Name = contact.Name;
                contactExisted.Email = contact.Email;
                contactExisted.Map = contact.Map;
                contactExisted.Phone = contact.Phone;
                contactExisted.Description = contact.Description;
                if(contact.ImageUpload != null)
                {
                    string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/Logo");
                    string fileName = Guid.NewGuid().ToString() +"_" + contact.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadDir, fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await contact.ImageUpload.CopyToAsync(fileStream);
                    }
                    if(!string.IsNullOrEmpty(contactExisted.LogoImage))
                    {
                        string oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, "media/Logo", contactExisted.LogoImage);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    contactExisted.LogoImage = fileName;

                }
                await _context.SaveChangesAsync();
                TempData["success"] = "Contact information updated successfully.";
                return RedirectToAction("Contact", "Contact");
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
    }
}
