using System.ComponentModel.DataAnnotations;

namespace Shopping_Web.Models.ViewModels
{
    public class ProductDetailsViewModels
    {
        public Product ProductDetails { get; set; }
        [Required(ErrorMessage = "Please enter your name")]
        public string UserName { get; set; }
        [EmailAddress, Required(ErrorMessage = "Please enter your email")]
        public string Email { get; set; }
        public string Review { get; set; }
    }
}
