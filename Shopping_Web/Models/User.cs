using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shopping_Web.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        [Required(ErrorMessage ="Pleasee enter username")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Pleasee enter email"), EmailAddress]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Pleasee enter password")]
        public string Password { get; set; }
    }
}
