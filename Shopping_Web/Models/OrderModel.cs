using Shopping_Web.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shopping_Web.Models
{
    public class OrderModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderId { get; set; }
        public string OrderCode { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal ShippingCost { get; set; }
        public string UserName { get; set; }
        public DateTime OrderDate { get; set; }
        public int Status { get; set; }
    }
}