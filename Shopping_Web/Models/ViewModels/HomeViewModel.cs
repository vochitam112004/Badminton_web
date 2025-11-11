namespace Shopping_Web.Models.ViewModels
{
    public class HomeViewModel
    {
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<Slider> Sliders { get; set; }
    }
}
