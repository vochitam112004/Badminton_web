using Microsoft.AspNetCore.Mvc;
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
            CartItemViews cartVM = new()
            {
                CartItems = cartItems,
                GrandToTal = cartItems.Sum(item => item.Quantity * item.Price)
            };
            return View(cartVM);
        }
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
            return Redirect(Request.Headers["Referer"].ToString());
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
            List<CartItem> carts = HttpContext.Session.GetJson<List<CartItem>>("Cart");
            CartItem cartItem = carts.Where(c => c.ProductId == ProductId).FirstOrDefault();
            if (cartItem.Quantity >= 1)
            {
                ++cartItem.Quantity;
            }
            else
            {
                carts.RemoveAll(p => p.ProductId == ProductId);
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
    }
}
