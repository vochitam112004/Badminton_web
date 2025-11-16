using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Shopping_Web.Repository.Components
{
    public class FooterViewComponent : ViewComponent
    {
        private readonly DataContext _context;
        public FooterViewComponent(DataContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync ()
        {
            var contact = await _context.Contacts.FirstOrDefaultAsync();
            return View(contact);
        }
    }
}
