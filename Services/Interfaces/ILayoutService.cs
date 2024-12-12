using WebApplication1.ViewModels;

namespace WebApplication1.Services.Interfaces
{
    public interface ILayoutService
    {
        Task<Dictionary<string, string>> GetSettingAsync();
        Task<List<BasketItemVM>> GetBasketAsync();
    }
}
