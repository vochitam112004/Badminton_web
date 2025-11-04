using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shopping_Web.Models
{
    public class Rating
    {
        public int RatingId { get; set; }
        public int ProductId { get; set; }
        [Required(ErrorMessage ="Please enter your name")]
        public string UserName { get; set; }
        [EmailAddress , Required(ErrorMessage ="Please enter your email")]
        public string Email { get; set; }
        public string Score { get; set; } // e.g., 1 to 5
        public string Review { get; set; }
        public DateTime CreatedAt { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
    }
}
