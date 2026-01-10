using System.ComponentModel.DataAnnotations.Schema;

namespace Shopping_Web.Models
{
    public class Shipping
    {
        public int Id { get; set; }
        public string District { get; set; }
        public string City { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }
        public string Ward { get; set; }
        public DateTime Datecreated { get; set; }
    }
}
