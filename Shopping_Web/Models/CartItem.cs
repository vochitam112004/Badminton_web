namespace Shopping_Web.Models
{
    public class CartItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string ImageUrl { get; set; }
        public decimal TotalPrice
        {
            get { return Price * Quantity; }
        }
        public CartItem()
        {

        }
        public CartItem(Product product)
        {
            ProductId = product.ProductId;
            ProductName = product.ProductName;
            ProductCode = product.ProductCode;
            Price = product.Price;
            Quantity = 1;
            ImageUrl = product.ImageUrl;
        }
    }
}
