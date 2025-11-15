using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Magazynek.Services
{
    public interface INeededSetting
    {
        List<NeededSettingObject> GetNeddedSettings();


    }
    public class NeededSetting : INeededSetting
    {
        private readonly IConfiguration _config;

        public NeededSetting(IConfiguration config)
        {
            _config = config;
        }
        public List<NeededSettingObject> GetNeddedSettings()
        {   
            return _config.GetSection("NeddedSettings").Get<List<NeededSettingObject>>() ?? new List<NeededSettingObject>();
        }
    }


    public class NeededSettingObject
    {
        public string Name { get; set; } = string.Empty;
        public int Type { get; set; }
        public string Value { get; set; } = string.Empty;
        public int SName { get; set; }
    }   
}