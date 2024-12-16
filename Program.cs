using System.Management;
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

            string GetSerialNumber()
            {
                string serialNumber = string.Empty;
                var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_BIOS");
                foreach (ManagementObject obj in searcher.Get())
                {
                    serialNumber = obj["SerialNumber"]?.ToString();
                    break;
                }
                return serialNumber;
            }

            string serialNumber = GetSerialNumber();
            Console.WriteLine(serialNumber);

            if (serialNumber == "4CE11718F6")
            {
                //univer
                builder.Services.AddDbContext<AppDbContext>(opt =>
           opt.UseSqlServer(builder.Configuration.GetConnectionString("Univer")));
            }
            else
            {
                //dom
                builder.Services.AddDbContext<AppDbContext>(opt =>
        opt.UseSqlServer(builder.Configuration.GetConnectionString("Home")));

            }

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
            builder.Services.AddScoped<IBasketService, BasketService>();
            builder.Services.AddHttpContextAccessor();
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
