using System.ComponentModel.DataAnnotations;

namespace Shopping_Web.Models.ViewModels
{
    public class CreateUserViewModel
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "Please enter username")]
        public string UserName { get; set; }

        public string Ocupation { get; set; }

        [Required(ErrorMessage = "Please enter email"), EmailAddress]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Please enter password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please enter phone number")]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Please choose a Role")]
        public string RoleId { get; set; }
    }
    public class EditUser
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "Please enter username")]
        public string UserName { get; set; }

        public string Ocupation { get; set; }

        [Required(ErrorMessage = "Please enter email"), EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "Please enter phone number")]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Please choose a Role")]
        public string RoleId { get; set; }
    }
}
