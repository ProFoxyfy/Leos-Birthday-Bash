using System;
using System.Text;

namespace UnityCipher
{
    public static class Base64
    {
        public static string Encode(string input)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(input);
            return Convert.ToBase64String(bytes);
        }

        public static string Decode(string input)
        {
            byte[] bytes = Convert.FromBase64String(input);
            return Encoding.Unicode.GetString(bytes);
        }
    }
}