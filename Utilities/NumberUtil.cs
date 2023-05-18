using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinishMaker.Utilities
{
    class NumberUtil
    {
        public static bool TryParse(string numberString, out int value)
        {
            if (numberString.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase))
            {
                return int.TryParse(numberString.Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out value);
            }
            else
            {
                return int.TryParse(numberString, out value);
            }
        }
    }
}
