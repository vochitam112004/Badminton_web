using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Shopping_Web.Repository.Components
{
    public class BrandsViewComponent : ViewComponent
    {
        private readonly DataContext _context;
        public BrandsViewComponent(DataContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var brands = await _context.Brand.ToListAsync();
            return View("BrandsView", brands);
        }
    }
}
