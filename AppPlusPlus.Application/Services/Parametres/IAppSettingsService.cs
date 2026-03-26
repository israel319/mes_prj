namespace AppPlusPlus.Application.Services.Parametres;

/// <summary>
/// Singleton service that caches global application settings (including the app name).
/// Loaded at startup, refreshable at any time.
/// </summary>
public interface IAppSettingsService
{
    const string KEY_APP_NAME = "AppName";

    string AppName { get; }
    string? Get(string key);
    Task LoadAsync();
    Task SetAsync(string key, string value);
    event Action? OnChange;
}
