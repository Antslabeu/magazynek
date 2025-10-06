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
        Task SaveSetting(SystemSetting systemSetting);
    }


    public class SystemSettingsService : ISystemSettingsService
    {

        private readonly IDataProtector protector;

        private readonly IDbContextFactory<DatabaseContext> dbContextFactory;

        public SystemSettingsService(IDataProtectionProvider provider, IDbContextFactory<DatabaseContext> dbContextFactory)
        {
            this.dbContextFactory = dbContextFactory;
            protector = provider.CreateProtector("AppSettingKeys");
        }

        public async Task<List<SystemSetting>> GetSettings()
        {
            await using var database = await dbContextFactory.CreateDbContextAsync();
            List<SystemSetting> settings = await database.SystemSettings.ToListAsync();

            foreach (SystemSetting setting in settings)
            {
                try { setting.value = protector.Unprotect(setting.value); }
                catch { setting.value = ""; }
            }

            return settings;
        }
        public async Task<T> GetSetting<T>(string name)
        {
            await using var database = await dbContextFactory.CreateDbContextAsync();
            SystemSetting? setting = await database.SystemSettings.FirstOrDefaultAsync(s => s.name == name);
            if (setting != null)
            {
                try { setting.value = protector.Unprotect(setting.value); }
                catch { return default!; }

                return (typeof(T), setting.type) switch
                {
                    (Type t, SystemSetting.SettingType.STRING) when t == typeof(string) => (T)(object)setting.value!,
                    (Type t, SystemSetting.SettingType.INT)    when t == typeof(int)    => (T)(object)setting.i_value,
                    (Type t, SystemSetting.SettingType.BOOL)   when t == typeof(bool)   => (T)(object)setting.b_value,
                    (Type t, SystemSetting.SettingType.FLOAT)  when t == typeof(float)  => (T)(object)setting.f_value,
                    _ => default!
                };
            }
            return default!;
        }
        public async Task SaveSetting(SystemSetting systemSetting)
        {
            await using var database = await dbContextFactory.CreateDbContextAsync();

            SystemSetting? existingSetting = await database.SystemSettings.FirstOrDefaultAsync(s => s.name == systemSetting.name);
            if (existingSetting != null)
            {
                existingSetting.name = systemSetting.name;
                existingSetting.type = systemSetting.type;
                existingSetting.value = protector.Protect(systemSetting.value);
            }
            else await database.SystemSettings.AddAsync(systemSetting);

            await database.SaveChangesAsync();
        }
    }
}