using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shopping_Web.Models;
using Shopping_Web.Models.ViewModels;

namespace Shopping_Web.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<AppUser> userManager;
        private SignInManager<AppUser> signInManager;

        public AccountController(UserManager<AppUser> userMgr, SignInManager<AppUser> signInMgr)
        {
            userManager = userMgr;
            signInManager = signInMgr;
        }
        public IActionResult Login(string returnUrl)
        {
            return View(new LoginViews { returnUrl = returnUrl });
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViews loginViews)
        {
            if (ModelState.IsValid)
            {
                Microsoft.AspNetCore.Identity.SignInResult result = await signInManager.PasswordSignInAsync(loginViews.UserName ,loginViews.Password , false , false);
                if (result.Succeeded)
                {
                    return Redirect(loginViews.returnUrl ?? "/");
                }
                ModelState.AddModelError("", "Username or password Invalid");
            }
            return View(loginViews);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user)
        {
           if(ModelState.IsValid)
           {
                AppUser newUser = new AppUser { UserName = user.UserName, Email = user.Email};
                IdentityResult result = await userManager.CreateAsync(newUser,user.Password);
                if (result.Succeeded)
                {
                    TempData["sucess"] = "User Created Successfully";
                    return RedirectToAction("Login");
                }
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(user);
        }
        public async Task<IActionResult> LogOut(string returnUrl = "/")
        {
            await signInManager.SignOutAsync();
            return Redirect(returnUrl);
        }
    }
}
