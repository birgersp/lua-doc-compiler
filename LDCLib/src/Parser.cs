using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LDC
{
    public class Parser
    {
        private string LFunRetTypeName, LFunRetDescription;
        public LuaType CurrentLuaType;
        public readonly Dictionary<string, LuaModule> LuaModules = new Dictionary<string, LuaModule>();
        private readonly List<LuaVariable> LuaFunctionParamBuffer = new List<LuaVariable>();
        public LuaModule CurrentModule;
        private readonly StringBuilder DescriptionBuffer = new StringBuilder();
        private readonly Dictionary<string, string> TypeAliases = new Dictionary<string, string>();

        public Parser()
        {
            LuaModules.Add("", new LuaModule("", ""));
            NewDoc();
        }

        private void FinishCurrent()
        {
            CurrentLuaType = null;
            LuaFunctionParamBuffer.Clear();
            DescriptionBuffer.Clear();
            LFunRetTypeName = null;
            LFunRetDescription = null;
        }

        public void NewDoc()
        {
            FinishCurrent();
            CurrentModule = LuaModules[""];
        }

        public void TryParseAsTypeAlias(string line)
        {
            if (CurrentLuaType == null)
            {
                return;
            }
            var varName = Util.Extract(line, "^(.*?)=").Trim();
            if (varName.Length == 0)
            {
                return;
            }
            TypeAliases.Add(varName, CurrentLuaType.Name);
        }

        public void ParseLine(string line)
        {
            var lineParser = new LineParser(line);

            if (lineParser.MatchNext("function"))
            {
                // Example:
                // function foobar(x)
                lineParser.SkipWhitespace();
                var fullName = Regex.Replace(lineParser.GetRest(), "\\(.*", "");
                string functionName;
                LuaType parentLuaType = null;
                if (fullName.Contains(":"))
                {
                    var split = fullName.Split(':');
                    var preSplit = split[0];
                    if (!TypeAliases.TryGetValue(preSplit, out string typeName))
                    {
                        typeName = $"#{preSplit}";
                    }
                    parentLuaType = GetLuaType(CurrentModule, typeName);
                    functionName = split[1];
                }
                else
                {
                    functionName = fullName;
                }
                var function = new LuaFunction(functionName, DescriptionBuffer.ToString(), LFunRetTypeName, LFunRetDescription);
                function.Parameters.AddRange(LuaFunctionParamBuffer);
                if (parentLuaType != null)
                {
                    parentLuaType.Functions.Add(function);
                }
                else
                {
                    CurrentModule.LuaFunctions.Add(function);
                }
                FinishCurrent();
                return;
            }

            if (!lineParser.MatchNext("--"))
            {
                TryParseAsTypeAlias(line);
                FinishCurrent();
                return;
            }
            lineParser.SkipNext('-');
            lineParser.SkipWhitespace();
            if (!lineParser.MatchNext('@'))
            {
                if (DescriptionBuffer.Length > 0)
                {
                    DescriptionBuffer.Append("\n");
                }
                DescriptionBuffer.Append(lineParser.GetRest());
                return;
            }

            string tag = lineParser.GetNextWord();

            if (tag == "module")
            {
                // Example:
                // -- @module MyModule
                string name = lineParser.GetNextWord();
                CurrentModule = GetLuaModule(name);
                FinishCurrent();
                return;
            }

            if (tag == "type")
            {
                // Example:
                // -- @type MyType
                string name = $"#{lineParser.GetNextWord()}";
                CurrentLuaType = GetLuaType(CurrentModule, name);
                return;
            }

            if (tag == "extends")
            {
                // Example:
                // -- @extends #SuperType
                if (CurrentLuaType == null)
                {
                    throw new Exception("'@extends' tag error: No type");
                }
                string moduleAndTypeName = lineParser.GetNextWord();
                CurrentLuaType.SuperTypeName = moduleAndTypeName;
            }

            if (tag == "field")
            {
                // Example:
                // -- @field #number x The X coordinate
                if (CurrentLuaType == null)
                {
                    throw new Exception("'$field' tag error: No parent type");
                }
                string moduleAndTypeName = lineParser.GetNextWord();
                var name = lineParser.GetNextWord();
                lineParser.SkipWhitespace();
                var description = lineParser.GetRest();
                LuaVariable variable = new LuaVariable(name, moduleAndTypeName, description);
                CurrentLuaType.Fields.Add(variable);
                return;
            }

            if (tag == "return")
            {
                // Example:
                // -- @return #MyType Some data
                string moduleAndTypeName = lineParser.GetNextWord();
                lineParser.SkipWhitespace();
                string description = lineParser.GetRest();
                LFunRetTypeName = moduleAndTypeName;
                LFunRetDescription = description;
                return;
            }

            if (tag == "param")
            {
                // Example:
                // -- @param #number x The input number
                string moduleAndTypeName = lineParser.GetNextWord();
                string name = lineParser.GetNextWord();
                lineParser.SkipWhitespace();
                string description = lineParser.GetRest();
                LuaVariable parameter = new LuaVariable(name, moduleAndTypeName, description);
                LuaFunctionParamBuffer.Add(parameter);
            }
        }

        private LuaModule GetLuaModule(string name)
        {
            if (!LuaModules.TryGetValue(name, out LuaModule module))
            {
                module = new LuaModule(name, DescriptionBuffer.ToString());
                LuaModules.Add(name, module);
            }
            return module;
        }

        private static LuaType GetLuaType(LuaModule module, string typeName)
        {
            if (!module.LuaTypes.TryGetValue(typeName, out LuaType lType))
            {
                lType = new LuaType(typeName);
                module.LuaTypes.Add(typeName, lType);
            }
            return lType;
        }
    }
}
