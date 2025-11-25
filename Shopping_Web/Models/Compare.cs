using System.ComponentModel.DataAnnotations.Schema;

namespace Shopping_Web.Models
{
    public class Compare
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int UserId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product  { get; set; }
    }
}
