using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Shopping_Web.Models;
using Shopping_Web.Models.CartItemView;
using Shopping_Web.Repository;
namespace Shopping_Web.Controllers
{
    public class CartController : Controller
    {
        private readonly DataContext _context;
        public CartController(DataContext _context)
        {
            this._context = _context;
        }
        public IActionResult Cart()
        {
            List<CartItem> cartItems = HttpContext.Session.GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            var shippingPriceCookies = Request.Cookies["ShippingCookie"];
            decimal shippingPrice = 0;

            if(shippingPriceCookies != null)
            {
                var shippingPriceJson = shippingPriceCookies;
                shippingPrice = JsonConvert.DeserializeObject<decimal>(shippingPriceJson);
            }

            CartItemViews cartVM = new()
            {
                CartItems = cartItems,
                GrandToTal = cartItems.Sum(item => item.Quantity * item.Price)
            };
            return View(cartVM);
        }
        [HttpPost]
        public async Task<IActionResult> Add(int ProductId)
        {
            Product product = await _context.Product.FindAsync(ProductId);
            List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            CartItem cartItems = cart.Where(c => c.ProductId == ProductId).FirstOrDefault();
            if(cartItems == null)
            {
                cart.Add(new CartItem(product));
            }
            else
            {
                cartItems.Quantity += 1;
            }
            HttpContext.Session.SetJson("Cart", cart);
            TempData["success"] = "Add to cart successfully";
            return Json(new { success = true });
        }
        public async Task<IActionResult> Decrease(int ProductId)
        {
            List<CartItem> carts = HttpContext.Session.GetJson<List<CartItem>>("Cart");
            CartItem cartItem = carts.Where(c => c.ProductId == ProductId).FirstOrDefault();

            if (cartItem.Quantity > 1)
            {
                --cartItem.Quantity;
            }
            else
            {
                carts.RemoveAll(p => p.ProductId == ProductId);
            }
            if(carts.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart", carts);
                TempData["success"] = "Decrease to cart successfully";
            }
            return RedirectToAction("Cart");
        }
        public async Task<IActionResult> Increase(int ProductId)
        {
            Product product = await _context.Product.Where(p => p.ProductId == ProductId).FirstAsync();
            List<CartItem> carts = HttpContext.Session.GetJson<List<CartItem>>("Cart");
            CartItem cartItem = carts.Where(c => c.ProductId == ProductId).FirstOrDefault();
            if (cartItem.Quantity >= 1 && product.Quantity > cartItem.Quantity)
            {
                ++cartItem.Quantity;
              
            }
            else
            {
                cartItem.Quantity = product.Quantity;
                TempData["success"] = "Quantities Product is maximum";
                //carts.RemoveAll(p => p.ProductId == ProductId);
            }
            if (carts.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart", carts);
                TempData["success"] = "Increase to cart successfully";
            }
            return RedirectToAction("Cart");

        }
        public async Task<IActionResult> Remove (int ProductId)
        {
            List<CartItem> carts = HttpContext.Session.GetJson<List<CartItem>>("Cart");
            carts.RemoveAll(c => c.ProductId == ProductId);

            if(carts.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart", carts);
                TempData["success"] = "Remove to cart Product successfully";
            }
            return RedirectToAction("Cart");
        }
        public async Task<IActionResult> Clear()
        {
            HttpContext.Session.Remove("Cart");
            TempData["success"] = "Clear to cart successfully";
            return RedirectToAction("Cart");
        }
        [HttpPost]
        public async Task<IActionResult> GetShiping(Shipping shippingModel, string tinh, string quan, string phuong)
        {
            var ShippingExisted = await _context.Shippings.FirstOrDefaultAsync(s => s.City == tinh && s.District == quan && s.Ward == phuong);
            decimal ShippingPrice = 0;
            if (ShippingExisted != null)
            {
                ShippingPrice = ShippingExisted.Price;
            }
            else
            {
                ShippingPrice = 50000;
            }
            var ShippingPriceJson = JsonConvert.SerializeObject(ShippingPrice);
            try
            {
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTime.UtcNow.AddMinutes(30),
                    Secure = true,
                };
                Response.Cookies.Append("ShippingCookie", ShippingPriceJson, cookieOptions);
            }catch (Exception ex)
            {
                Console.WriteLine($"Error adding shipping price cookie: {ex.Message}" );
            }
            return Json(new { ShippingPrice });
        }
    }
}
