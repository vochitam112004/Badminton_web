using Microsoft.EntityFrameworkCore;
using Shopping_Web.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
namespace Shopping_Web.Repository
{
    public class DataContext : IdentityDbContext<AppUser>
    {
        public DataContext(DbContextOptions options) : base(options)
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

                var connectionString = configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }
        public DbSet<Product> Product { get; set; }
        public DbSet<Brands> Brand { get; set; }
        public DbSet<Categories> Categories { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<OrderDetail> orderDetails { get; set; }
        public DbSet<OrderModel> Orders { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }
        public DbSet<Compare> Compares { get; set; }
        public DbSet<Quantity> Quantities { get; set; }
        public DbSet<Shipping> Shippings { get; set; }
    }
}
