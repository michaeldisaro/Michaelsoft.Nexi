using System;
using System.Security.Cryptography;
using System.Text;

namespace Michaelsoft.Nexi.Extensions
{
    public class EncodingHelper
    {

        public static string toSafeUrlBase64(byte[] input)
        {
            return Convert.ToBase64String(input)
                          .TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }

        public static byte[] fromSafeUrlBase64(string base64)
        {
            string incoming = base64
                              .Replace('_', '/').Replace('-', '+');
            switch (base64.Length % 4)
            {
                case 2:
                    incoming += "==";
                    break;
                case 3:
                    incoming += "=";
                    break;
            }

            return Convert.FromBase64String(incoming);
        }

        public static string HashMac(string s)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(s);
            var sha1 = SHA1.Create();
            byte[] hashBytes = sha1.ComputeHash(bytes);
            return HexStringFromBytes(hashBytes);
        }

        public static string HexStringFromBytes(byte[] bytes)
        {
            var sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                var hex = b.ToString("x2");
                sb.Append(hex);
            }

            return sb.ToString();
        }

    }
}