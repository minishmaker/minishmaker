using System;

namespace MinishMaker.Utilities
{
    public static class ExtensionMethods
    {
        public static string Hex(this uint num)
        {
            return Convert.ToString(num, 16).ToUpper();
        }

        public static string Hex(this int num)
        {
            return Convert.ToString(num, 16).ToUpper();
        }

        public static string Hex(this long num)
        {
            return Convert.ToString(num, 16).ToUpper();
        }

        public static string Hex(this ushort num)
        {
            return Convert.ToString(num, 16).ToUpper();
        }

        public static string Hex(this byte num)
        {
            return Convert.ToString(num, 16).ToUpper();
        }

        public static string Hex(this uint num, int length)
        {
            return Convert.ToString(num, 16).ToUpper().PadLeft(length, '0');
        }

        public static string Hex(this int num, int length)
        {
            return Convert.ToString(num, 16).ToUpper().PadLeft(length, '0');
        }

        public static string Hex(this long num, int length)
        {
            return Convert.ToString(num, 16).ToUpper().PadLeft(length, '0');
        }

        public static string Hex(this ushort num, int length)
        {
            return Convert.ToString(num, 16).ToUpper().PadLeft(length, '0');
        }

        public static string Hex(this byte num, int length)
        {
            return Convert.ToString(num, 16).ToUpper().PadLeft(length, '0');
        }
    }
}
