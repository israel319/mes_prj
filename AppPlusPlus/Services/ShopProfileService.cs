using AppPlusPlus.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AppPlusPlus.Services;

/// <summary>
/// Gère le profil du shop utilisé lors de l'impression (logo/photo, adresses, nom via AppSetting).
/// </summary>
public class ShopProfileService
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory;

    public ShopProfileService(IDbContextFactory<AppDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<ShopProfile> GetAsync()
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        await EnsureTablesAsync(ctx);

        var profile = await ctx.ShopProfiles
            .Include(p => p.AppNameSetting)
            .OrderBy(p => p.Id)
            .FirstOrDefaultAsync();

        if (profile is not null)
        {
            return profile;
        }

        profile = new ShopProfile
        {
            AppNameSettingKey = AppSettingsService.KEY_APP_NAME
        };

        ctx.ShopProfiles.Add(profile);
        await ctx.SaveChangesAsync();
        await ctx.Entry(profile).Reference(p => p.AppNameSetting).LoadAsync();

        return profile;
    }

    public async Task SaveAsync(string? photoShop, string? adresse1, string? adresse2)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        await EnsureTablesAsync(ctx);

        var profile = await ctx.ShopProfiles
            .OrderBy(p => p.Id)
            .FirstOrDefaultAsync();

        if (profile is null)
        {
            profile = new ShopProfile
            {
                AppNameSettingKey = AppSettingsService.KEY_APP_NAME
            };
            ctx.ShopProfiles.Add(profile);
        }

        profile.PhotoShop = string.IsNullOrWhiteSpace(photoShop) ? null : photoShop.Trim();
        profile.Adresse1 = string.IsNullOrWhiteSpace(adresse1) ? null : adresse1.Trim();
        profile.Adresse2 = string.IsNullOrWhiteSpace(adresse2) ? null : adresse2.Trim();
        profile.AppNameSettingKey = AppSettingsService.KEY_APP_NAME;

        await ctx.SaveChangesAsync();
    }

    private static async Task EnsureTablesAsync(AppDbContext ctx)
    {
        await ctx.Database.ExecuteSqlRawAsync("""
            IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[T_AppSettings]') AND type = 'U')
            CREATE TABLE [dbo].[T_AppSettings](
                [Key] [varchar](100) NOT NULL,
                [Value] [varchar](500) NULL,
                CONSTRAINT [PK_T_AppSettings] PRIMARY KEY CLUSTERED ([Key] ASC)
            )
            """);

        await ctx.Database.ExecuteSqlRawAsync("""
            IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[T_Profile]') AND type = 'U')
            BEGIN
                CREATE TABLE [dbo].[T_Profile](
                    [Id] [int] IDENTITY(1,1) NOT NULL,
                    [AppNameSettingKey] [varchar](100) NOT NULL CONSTRAINT [DF_T_Profile_AppNameSettingKey] DEFAULT ('AppName'),
                    [PhotoShop] [nvarchar](max) NULL,
                    [Adresse1] [varchar](250) NULL,
                    [Adresse2] [varchar](250) NULL,
                    CONSTRAINT [PK_T_Profile] PRIMARY KEY CLUSTERED ([Id] ASC),
                    CONSTRAINT [FK_T_Profile_T_AppSettings_AppNameSettingKey] FOREIGN KEY([AppNameSettingKey]) REFERENCES [dbo].[T_AppSettings]([Key])
                )
            END
            """);
    }
}
