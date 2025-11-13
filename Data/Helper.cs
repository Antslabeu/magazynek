using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace magazynek.Data
{
    public static class Helper
    {
        public static string DatetimeToString(DateTime dt)
        {
            return dt.ToString("MM-dd-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
        }

    }
}