using WebApplication1.ViewModels;

namespace WebApplication1.Services.Interfaces
{
    public interface IBasketService
    {
        Task<List<BasketItemVM>> GetBasketAsync();
    }
}
