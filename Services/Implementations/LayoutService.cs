using Microsoft.EntityFrameworkCore;
using WebApplication1.DAL;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Services.Implementations
{
    public class LayoutService : ILayoutService
    {
        private readonly AppDbContext _context;


        public LayoutService(AppDbContext context)
        {
            _context = context;

        }

        //public async Task<List<BasketItemVM>> GetBasketAsync()
        //{
        //    List<BasketItemVM> basketVM = new();

        //    if (_user.Identity.IsAuthenticated)
        //    {
        //        if (_context.BasketItems is not null)
        //        {
        //            basketVM = _context.BasketItems
        //               .Where(bi => bi.AppUserId == _user.FindFirstValue(ClaimTypes.NameIdentifier))
        //               .Select(bi => new BasketItemVM
        //               {
        //                   Count = bi.Count,
        //                   Price = bi.Product.Price,
        //                   Image = bi.Product.ProductImages.FirstOrDefault(pi => pi.IsPrimary == true).Image,
        //                   Name = bi.Product.Name,
        //                   SubTotal = bi.Product.Price * bi.Count,
        //                   Id = bi.ProductId
        //               })
        //               .ToList();
        //            Console.WriteLine("NOTNULL");
        //        }
        //    }
        //    else
        //    {
        //        List<BasketCookieItemVM> cookiesVM;
        //        string cookie = _http.Request.Cookies["basket"];


        //        if (cookie == null)
        //        {
        //            return basketVM;
        //        }

        //        cookiesVM = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(cookie);

        //        foreach (BasketCookieItemVM item in cookiesVM)
        //        {
        //            Product product = await _context.Products
        //                .Include(p => p.ProductImages.Where(pi => pi.IsPrimary == true))
        //                .FirstOrDefaultAsync(p => p.Id == item.Id);

        //            if (product != null)
        //            {
        //                basketVM.Add(new BasketItemVM
        //                {
        //                    Id = product.Id,
        //                    Name = product.Name,
        //                    Image = product.ProductImages[0].Image,
        //                    Price = product.Price,
        //                    Count = item.Count,
        //                    SubTotal = item.Count * product.Price
        //                });
        //            }
        //        }
        //    }
        //    return basketVM;
        //}
        public async Task<Dictionary<string, string>> GetSettingAsync()
        {
            Dictionary<string, string> settings = await _context.Settings.ToDictionaryAsync(x => x.Key, x => x.Value);
            return settings;
        }

    }
}
