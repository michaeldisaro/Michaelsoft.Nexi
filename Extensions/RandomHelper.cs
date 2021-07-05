using System;
using System.Linq;
using System.Text;

namespace Michaelsoft.Nexi.Extensions
{
    public class RandomHelper
    {

        private static readonly int Seed = Environment.TickCount;

        private static readonly Random RandomNumberGenerator = new Random(Seed);

        public static string RandomNumberString(int min,
                                                int max)
        {
            return $"{RandomNumberGenerator.Next(min, max)}";
        }

        public static int RandomNumber(int max)
        {
            return RandomNumberGenerator.Next(max);
        }

        public static int RandomNumber(int min,
                                       int max)
        {
            return RandomNumberGenerator.Next(min, max);
        }

        public static string RandomString(int length,
                                          string symbols = "")
        {
            var groups = new[] {"ABCDEFGHIJKLMNOPQRSTUVWXYZ", "abcdefghijklmnopqrstuvwxyz", "0123456789", symbols};

            var sb = new StringBuilder();
            for (var i = 0; i < length; i++)
            {
                var group = groups[i % 4];
                if (IsNullOrEmpty(group)) group = groups[0];
                sb.Append(group.Split().Select(s => s[RandomHelper.RandomNumber(s.Length)]).FirstOrDefault());
            }

            return Shuffle(sb.ToString());
        }
        
        private static bool IsNullOrEmpty(string s)
        {
            return s == null || s.Trim().Equals("");
        }

        private static string Shuffle(string inputString)
        {
            var chars = inputString.ToCharArray().ToList();
            var sb = new StringBuilder();
            while (chars.Any())
            {
                var index = RandomNumber(chars.Count);
                var element = chars[index];
                sb.Append(element);
                chars.Remove(element);
            }

            return sb.ToString();
        }

    }
}