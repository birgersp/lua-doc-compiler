using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LDCLib.src
{
    public class ArgParser
    {
        readonly Dictionary<string, Action<string>> Callbacks = new Dictionary<string, Action<string>>();

        public void AddAlternative(string key, Action<string> callback)
        {
            Callbacks.Add(key, callback);
        }

        public void AddAlternative(string key, Action callback)
        {
            AddAlternative(key, (str) =>
            {
                callback();
            });
        }

        public void Parse(string argument)
        {
            var delimiterIndex = argument.IndexOf("=");
            string key;
            string value;
            if (delimiterIndex == -1)
            {
                key = argument;
                value = "";
            }
            else
            {
                key = argument.Substring(0, delimiterIndex);
                value = argument.Substring(delimiterIndex + 1);
            }
            Callbacks[key](value);
        }
    }
}
