using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Areas.Admin.Models;
using WebApplication1.DAL;
using WebApplication1.Services.Implementations;
using WebApplication1.Services.Interfaces;

namespace WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<AppDbContext>(opt =>
            opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
            builder.Services.AddIdentity<AppUser, IdentityRole>(opt =>
            {
                opt.Password.RequiredLength = 8;
                opt.Password.RequireNonAlphanumeric = false;
                //opt.User.AllowedUserNameCharacters = "qwertyuiopasdfghjklzxcvbnm";
                opt.User.RequireUniqueEmail = true;
                opt.Lockout.AllowedForNewUsers = true;
                opt.Lockout.MaxFailedAccessAttempts = 4;
                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromSeconds(100);
            }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
            builder.Services.AddScoped<ILayoutService, LayoutService>();


            var app = builder.Build();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseStaticFiles();
            app.MapControllerRoute(
               name: "admin",
               pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
           );
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}"
            );

            app.Run();
        }
    }
}
