using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Magazynek.Entities;
using Magazynek.Services;

namespace Magazynek.Data
{
    
    public static class StartupHelper
    {
        public static async Task InitApplicationAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var config = services.GetRequiredService<IConfiguration>();

            // init system settings - create standard settings if not exist
            List<SettingDefinition> neededSettings = config.GetSection("NeddedSettings").Get<List<SettingDefinition>>() ?? new List<SettingDefinition>();
            ISystemSettingsService ssService = scope.ServiceProvider.GetRequiredService<ISystemSettingsService>();
            await ssService.CreateStandardSettingsIfNotExist(neededSettings);

        }
    }
}