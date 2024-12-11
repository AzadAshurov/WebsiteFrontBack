namespace WebApplication1.Services.Interfaces
{
    public interface ILayoutService
    {
        Task<Dictionary<string, string>> GetSettingAsync();
    }
}
