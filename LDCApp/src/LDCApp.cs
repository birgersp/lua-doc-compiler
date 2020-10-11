using LDC;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LDC
{
    public class LDCApp
    {
        static bool ModuleHasContent(LuaModule module)
        {
            return module.LuaFunctions.Count > 0 || module.LuaTypes.Count > 0;
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

        public string InDir = ".";
        public string OutDir = "doc";
        public bool OverwriteStylesheet = false;
        readonly Parser Parser = new Parser();
        readonly SortedDictionary<string, LuaModule> IncludedModules = new SortedDictionary<string, LuaModule>();

        public void Execute()
        {
            ParseInputFiles();
            WriteToOutFiles();
            try
            {
                File.Copy("style.css", $"{OutDir}\\style.css", OverwriteStylesheet);
            }
            catch (IOException e)
            {
                Util.Log(e.Message);
            }
            Util.Log("Done");
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
                Util.Log($"Reading file: {file}");
                ParseFile(file);
            }
        }

        void ParseFile(string fileName)
        {
            foreach (var line in File.ReadLines(fileName))
            {
                Parser.ParseLine(line);
            }
        }

        void WriteToOutFiles()
        {
            foreach (var module in Parser.LuaModules.Values)
            {
                if (!ModuleHasContent(module))
                {
                    continue;
                }
                IncludedModules.Add(module.Name, module);
            }
            var templateData = GetTemplateWithMenu();
            Directory.CreateDirectory(OutDir);
            foreach (var module in IncludedModules.Values)
            {
                string fileName = GetModuleHTMLFilename(module);
                var builder = new HtmlBuilder();
                WriteModule(module, builder);
                var contentData = templateData.Replace("@@CONTENT@@", builder.ToString());
                File.WriteAllText($"{OutDir}\\{fileName}", contentData.ToString());
            }
        }

        string GetTemplateWithMenu()
        {
            var menuBuilder = new HtmlBuilder();
            foreach (var module in IncludedModules.Values)
            {
                var moduleHtmlName = GetModuleHTMLFilename(module);
                menuBuilder.Add("h3 class='modulelink'", $"a href='{moduleHtmlName}'", module.Name);
            }
            var templateData = File.ReadAllText("template.html");
            return templateData.Replace("@@MODULES@@", menuBuilder.ToString());
        }

        void WriteModule(LuaModule module, HtmlBuilder html)
        {
            var htmlDocName = GetModuleHTMLFilename(module);
            html.Add("h1", module.Name);
            if (module.Description.Length > 0)
            {
                html.Add("p", module.Description);
            }
            SortedDictionary<string, LuaType> includedTypes = new SortedDictionary<string, LuaType>();
            foreach (var type in module.LuaTypes.Values)
            {
                if (type.Name.Length == 0 ||
                    (type.Fields.Count == 0 && type.Functions.Count == 0))
                {
                    continue;
                }
                includedTypes.Add(type.Name, type);
            }
            if (includedTypes.Count > 0)
            {
                html.Add("h2", "Types");
                html.Open("ul");
                foreach (var type in includedTypes.Values)
                {
                    html.Add("li", $"a href='{htmlDocName}#{type.Name}'", type.Name);
                }
                html.Close("ul");
            }
            var functions = Util.ArrayToSortedDict(module.LuaFunctions, f => f.Name);
            if (functions.Count > 0)
            {
                html.Add("h2", "Functions");
                WriteFunctionTable(functions.Values, html, $"{htmlDocName}#");
                foreach (var function in functions.Values)
                {
                    WriteFunction(function, html, function.Name, GetModuleHTMLFilename(module));
                }
            }
            foreach (var type in includedTypes.Values)
            {
                WriteType(module, type, html);
            }
        }

        void WriteType(LuaModule module, LuaType type, HtmlBuilder html)
        {
            var htmlDocName = GetModuleHTMLFilename(module);

            html.Add($"h3 class='typeheader' id='{type.Name}'", $"Type {type.Name}");
            var functions = Util.ArrayToSorted(type.Functions, f => f.Name);
            WriteFunctionTable(functions.Values, html, $"{htmlDocName}#{type.Name}:");
            foreach (var function in functions.Values)
            {
                WriteFunction(function, html, $"{type.Name}:{function.Name}", htmlDocName);
            }
        }

        void WriteFunctionTable(IEnumerable<LuaFunction> functions, HtmlBuilder html, string linkPrefix)
        {
            html.Open("table", "tbody");
            html.Open("tr");
            html.Add("td", "p class='cellheader'", "Name");
            html.Add("td class='funcdesccell'", "p class='cellheader'", "Description");
            html.Close("tr");
            foreach (var function in functions)
            {
                var link = $"{linkPrefix}{function.Name}";
                html.Open("tr");
                html.Add("td", "p class='cell'", $"a href='{link}'", function.Name);
                html.Add("td class='funcdesccell'", "p class='cell'", function.Description);
                html.Close("tr");
            }
            html.Close("tbody", "table");
        }

        void WriteFunction(LuaFunction function, HtmlBuilder html, string name, string htmlDocName)
        {
            html.Open($"div id='{name}'");
            html.Add("h4 class='functionheader'", $"a href='{htmlDocName}#{name}'", name);
            if (function.Description.Length > 0)
            {
                html.Add("p", function.Description);
            }
            if (function.Parameters.Count > 1)
            {
                WriteFunctionParams(function, html);
            }
            if (function.ReturnType != null)
            {
                html.Add("h4 class='funcreturnheader'", "Return");
                string returnLine = function.ReturnType;
                if (function.ReturnDescription.Length > 0)
                {
                    returnLine += $" - {function.ReturnDescription}";
                }
                html.Add("ul", "li", $"{returnLine}");
            }
            html.Open("hr");
            html.Close("div");
        }

        void WriteFunctionParams(LuaFunction function, HtmlBuilder html)
        {
            html.Add("h4 class='funcparamheader'", "Parameters");
            html.Open("ul class='funcparamlist'");
            foreach (var param in function.Parameters)
            {
                if (param.Name == "self")
                {
                    continue;
                }
                html.Add("li", $"{param.TypeName} {param.Name} - {param.Description}");
            }
            html.Close("ul");
        }
    }
}
