using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shopping_Web.Repository;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Shopping_Web.Models;
namespace Shopping_Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // EF Core DbContext
            builder.Services.AddDbContext<DataContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // đăng ký Identity
            builder.Services.AddIdentity<AppUser, IdentityRole>()
                    .AddEntityFrameworkStores<DataContext>().AddDefaultTokenProviders();

            builder.Services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5; //truy cập sai 5 lần thì khóa
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = false;
            });
            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IOTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.IsEssential = true;
            });
            var app = builder.Build();
            // Đăng ký seesion 
            app.UseSession();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            // cấu hình nếu xảy ra lỗi 404 thì chuyển hướng về trang /Home/Error    
            app.UseStatusCodePagesWithRedirects("/Home/Error?Statuscode={0}");

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication(); // xac thuc 
            app.UseAuthorization(); // phan quyen

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "Areas",
                pattern: "{area:exists}/{controller=Product}/{action=Product}/{id?}");

            
            app.MapControllerRoute(
                name: "CategoryByName",
                pattern: "categories/{categoryName}",
                defaults: new { controller = "Category", action = "Category" });

            app.MapControllerRoute(
               name: "BrandByName",
               pattern: "Brands/{brandName}",
               defaults: new { controller = "Brand", action = "Brands" });

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Home}/{id?}");
            
            //Seeding data
            var context = app.Services.CreateScope().ServiceProvider.GetRequiredService<DataContext>();
            //SeedingData.seeding(context);
            //app.Urls.Add("http://0.0.0.0:8080");

            app.Run();
        }
    }
}
