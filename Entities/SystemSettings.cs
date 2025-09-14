using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Magazynek.Entities
{
    [Table("systemsettings")]
    public class SystemSetting
    {
        public enum SettingType
        {
            STRING,
            INT,
            BOOL,
            FLOAT
        }
        [Key] public string name { get; set; } = string.Empty;
        [Required] public SettingType type { get; set; } = SettingType.STRING;
        [Required] public string value { get; set; } = string.Empty;


        [NotMapped]
        public bool b_value
        {
            get => value == "true";
            set => this.value = value.ToString().ToLower();
        }
        [NotMapped]
        public int i_value
        {
            get => int.TryParse(value, out int res) ? res : 0;
            set => this.value = value.ToString();
        }
        [NotMapped]
        public float f_value
        {
            get => float.TryParse(value, out float res) ? res : 0;
            set => this.value = value.ToString();
        }

        protected SystemSetting() { }

        public SystemSetting(string name, SettingType type, string value)
        {
            this.name = name;
            this.type = type;
            this.value = value;
        }
    }

    public static class SystemSettingHelper
    {
        public static SystemSetting? GetSettingByName(this List<SystemSetting> list, string name)
        {
            return list.FirstOrDefault(s => s.name == name);
        }
    }   
}