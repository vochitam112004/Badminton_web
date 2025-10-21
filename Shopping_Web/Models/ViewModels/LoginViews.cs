using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.ComponentModel.DataAnnotations;

namespace Shopping_Web.Models.ViewModels
{
    public class LoginViews
    {
        [Required(ErrorMessage = "Pleasee enter username")]
        public string UserName { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Pleasee enter password")]
        public string Password { get; set; }
        public String returnUrl { get; set; }
    }
}
