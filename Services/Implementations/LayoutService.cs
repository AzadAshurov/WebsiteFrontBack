using Microsoft.EntityFrameworkCore;
using WebApplication1.DAL;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Services.Implementations
{
    public class LayoutService : ILayoutService
    {
        private readonly AppDbContext _appDbContext;

        public LayoutService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<Dictionary<string, string>> GetSettingAsync()
        {
            Dictionary<string, string> settings = await _appDbContext.Settings.ToDictionaryAsync(x => x.Key, x => x.Value);
            return settings;
        }
    }
}
