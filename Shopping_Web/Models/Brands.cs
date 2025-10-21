using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Shopping_Web.Models
{
    public class Brands
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BrandId { get; set; }
        [Required(ErrorMessage ="Brand Name is required")]
        public required string BrandName { get; set; }
        public string? Description { get; set; }
    }
}
