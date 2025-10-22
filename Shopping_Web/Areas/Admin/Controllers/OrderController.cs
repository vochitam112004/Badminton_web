using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping_Web.Repository;

namespace Shopping_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly DataContext _dataContext;
        public OrderController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<IActionResult> Order()
        {
            var oders = await _dataContext.Orders.OrderByDescending(o => o.OrderCode).ToListAsync();
            return View(oders);
        }
        public async Task<IActionResult> ViewOrder(string OrderCode)
        {
            if(OrderCode == null)
            {
                return RedirectToAction("Order");
            }
            var oders = await _dataContext.orderDetails.Where(o => o.OrderCode == OrderCode).Include(od => od.Product).ToListAsync();
            if(oders == null)
            {
                return RedirectToAction("Order");
            }
            return View(oders);
        }
    }
}
