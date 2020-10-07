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

        void WriteFunction(LuaType type,LuaFunction function, StringBuilder dest)
        {
            var id = $"{type.Name}:{function.Name}";
            var inner = $"<a href=\"#{id}\">{function.Name}</a>\n";
            inner += $"<p>{function.Description}</p>\n";
            if (function.Parameters.Count > 1)
            {
                inner += $"<h4>Parameters</h4>\n";
                inner += $"<ul>\n";
                foreach (var param in function.Parameters)
                {
                    if (param.Name == "self")
                    {
                        continue;
                    }
                    inner += $"<li>{param.TypeName} {param.Name} - {param.Description}</li>\n";
                }
                inner += $"</ul>\n";
            }
            dest.Append($"<div id=\"{id}\">{inner}</div>\n");
        }

        void WriteType(LuaType type, StringBuilder dest)
        {
            dest.Append($"<h3>Type {type.Name}</h3>\n");
            foreach (var function in type.Functions)
            {
                WriteFunction(type,function, dest);
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
            StringBuilder data = new StringBuilder();
            data.Append($@"
<html><head></head><body>
<h1>{module.Name}</h1>
<p>{module.Description}</p>
<h2>Types</h2>
");
            foreach (var type in module.LuaTypes.Values)
            {
                if (type.Name.Length == 0)
                {
                    continue;
                }
                data.Append($"<p>{type.Name}</p>\n");
            }
            foreach (var type in module.LuaTypes.Values)
            {
                if (type.Name.Length == 0)
                {
                    continue;
                }
                WriteType(type, data);
            }
            data.Append("</body></html>\n");
            File.WriteAllText($"{OutDir}\\{fileName}", data.ToString());
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
