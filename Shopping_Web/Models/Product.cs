using Microsoft.EntityFrameworkCore;
using Shopping_Web.Repository.ValidationImage;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Shopping_Web.Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductId { get; set; }

        [Required , Range(1, int.MaxValue ,ErrorMessage ="Brand is required")]
        public  int BrandId { get; set; }
        [Required, Range(1, int.MaxValue, ErrorMessage = "Category is required")]
        public  int CategoryId { get; set; }

        [Required]
        public string ProductCode { get; set; }
        [Required]
        [MinLength(2, ErrorMessage = "Product name must be at least 2 characters long.")]
        public string ProductName { get; set; }

        [Required(ErrorMessage ="Price is required")]
        [Precision(18, 2)]
        public Decimal Price { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        public int Quantity { get; set; }
        public int Sole { get; set; }

        [Required(ErrorMessage = "Size is required")]
        public string Size { get; set; } 

        [MinLength(2, ErrorMessage = "Description must be at least 2 characters long. ")]
        public string? Description { get; set; }
        public string? sex { get; set; }
        public string? ImageUrl { get; set; }

        [NotMapped]
        //[FileExtensions(Extensions = "png,jpg,jpeg", ErrorMessage = "Only .png, .jpg, or .jpeg files are allowed.")]
        [FileExtensionsAttibute]
        public IFormFile? ImageUpload { get; set; }
        public DateOnly CreateAt { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public DateOnly? UpdateAt { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public  Brands Brand { get; set; }
        public Rating Ratings { get; set; }
        public Categories Category { get; set; }
    }
}
