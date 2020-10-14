using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LDC
{
    class LDCMain
    {
        readonly LDCApp App = new LDCApp();

        void ParseArgs(string[] args)
        {
            var argParser = new ArgParser();
            argParser.AddAlternative("inDir", (str) =>
            {
                App.InDir = str;
            });
            argParser.AddAlternative("outDir", (str) =>
            {
                App.OutDir = str;
            });
            argParser.AddAlternative("overwritecss", (str) =>
            {
                App.OverwriteStylesheet = Util.ParseBoolString(str);
            });
            argParser.AddAlternative("inFile", (str) => {
                App.InFile = str;
            });
            foreach (var arg in args)
            {
                argParser.Parse(arg);
            }
        }

        void Run(string[] args)
        {
            ParseArgs(args);
            App.Execute();
        }

        static void Main(string[] args)
        {
            LDCMain app = new LDCMain();
            app.Run(args);
        }
    }
}
