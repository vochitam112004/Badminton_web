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
            var oders = await _dataContext.orderDetails.Where(o => o.OrderCode == OrderCode)
                .Include(od => od.Product)
                .ToListAsync();
            if(oders == null)
            {
                return RedirectToAction("Order");
            }
            return View(oders);
        }
        [HttpPost]
        [Route("UpdateOrderStatus")]
        public async Task<IActionResult> UpdateOrderStatus(string orderCode, int status)
        {
            var order = await _dataContext.Orders.FirstOrDefaultAsync(o => o.OrderCode == orderCode);
            if (order == null)
            {
                return Json(new { success = false, message = "Order not found" });
            }
            order.Status = status;
            try
            {
                _dataContext.Update(order);
                await _dataContext.SaveChangesAsync();
                return Json(new { success = true, message = "Order updated successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });

            }

        }
        [HttpPost]
        [Route("DeleteOrder")]
        public async Task<IActionResult> Delete(string OrderCode)
        {
            var order = await _dataContext.Orders.FirstOrDefaultAsync(o => o.OrderCode == OrderCode);
            if (order == null)
            {
                return Json(new { success = false, message = "Order not found" });
            }
            try
            {
                _dataContext.orderDetails.RemoveRange(_dataContext.orderDetails.Where(od => od.OrderCode == OrderCode));
                await _dataContext.SaveChangesAsync();
                _dataContext.Orders.Remove(order);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Order Deleted Successfully";
                return RedirectToAction("ViewOrder");
            }
            catch (Exception ex)
            {
                TempData["error"] = "Error deleting order: ";
                return RedirectToAction("ViewOrder");
            }
        }
    }
}
