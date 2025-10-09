using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;


namespace Magazynek.Entities
{
    [Table("systemsettings")]
    public class SystemSetting
    {
        public enum SettingName
        {
            TME_API_token,
            Typy_produktów
        };

        private static readonly string ARRAY_SEPARATOR = "%()%";
        public enum SettingType
        {
            STRING,
            INT,
            BOOL,
            FLOAT,
            ARRAY
        }
        [Key] [Column("name")] public string Name { get; set; } = string.Empty;
        [Required] [Column("type")] public SettingType Type { get; set; } = SettingType.STRING;
        [Required] [Column("value")] public string Value { get; set; } = string.Empty;
        [Required] [Column("settingname")] public SettingName SName { get; set; }


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
        [NotMapped]
        public List<string> A_Value
        {
            get
            {
                return this.Value.Split(ARRAY_SEPARATOR).ToList(); 
            }
            set
            {
                this.Value = string.Join(ARRAY_SEPARATOR, value);
            }
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
        public static SystemSetting? GetSettingByType(this List<SystemSetting> list, SystemSetting.SettingName Name)
        {
            return list.FirstOrDefault(s => s.SName == Name);
        }
    }   
}