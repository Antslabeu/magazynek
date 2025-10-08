using Magazynek.Data;
using Magazynek.Entities;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;


namespace Magazynek.Services
{
    public interface ISystemSettingsService
    {
        Task<List<SystemSetting>> GetSettings();
        Task<T> GetSetting<T>(string name);
        Task SaveSetting(SystemSetting systemSetting, bool saveChangesAsync = true);

        Task InitSettings();
    }


    public class SystemSettingsService : ISystemSettingsService
    {

        private readonly IDataProtector protector;
        private readonly IConfiguration configuration;
        private readonly IDbContextFactory<DatabaseContext> dbContextFactory;

        public SystemSettingsService(IDataProtectionProvider provider, IDbContextFactory<DatabaseContext> dbContextFactory, IConfiguration configuration)
        {
            this.dbContextFactory = dbContextFactory;
            protector = provider.CreateProtector("AppSettingKeys");
            this.configuration = configuration;
        }

        public async Task<List<SystemSetting>> GetSettings()
        {
            await using var database = await dbContextFactory.CreateDbContextAsync();
            List<SystemSetting> settings = await database.SystemSettings.ToListAsync();

            foreach (SystemSetting setting in settings)
            {
                try { setting.Value = protector.Unprotect(setting.Value); }
                catch { setting.Value = ""; }
            }

            return settings;
        }
        public async Task<T> GetSetting<T>(string name)
        {
            await using var database = await dbContextFactory.CreateDbContextAsync();
            SystemSetting? setting = await database.SystemSettings.FirstOrDefaultAsync(s => s.Name == name);
            if (setting != null)
            {
                try { setting.Value = protector.Unprotect(setting.Value); }
                catch { return default!; }

                return (typeof(T), setting.Type) switch
                {
                    (Type t, SystemSetting.SettingType.STRING) when t == typeof(string) => (T)(object)setting.Value!,
                    (Type t, SystemSetting.SettingType.INT) when t == typeof(int) => (T)(object)setting.I_Value,
                    (Type t, SystemSetting.SettingType.BOOL) when t == typeof(bool) => (T)(object)setting.B_Value,
                    (Type t, SystemSetting.SettingType.FLOAT) when t == typeof(float) => (T)(object)setting.F_Value,
                    _ => default!
                };
            }
            return default!;
        }
        public async Task SaveSetting(SystemSetting systemSetting, bool saveChangesAsync = true)
        {
            await using var database = await dbContextFactory.CreateDbContextAsync();

            SystemSetting? existingSetting = await database.SystemSettings.FirstOrDefaultAsync(s => s.Name == systemSetting.Name);
            if (existingSetting != null)
            {
                existingSetting.Name = systemSetting.Name;
                existingSetting.Type = systemSetting.Type;
                existingSetting.Value = protector.Protect(systemSetting.Value);
            }
            else await database.SystemSettings.AddAsync(systemSetting);

            if (saveChangesAsync) await database.SaveChangesAsync();
        }

        public async Task InitSettings()
        {
            var settings = configuration.GetSection("NeddedSettings").Get<List<SystemSetting>>();
            if(settings == null || settings.Count == 0) return;

            await using var db = await dbContextFactory.CreateDbContextAsync();
            foreach (var s in settings)
            {
                if (!db.SystemSettings.Any(x => x.Name == s.Name))
                    db.SystemSettings.Add(new SystemSetting(s.Name, s.Type, protector.Protect(string.Empty)));
            }
            await db.SaveChangesAsync();
        }
    }
}