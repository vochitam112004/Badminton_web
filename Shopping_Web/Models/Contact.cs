
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shopping_Web.Repository.ValidationImage;

namespace Shopping_Web.Models
{
    public class Contact
    {
        [Key]
        public int ContactId { get; set; }
        [Required(ErrorMessage = "Vui Lòng nhập tên của bạn")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Vui Lòng nhập email của bạn")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập Map")]
        public string Map { get; set; }
        [Required(ErrorMessage = "Vui Lòng nhập số điện thoại của bạn")]
        public string Phone { get; set; }
        [Required (ErrorMessage = "Vui Lòng nhập mô tả")]
        public string Description { get; set; }
        public string LogoImage { get; set; }
        [NotMapped]
        [FileExtensionsAttibute]
        public IFormFile ImageUpload { get; set; }

    }
}
