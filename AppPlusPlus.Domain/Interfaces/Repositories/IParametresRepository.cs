using AppPlusPlus.Domain.Entities.Parametres;

namespace AppPlusPlus.Domain.Interfaces.Repositories;

public interface IParametresRepository
{
    // AppSetting
    Task<AppSetting?> GetSettingAsync(string key);
    Task<List<AppSetting>> GetAllSettingsAsync();
    Task SetSettingAsync(string key, string? value);

    // ShopProfile
    Task<ShopProfile?> GetShopProfileAsync();
    Task AddShopProfileAsync(ShopProfile profile);
    Task SaveShopProfileAsync(ShopProfile profile);
    Task UpdateShopProfileAsync(ShopProfile profile);
}
