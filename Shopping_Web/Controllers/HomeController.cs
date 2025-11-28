using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping_Web.Models;
using Shopping_Web.Repository;
using Shopping_Web.Models.ViewModels;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Identity;
namespace Shopping_Web.Controllers
{
    public class HomeController : Controller
    {
        private UserManager<AppUser> _userManager;
        private readonly ILogger<HomeController> _logger;
        private readonly DataContext _context;
        public HomeController(ILogger<HomeController> logger, DataContext context, UserManager<AppUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }
        

        public async Task<IActionResult> Home()
        {
            var products = _context.Product.Include("Category").Include("Brand").ToList();
            var slider = _context.Sliders.Where( s => s.Status ==1).ToList();
            return View (new HomeViewModel
            {
                Products = products,
                Sliders = slider
            });
        }
        [HttpGet]
        public async Task<IActionResult> WishList()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Compare()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddWishList(int ProductId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized(new { success = false, message = "Bạn cần đăng nhập để thực hiện chức năng này" });
            }
            // 1. Kiểm tra xem sản phẩm đã tồn tại trong Wishlist của User này chưa
            var existingItem = await _context.Wishlists 
                .FirstOrDefaultAsync(w => w.ProductId == ProductId && w.UserId == user.Id);

            if (existingItem != null)
            {
                return Ok(new { success = false, message = "Thêm sản phẩm thất bại hoặc đã có trong mục yêu thích" });
            }
            // 2. Khởi tạo đối tượng mới 
            var wishlist = new Wishlist
            {
                ProductId = ProductId,
                UserId = user.Id
            };
            try
            {
                _context.Add(wishlist); 
                await _context.SaveChangesAsync();
                return Ok(new { success = true, message = "Thêm vào danh sách yêu thích thành công" });
            }
            catch (Exception ex)
            {
               
                return StatusCode(500, new { success = false, message = "Lỗi server: " + ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddComapre(int ProductId)
        {
            var user= await _userManager.GetUserAsync(User);
            if(user == null)
            {
                return Unauthorized(new { success = false, message = "Bạn cần đăng nhập để thực hiện chức năng này" });
            }
            var compareExisted = await _context.Compares.FirstOrDefaultAsync(c =>c.ProductId == ProductId && c.UserId ==user.Id);
            if (compareExisted != null)
            {
                return Ok(new { success = false, message = "Thêm sản phẩm thất bại hoặc đã có trong mục so sánh" });
            }
            var compare = new Compare()
            {
                ProductId = ProductId,
                UserId = user.Id
            };
            try
            {
                _context.Add(compare);
                await _context.SaveChangesAsync();
                return Ok(new { success = true, message = "Thêm sản phẩm compare thành công" });
            }
            catch (Exception ex)
            {
                 return StatusCode(500, new { success = false, message = "Lỗi server: " + ex.Message });
            }
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int Statuscode)
        {
            if(Statuscode == 404)
            {
                return View("NotFound");
            }
            else
            {
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }  
        }
        public async Task<IActionResult> Contact()
        {
            return View(await _context.Contacts.FirstOrDefaultAsync());
        }
    }
}
