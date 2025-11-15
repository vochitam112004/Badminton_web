using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping_Web.Models;
using Shopping_Web.Repository;
using Shopping_Web.Models.ViewModels;
namespace Shopping_Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataContext _context;
        public HomeController(ILogger<HomeController> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
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
        public IActionResult Contact()
        {
            return View();
        }
    }
}
