using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LDCApp.src
{
    public class HTMLBuilder
    {
        readonly StringBuilder data = new StringBuilder();

        private static string ToLiteral(string text)
        {
            return text
                .Replace("<", "&lt;")
                .Replace(">", "&gt;");
        }

        public void AddLiteral(string text)
        {
            var translated = ToLiteral(text);
            data.Append(translated);
        }

        public void Add(params string[] entries)
        {
            if (entries.Length == 1)
            {
                AddLiteral(entries[0]);
                data.Append("\n");
                return;
            }
            if (entries.Length > 1)
            {
                var index = 0;
                while (index < entries.Length - 1)
                {
                    Open(entries[index++]);
                }
                AddLiteral(entries.Last());
                index--;
                while(index >= 0)
                {
                    var tag = Regex.Replace(entries[index--], " .*", "");
                    Close(tag);
                }
                data.Append("\n");
                return;
            }
        }

        public void Open(params string[] tags)
        {
            foreach (var tag in tags)
            {
                data.Append($"<{tag}>");
            }
        }

        public void Close(params string[] tags)
        {
            foreach (var tag in tags)
            {
                data.Append($"</{tag}>");
            }
        }

        public override string ToString()
        {
            return data.ToString();
        }
    }
}
