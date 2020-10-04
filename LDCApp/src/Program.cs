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
            DocBuilder db = new DocBuilder();
            db.Begin();
            db.AddModuleHeader(module.Name);
            db.AddMajorHeader("Types");
            var tb = new TableBuilder();
            tb.Begin();
            foreach (var type in module.LuaTypes.Values)
            {
                //tb.AddRow(type.Name, type.Description);
                tb.AddRow(type.Name, "");
            }
            tb.Finish();
            db.Add(tb.ToString());
            db.Finish();
            File.WriteAllText($"{OutDir}\\{fileName}", db.ToString());
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
