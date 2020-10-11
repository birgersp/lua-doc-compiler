using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LDC
{
    public class Util
    {
        static readonly string[] BoolStringAlternatives = { "1", "true", "yes" };

        public static string Extract(string input, string pattern)
        {
            var match = Regex.Match(input, pattern);
            if (match.Groups.Count < 2)
            {
                return "";
            }
            return match.Groups[1].Value;
        }

        public static bool ParseBoolString(string input)
        {
            var lowercase = input.ToLower();
            return BoolStringAlternatives.Any(str => str.Equals(lowercase));
        }

        public static void Log(object obj)
        {
            Console.WriteLine(obj);
        }

        public static void SortAlphabetically(string[] strings)
        {
            Array.Sort(strings, (s1, s2) => String.Compare(s1, s2));
        }

        public static SortedDictionary<string, T> ArrayToSortedDict<T>(IEnumerable<T> items, Func<T, string> extractor)
        {
            var result = new SortedDictionary<string, T>();
            foreach (var item in items)
            {
                result.Add(extractor(item), item);
            }
            return result;
        }

        public static SortedList<string, T> ArrayToSorted<T>(IEnumerable<T> items, Func<T, string> extractor)
        {
            var result = new SortedList<string, T>();
            foreach (var item in items)
            {
                result.Add(extractor(item), item);
            }
            return result;
        }
    }
}
