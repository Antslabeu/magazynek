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
            Typy_produktów,
            NameTableProduct
        };
        public enum SettingType
        {
            STRING,
            INT,
            BOOL,
            FLOAT,
            ARRAY,
            ARRAY_STATIC_SIZE
        }

        public enum SettingStaticLengths
        {
            NameTableProduct = 6
        }

        private static readonly string ARRAY_SEPARATOR = "%()%";
        
        [Key] public Guid id { get; set; }
        [Required] [Column("name")] public string Name { get; set; } = string.Empty;
        [Required] [Column("userid")] public Guid userID { get; set; }
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
                if(this.Type == SettingType.ARRAY_STATIC_SIZE)
                {
                    List<string> temporary = this.Value.Split(ARRAY_SEPARATOR).ToList();
                    if(temporary.Count != GetStaticLength(SName))
                    {
                        temporary = Enumerable.Repeat(string.Empty, GetStaticLength(SName)).ToList();
                        this.Value = string.Join(ARRAY_SEPARATOR, temporary);
                    }
                }

                return this.Value.Split(ARRAY_SEPARATOR).ToList();
            }
            set
            {
                if(value.Count != GetStaticLength(SName) && this.Type == SettingType.ARRAY_STATIC_SIZE)
                {
                    value = Enumerable.Repeat(string.Empty, GetStaticLength(SName)).ToList();
                }
                this.Value = string.Join(ARRAY_SEPARATOR, value);
            }
        }

        protected SystemSetting() { }

        public SystemSetting(string name, SettingType type, string Value, SettingName sName, Guid userID)
        {
            this.Name = name;
            this.Type = type;
            this.Value = Value;
            this.SName = sName;
            this.userID = userID;
        }

        public static int GetStaticLength(SettingName name)
        {
            return name switch
            {
                SettingName.NameTableProduct => (int)SettingStaticLengths.NameTableProduct,
                _ => 0,
            };
        }
        public static List<string> PrepareShippingNames(SystemSetting? productSettings)
        {
            List<string> names = new List<string>();
            if(productSettings != null && productSettings.A_Value.Count == GetStaticLength(SettingName.NameTableProduct))
            {
                names.Add(productSettings.A_Value[0]); // Product type
                names.Add(productSettings.A_Value[1]); // Product name
                names.Add(productSettings.A_Value[3]); // Product description
                names.Add(productSettings.A_Value[4]); // Product package
                names.Add("Na stanie");
                names.Add(productSettings.A_Value[5]); // Product TME ID
                names.Add("Dostępne");
            }
            return names;
        }
        public static bool HasInvalidCharacters(List<string> strings)
        {
            foreach(var item in strings) if(item.Contains(ARRAY_SEPARATOR)) return true;
            return false;
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