
using Microsoft.AspNetCore.Mvc;
using Shopping_Web.Models;
using Shopping_Web.Repository;
using System.Security.Claims;
using Shopping_Web.Areas.Admin.Repository;
namespace Shopping_Web.Controllers
{
    public class CheckOutController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IEmailSender _emailSender;
        public CheckOutController(DataContext context, IEmailSender emailSender)
        {
            _dataContext = context;
            _emailSender = emailSender;
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
                var email = "vochitam112004@gmail.com";
                var subject = "Tâm Badminton";
                var mesage = "Chúc mừng bạn đã đặt hành thành công , vui lòng chờ duyệt đơn hàng !";
                await _emailSender.SendEmailAsync(email, subject, mesage);
                TempData["success"] = "CheckOut Cart thành công ,vui lòng kiểm tra email";
                return RedirectToAction("Cart", "Cart");
            }
        }
    }
}
