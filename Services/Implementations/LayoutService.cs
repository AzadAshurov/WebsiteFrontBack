using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WebApplication1.DAL;
using WebApplication1.Models;
using WebApplication1.Services.Interfaces;
using WebApplication1.ViewModels;

namespace WebApplication1.Services.Implementations
{
    public class LayoutService : ILayoutService
    {
        private readonly AppDbContext _context;
        private readonly HttpContext _http;

        public LayoutService(AppDbContext context, IHttpContextAccessor http)
        {
            _context = context;
            _http = http.HttpContext;
        }

        public async Task<List<BasketItemVM>> GetBasketAsync()
        {

            List<BasketCookieItemVM> cookiesVM;
            string cookie = _http.Request.Cookies["basket"];

            List<BasketItemVM> basketVM = new();

            if (cookie == null)
            {
                return basketVM;
            }

            cookiesVM = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(cookie);

            foreach (BasketCookieItemVM item in cookiesVM)
            {
                Product product = await _context.Products
                    .Include(p => p.ProductImages.Where(pi => pi.IsPrimary == true))
                    .FirstOrDefaultAsync(p => p.Id == item.Id);

                if (product != null)
                {
                    basketVM.Add(new BasketItemVM
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Image = product.ProductImages[0].Image,
                        Price = product.Price,
                        Count = item.Count,
                        SubTotal = item.Count * product.Price
                    });
                }
            }


            return basketVM;
        }

        public async Task<Dictionary<string, string>> GetSettingAsync()
        {
            Dictionary<string, string> settings = await _context.Settings.ToDictionaryAsync(x => x.Key, x => x.Value);
            return settings;
        }

    }
}
