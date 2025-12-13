using Magazynek.Data;
using Magazynek.Entities;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;


namespace Magazynek.Services
{
    public interface ISystemSettingsService
    {
        Task<List<SystemSetting>> GetSettings(User user);
        Task<T> GetSetting<T>(string name, User user);
        Task SaveSetting(SystemSetting systemSetting, User user, bool saveChangesAsync = true);
    }


    public class SystemSettingsService : ISystemSettingsService
    {
        private readonly bool protectorEnabled = true;

        private readonly IDataProtector protector;
        private readonly IConfiguration configuration;
        private readonly IDbContextFactory<DatabaseContext> dbContextFactory;

        public SystemSettingsService(IDataProtectionProvider provider, IDbContextFactory<DatabaseContext> dbContextFactory, IConfiguration configuration)
        {
            this.dbContextFactory = dbContextFactory;
            protector = provider.CreateProtector("AppSettingKeys");
            this.configuration = configuration;
        }

        public async Task<List<SystemSetting>> GetSettings(User user)
        {
            await using var database = await dbContextFactory.CreateDbContextAsync();
            List<SystemSetting> settings = await database.SystemSettings.Where(s => s.userID == user.id).ToListAsync();

            if(protectorEnabled)
            {
                foreach (SystemSetting setting in settings)
                {
                    try { setting.Value = protector.Unprotect(setting.Value); }
                    catch { setting.Value = ""; }
                }
            }

            return settings;
        }
        public async Task<T> GetSetting<T>(string name, User user)
        {
            await using var database = await dbContextFactory.CreateDbContextAsync();
            SystemSetting? setting = await database.SystemSettings.FirstOrDefaultAsync(s => s.Name == name && s.userID == user.id);
            if (setting != null)
            {
                if(protectorEnabled)
                {
                    try { setting.Value = protector.Unprotect(setting.Value); }
                    catch { return default!; }
                }

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
        public async Task SaveSetting(SystemSetting systemSetting, User user, bool saveChangesAsync = true)
        {
            await using var database = await dbContextFactory.CreateDbContextAsync();

            SystemSetting? existingSetting = await database.SystemSettings.FirstOrDefaultAsync(s => s.Name == systemSetting.Name && s.userID == user.id);
            if (existingSetting != null)
            {
                existingSetting.Name = systemSetting.Name;
                existingSetting.Type = systemSetting.Type;
                if(protectorEnabled) existingSetting.Value = protector.Protect(systemSetting.Value);
                else existingSetting.Value = systemSetting.Value;
            }
            else await database.SystemSettings.AddAsync(systemSetting);

            if (!saveChangesAsync) return;
            var affected = await database.SaveChangesAsync();
            Console.WriteLine($"SaveChanges affected: {affected}");
        }
    }
}