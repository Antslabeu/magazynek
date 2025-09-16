using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Magazynek.Entities;

namespace Magazynek.Data
{
    public static class StaticHelper
    {
        public static DisplayInfoAttribute? GetDisplayInfo(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            return field?.GetCustomAttribute<DisplayInfoAttribute>();
        }
        public static string GetLabel(this Enum value) => value.GetDisplayInfo()?.Label ?? value.ToString();
        public static string GetAbbreviation(this Enum value) => value.GetDisplayInfo()?.Abbreviation ?? "";
    }
}