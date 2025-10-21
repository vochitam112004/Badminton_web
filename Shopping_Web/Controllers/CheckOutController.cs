using Microsoft.AspNetCore.Mvc;
using Shopping_Web.Models;
using Shopping_Web.Repository;
using System.Security.Claims;
namespace Shopping_Web.Controllers
{
    public class CheckOutController : Controller
    {
        private readonly DataContext _dataContext;
        public CheckOutController(DataContext context)
        {
            _dataContext = context;
        }
       public async Task<IActionResult> CheckOut()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if(userEmail == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                var orderCode = Guid.NewGuid().ToString();
                var orderItem = new OrderModel();
                orderItem.OrderCode = orderCode;
                orderItem.UserName = userEmail;
                orderItem.OrderDate = DateTime.Now;
                orderItem.Status = 1;
                _dataContext.Add(orderItem);
                await _dataContext.SaveChangesAsync();
                List<CartItem> cartItems = HttpContext.Session.GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();
                foreach(var cart in cartItems)
                {
                    var orderDetails = new OrderDetail();
                    orderDetails.UserName = userEmail;
                    orderDetails.Price = cart.Price;
                    orderDetails.Quantity = cart.Quantity;
                    orderDetails.OrderCode = orderCode;
                    orderDetails.ProductId = cart.ProductId;
                    _dataContext.Add(orderDetails);
                    _dataContext.SaveChanges();
                }
                HttpContext.Session.Remove("Cart");
                TempData["success"] = "CheckOut Cart thành công";
                return RedirectToAction("Cart", "Cart");
            }
        }
    }
}
