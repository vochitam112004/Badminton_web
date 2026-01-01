using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping_Web.Models;
using Shopping_Web.Repository;
using System.Threading.Tasks;

namespace Shopping_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Shipping")]
    public class ShippingController : Controller
    {
        private readonly DataContext _context;
        public ShippingController(DataContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Shipping()
        {
            var shipping = await _context.Shippings.ToListAsync();
            ViewBag.Shippings = shipping;
            return View();
        }
        [HttpPost]
        [Route("StoreShipping")]
        public async Task<IActionResult> StoreShipping(Shipping shippingModel , string tinh , string quan, string phuong, decimal price)
        {
            shippingModel.City = tinh;
            shippingModel.District = quan;
            shippingModel.Ward = phuong;
            shippingModel.Price = price;
            try
            {
                var shippingExisted = await _context.Shippings.AnyAsync(s => s.City == tinh && s.District == quan && s.Ward == phuong);
                if (shippingExisted)
                {
                    return Ok(new { duplicate = true, message = "dữ liệu bị trùng lặp" });
                }
                _context.Shippings.Add(shippingModel);
                await _context.SaveChangesAsync();
                return Ok(new { success = true, message ="Thêm shipping thành công" });
            }
            catch(Exception)
            {
                return StatusCode(500, "An error occurred while adding shipping");
            }
        }
        [Route("Delete")]
        public async Task<IActionResult>Delete(int id)
        {
           var shippingExisted = await _context.Shippings.FindAsync(id);
           if(shippingExisted == null)
            {
                return NotFound();
            }
           _context.Shippings.Remove(shippingExisted);
           await _context.SaveChangesAsync();
            TempData["success"] = "Shipping deleted successfully";
            return RedirectToAction("Shipping");
        }
    }
}
