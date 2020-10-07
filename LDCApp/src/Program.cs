using LDCApp.src;
using LDCLib;
using LDCLib.src;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LDCApp
{
    class Program
    {
        string InDir = ".";
        string OutDir = "doc";
        readonly Parser Parser = new Parser();

        static void Log(object obj)
        {
            Console.WriteLine(obj);
        }

        void ParseFile(string fileName)
        {
            foreach (var line in File.ReadLines(fileName))
            {
                Parser.ParseLine(line);
            }
        }

        void ParseArgs(string[] args)
        {
            var argParser = new ArgParser();
            argParser.AddAlternative("inDir", (str) =>
            {
                InDir = str;
            });
            argParser.AddAlternative("outDir", (str) =>
            {
                OutDir = str;
            });
            foreach (var arg in args)
            {
                argParser.Parse(arg);
            }
        }

        void ParseInputFiles()
        {
            var files = Directory.GetFiles(InDir);
            foreach (var file in files)
            {
                var fileSuffix = Regex.Replace(file, "^.*\\.", "");
                if (fileSuffix != "lua")
                {
                    continue;
                }
                Log($"Reading file: {file}");
                ParseFile(file);
            }
        }

        void Run(string[] args)
        {
            ParseArgs(args);
            ParseInputFiles();
            WriteToOutFiles();
            Log("Done");
        }

        void WriteFunction(LuaType type, LuaFunction function, HTMLBuilder dest)
        {
            var id = $"{type.Name}:{function.Name}";
            dest.Open($"div id='{id}'");
            dest.Add("h4 class='functionheader'", $"a href='#{id}'", function.Name);
            dest.Add("p", function.Description);
            if (function.Parameters.Count > 1)
            {
                dest.Add("h4 class='funcparamheader'", "Parameters");
                dest.Open("ul class='funcparamlist'");
                foreach (var param in function.Parameters)
                {
                    if (param.Name == "self")
                    {
                        continue;
                    }
                    dest.Add("li", $"{param.TypeName} {param.Name} - {param.Description}");
                }
                dest.Close("ul");
            }
            if (function.ReturnType != null)
            {
                dest.Add("h4 class='funcreturnheader'", "Return");
                string returnLine = function.ReturnType;
                if (function.ReturnDescription.Length > 0)
                {
                    returnLine += $" - {function.ReturnDescription}";
                }
                dest.Add("ul", "li", $"{returnLine}");
            }
            dest.Close("div");
        }

        void WriteType(LuaType type, HTMLBuilder dest)
        {
            dest.Add("h3", $"Type {type.Name}");
            foreach (var function in type.Functions)
            {
                WriteFunction(type, function, dest);
            }
        }

        void WriteModule(LuaModule module)
        {
            string fileName;
            if (module.Name == "")
            {
                fileName = "root.html";
            }
            else
            {
                fileName = $"{module.Name}.html";
            }
            var builder = new HTMLBuilder();
            builder.Open("html", "head", "/head", "body");
            builder.Add("h1", module.Name);
            builder.Add("p", module.Description);
            builder.Add("h2", "Types");
            foreach (var type in module.LuaTypes.Values)
            {
                if (type.Name.Length == 0)
                {
                    continue;
                }
                builder.Add("p", type.Name);
            }
            foreach (var type in module.LuaTypes.Values)
            {
                if (type.Name.Length == 0)
                {
                    continue;
                }
                WriteType(type, builder);
            }
            builder.Close("body", "html");
            File.WriteAllText($"{OutDir}\\{fileName}", builder.ToString());
        }

        void WriteToOutFiles()
        {
            Directory.CreateDirectory(OutDir);
            foreach (var module in Parser.LuaModules.Values)
            {
                WriteModule(module);
            }
        }

        static void Main(string[] args)
        {
            Program p = new Program();
            p.Run(args);
        }
    }
}
