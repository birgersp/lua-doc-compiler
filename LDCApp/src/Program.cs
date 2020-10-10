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
        static bool ModuleHasContent(LuaModule module)
        {
            return module.LuaFunctions.Count > 0 || module.LuaTypes.Count > 0;
        }

        static void Log(object obj)
        {
            Console.WriteLine(obj);
        }

        static string GetModuleHTMLFilename(LuaModule module)
        {
            if (module.Name == "")
            {
                return "root.html";
            }
            else
            {
                return $"{module.Name}.html";
            }
        }

        string InDir = ".";
        string OutDir = "doc";
        bool OverwriteStylesheet = false;
        readonly Parser Parser = new Parser();

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
            argParser.AddAlternative("overwritecss", (str) =>
            {
                OverwriteStylesheet = Util.ParseBoolString(str);
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
            try
            {
                File.Copy("style.css", $"{OutDir}\\style.css", OverwriteStylesheet);
            }
            catch (IOException e)
            {
                Log(e.Message);
            }
            Log("Done");
        }

        void WriteFunction(LuaFunction function, HTMLBuilder dest, string prefix = "")
        {
            var id = $"{prefix}{function.Name}";
            dest.Open($"div id='{id}'");
            dest.Add("hr", "");
            dest.Add("h4 class='functionheader'", $"a href='#{id}'", $"{prefix}{function.Name}");
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
            dest.Add("h3 class='typeheader'", $"Type {type.Name}");
            foreach (var function in type.Functions)
            {
                WriteFunction(function, dest, $"{type.Name}:");
            }
        }

        void WriteModule(LuaModule module, HTMLBuilder builder)
        {
            builder.Add("h1", module.Name);
            builder.Add("p", module.Description);
            builder.Add("h2", "Types");
            foreach (var type in module.LuaTypes.Values)
            {
                if (type.Name.Length == 0 ||
                    (type.Fields.Count == 0 && type.Functions.Count == 0))
                {
                    continue;
                }
                builder.Add("p", type.Name);
            }
            foreach (var function in module.LuaFunctions)
            {
                WriteFunction(function, builder);
            }
            foreach (var type in module.LuaTypes.Values)
            {
                if (type.Name.Length == 0 ||
                    (type.Fields.Count == 0 && type.Functions.Count == 0))
                {
                    continue;
                }
                WriteType(type, builder);
            }
        }

        string GetTemplateWithMenu()
        {
            var menuBuilder = new HTMLBuilder();
            foreach (var module in Parser.LuaModules.Values)
            {
                if (!ModuleHasContent(module))
                {
                    continue;
                }
                var moduleHtmlName = GetModuleHTMLFilename(module);
                menuBuilder.Add("h3 class='modulelink'", $"a href='{moduleHtmlName}'", module.Name);
            }
            var templateData = File.ReadAllText("template.html");
            return templateData.Replace("@@MODULES@@", menuBuilder.ToString());
        }

        void WriteToOutFiles()
        {
            var templateData = GetTemplateWithMenu();
            Directory.CreateDirectory(OutDir);
            foreach (var module in Parser.LuaModules.Values)
            {
                if (!ModuleHasContent(module))
                {
                    continue;
                }
                string fileName = GetModuleHTMLFilename(module);
                var builder = new HTMLBuilder();
                WriteModule(module, builder);
                var contentData = templateData.Replace("@@CONTENT@@", builder.ToString());
                File.WriteAllText($"{OutDir}\\{fileName}", contentData.ToString());
            }
        }

        static void Main(string[] args)
        {
            Program p = new Program();
            p.Run(args);
        }
    }
}
