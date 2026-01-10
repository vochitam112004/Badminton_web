namespace Shopping_Web.Models.CartItemView
{
    public class CartItemViews
    {
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public decimal GrandToTal { get;set; } 
        public decimal ShippingPrice { get ; set; }
    }
}
