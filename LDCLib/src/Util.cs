using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LDCLib.src
{
    public class Util
    {
        public static string Extract(string input, string pattern)
        {
            var match = Regex.Match(input, pattern);
            if (match.Groups.Count < 2)
            {
                return "";
            }
            return match.Groups[1].Value;
        }
    }
}
