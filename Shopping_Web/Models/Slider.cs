using Shopping_Web.Repository.ValidationImage;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shopping_Web.Models
{
    public class Slider
    {
        [Key]
        public int SliderId { get; set; }
        [Required(ErrorMessage = "Please enter Name")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Please enter Description")]
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public int? Status { get; set; }
        [NotMapped]
        [FileExtensionsAttibute]
        public IFormFile ImageFile { get; set; }

    }
}
