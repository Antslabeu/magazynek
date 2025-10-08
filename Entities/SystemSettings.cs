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
        [Key] [Column("name")] public string Name { get; set; } = string.Empty;
        [Required] [Column("type")] public SettingType Type { get; set; } = SettingType.STRING;
        [Required] [Column("value")] public string Value { get; set; } = string.Empty;


        [NotMapped]
        public bool B_Value
        {
            get => Value == "true";
            set => this.Value = value.ToString().ToLower();
        }
        [NotMapped]
        public int I_Value
        {
            get => int.TryParse(Value, out int res) ? res : 0;
            set => this.Value = value.ToString();
        }
        [NotMapped]
        public float F_Value
        {
            get => float.TryParse(Value, out float res) ? res : 0;
            set => this.Value = value.ToString();
        }

        protected SystemSetting() { }

        public SystemSetting(string name, SettingType type, string Value)
        {
            this.Name = name;
            this.Type = type;
            this.Value = Value;
        }
    }

    public static class SystemSettingHelper
    {
        public static SystemSetting? GetSettingByName(this List<SystemSetting> list, string Name)
        {
            return list.FirstOrDefault(s => s.Name == Name);
        }
    }   
}