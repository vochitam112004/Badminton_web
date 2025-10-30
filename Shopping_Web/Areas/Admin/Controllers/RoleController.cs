using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping_Web.Repository;

namespace Shopping_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RoleController : Controller
    {
        private readonly DataContext _dataContext;
        private RoleManager<IdentityRole> _roleManager;
        public RoleController(DataContext dataContext , RoleManager<IdentityRole> roleManager)
        {
            _dataContext = dataContext;
            _roleManager = roleManager;
        }
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            var roles = await _dataContext.Roles.OrderByDescending(x => x.Id).ToListAsync();
            return View(roles);
        }
        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create(IdentityRole role)
        {
            if (!_roleManager.RoleExistsAsync(role.Name).GetAwaiter().GetResult())
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(role.Name));
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                return View(result);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Role already exists");
                return View(role);
            }
        }
        [HttpGet]
        [Route("Edit")]
        public async Task<IActionResult> Edit(string Id)
        {
            var role = await _roleManager.FindByIdAsync(Id);
            if(role == null)
            {
                return NotFound("Role not exists");
            }
            return View(role);
        }
        [HttpPost]
        [Route("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string Id, IdentityRole model)
        {
            if(string.IsNullOrEmpty(Id))
            {
                return NotFound("Role not exists");
            }
            if (ModelState.IsValid)
            {
                var role = _roleManager.FindByIdAsync(Id).GetAwaiter().GetResult();
                if (role != null)
                {
                    role.Name = model.Name;
                    _roleManager.UpdateAsync(role).GetAwaiter().GetResult();
                    TempData["Success"] = "Role updated successfully";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["Error"] = "Role not found";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                TempData["errors"] = "Invalid data";
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
        [HttpPost]
        [Route("Delete")]
        public async Task<IActionResult> Delete(string Id)
        {
            if (ModelState.IsValid)
            {
                var roles = _roleManager.FindByIdAsync(Id).GetAwaiter().GetResult();
                if (roles != null)
                {
                    _roleManager.DeleteAsync(roles).GetAwaiter().GetResult();
                    TempData["Success"] = "Role deleted successfully";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["Error"] = "Role not found";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                TempData["Error"] = "Invalid data";
                return RedirectToAction("Index");
            }
        }
    }
}
