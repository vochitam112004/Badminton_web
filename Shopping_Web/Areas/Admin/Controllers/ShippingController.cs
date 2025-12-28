using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Shopping_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Shipping")]
    public class ShippingController : Controller
    {
        public IActionResult Shipping()
        {
            return View();
        }
    }
}
