using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shopping_Web.Models
{
    public class OrderDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OderDetailId { get; set; }
        public string UserName { get; set; }
        public int ProductId { get; set; }
        public string OrderCode { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
