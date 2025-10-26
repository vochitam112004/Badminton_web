using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shopping_Web.Models;
using Shopping_Web.Models.ViewModels;
using Shopping_Web.Repository;

namespace Shopping_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/User")]
    public class UserController : Controller
    {
        private UserManager<AppUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private readonly DataContext _dataContext;
        public UserController(DataContext dataContext , RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
        {
            this._dataContext = dataContext;
            this._roleManager = roleManager;
            this._userManager = userManager;
        }
        [HttpGet]
        public async Task<IActionResult> User()
        {
            var usersWithRoles = await (from user in _dataContext.Users
                                        join userRole in _dataContext.UserRoles on user.Id equals userRole.UserId into userRoles
                                        from ur in userRoles.DefaultIfEmpty()
                                        join role in _dataContext.Roles on ur.RoleId equals role.Id into roles
                                        from r in roles.DefaultIfEmpty()
                                        select new UserWithRoleViewModel
                                        {
                                            Id = user.Id,
                                            UserName = user.UserName,
                                            Email = user.Email,
                                            PhoneNumber = user.PhoneNumber,
                                            Ocupation = user.Ocupation,
                                            RoleName = r != null ? r.Name : "No Role"
                                        })
                                        .OrderByDescending(u => u.Id)
                                        .ToListAsync();

            return View(usersWithRoles);
        }
        [HttpGet]
        [Route("Create")]
        public async Task<IActionResult> Create()
        {
            // Lấy danh sách Role và chuyển thành SelectList cho View
            await SetRolesViewBag();
            // Trả về ViewModel để Form có thể bind data
            return View(new CreateUserViewModel());
        }
        [HttpPost]
        [Route("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (await _userManager.FindByNameAsync(model.UserName) != null)
                {
                    ModelState.AddModelError("UserName", "User Name existed in Database");
                    await SetRolesViewBag();
                    return View(model);
                }
                if (await _userManager.FindByEmailAsync(model.Email) != null)
                {
                    ModelState.AddModelError("Email", "Email existed in Database");
                    await SetRolesViewBag();
                    return View(model);
                }

                // Tạo đối tượng AppUser từ ViewModel
                var newUser = new AppUser
                {
                    UserName = model.UserName,
                    Ocupation = model.Ocupation,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                };
                var createUserResult = await _userManager.CreateAsync(newUser, model.Password);
                if (createUserResult.Succeeded)
                {
                    // 1. Tìm Role theo RoleId mà Admin đã chọn
                    var role = await _roleManager.FindByIdAsync(model.RoleId);
                    if (role != null)
                    {
                        // 2. Gán Role cho người dùng mới tạo
                        var roleResult = await _userManager.AddToRoleAsync(newUser, role.Name);

                        if (roleResult.Succeeded)
                        {
                            TempData["success"] = $"User {newUser.UserName} created and assigned to role {role.Name} successfully!";
                            return RedirectToAction("User");
                        }
                        else
                        {
                            // Xử lý lỗi nếu không gán được Role (nên xóa User vừa tạo để clean up)
                            await _userManager.DeleteAsync(newUser);
                            TempData["error"] = "Error assigning role to user. User creation rolled back.";
                            await SetRolesViewBag();
                            return View(model);
                        }
                    }
                    else
                    {
                        // Xử lý lỗi nếu không tìm thấy Role
                        await _userManager.DeleteAsync(newUser);
                        TempData["error"] = "Error: Role not found. User creation rolled back.";
                        await SetRolesViewBag();
                        return View(model);
                    }
                }
                else
                {
                    // Xử lý lỗi tạo User (ví dụ: mật khẩu yếu)
                    foreach (var error in createUserResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    await SetRolesViewBag();
                    return View(model);
                }
            }

            // Nếu ModelState không hợp lệ (lỗi validation từ ViewModel)
            await SetRolesViewBag();
            return View(model);
        }
        [HttpGet]
        [Route("Edit")]
        public async Task<IActionResult> Edit(string Id)
        {
            if (string.IsNullOrEmpty(Id))
                return NotFound();

            var user = await _userManager.FindByIdAsync(Id);
            if (user == null)
                return NotFound();
            var userRoles = await _userManager.GetRolesAsync(user);
            var currentRole = userRoles.FirstOrDefault();
            var role = currentRole != null ? await _roleManager.FindByNameAsync(currentRole) : null;

            var model = new EditUser
            {
                Id = user.Id,
                UserName = user.UserName,
                Ocupation = user.Ocupation,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                RoleId = role?.Id
            };

            await SetRolesViewBag();
            return View(model);
        }
        [HttpPost]
        [Route("Edit")]
        public async Task<IActionResult> Edit(EditUser model)
        {
            if (!ModelState.IsValid)
            {
                await SetRolesViewBag();
                return View(model);
            }
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return NotFound();
            }

            // Cập nhật thông tin không liên quan tới password
            user.UserName = model.UserName;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            user.Ocupation = model.Ocupation;

            // Update vào database
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                await SetRolesViewBag();
                return View(model);
            }

            // Update Role
            var userRoles = await _userManager.GetRolesAsync(user);
            var oldRole = userRoles.FirstOrDefault();
            if (!string.IsNullOrEmpty(oldRole))
            {
                await _userManager.RemoveFromRoleAsync(user, oldRole);
            }

            var newRole = await _roleManager.FindByIdAsync(model.RoleId);
            if (newRole != null)
            {
                await _userManager.AddToRoleAsync(user, newRole.Name);
            }
            TempData["SuccessMessage"] = "User updated successfully!";
            return RedirectToAction("User");
        }
        private async Task SetRolesViewBag()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
        }
        [HttpPost]
        [Route("Delete")]
        public async Task<IActionResult> Delete(string Id)
        {
            var user = await _userManager.FindByIdAsync(Id);
            if(user == null)
            {
                return NotFound();
            }
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return View("Error");
            }
            TempData["success"] = "User deleted successfully";
            return RedirectToAction("User");
        }
    }
}
