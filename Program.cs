using Microsoft.EntityFrameworkCore;
using WebApplication1.DAL;

namespace WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<AppDbContext>(opt =>
            opt.UseSqlServer("server=DESKTOP-JR023V5\\SQLEXPRESS;Database=WebAppDB;Trusted_Connection=True;integrated security=true;TrustServerCertificate=true;"));
            var app = builder.Build();
            app.UseStaticFiles();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}"
            );

            app.Run();
        }
    }
}
