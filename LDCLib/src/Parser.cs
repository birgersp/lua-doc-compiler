using LDCLib.src;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LDCLib
{
    public class Parser
    {
        private LuaFunctionReturn CurrentLuaFunctionReturn;
        public LuaType CurrentLuaType;
        public readonly Dictionary<string, LuaModule> LuaModules = new Dictionary<string, LuaModule>();
        private readonly List<LuaVariable> LuaFunctionParamBuffer = new List<LuaVariable>();
        public LuaModule CurrentModule;
        private readonly StringBuilder DescriptionBuffer = new StringBuilder();

        public Parser()
        {
            LuaModules.Add("", new LuaModule(""));
            NewDoc();
        }

        private void FinishCurrent()
        {
            CurrentLuaType = null;
            LuaFunctionParamBuffer.Clear();
            DescriptionBuffer.Clear();
        }

        public void NewDoc()
        {
            FinishCurrent();
            CurrentModule = LuaModules[""];
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
                    parentLuaType = GetLuaType(CurrentModule, split[0]);
                    functionName = split[1];
                }
                else
                {
                    functionName = fullName;
                }
                var function = new LuaFunction(functionName, DescriptionBuffer.ToString(), CurrentLuaFunctionReturn);
                function.Parameters.AddRange(LuaFunctionParamBuffer);
                if (parentLuaType != null)
                {
                    parentLuaType.Functions.Add(function);
                } else
                {
                    CurrentModule.LuaFunctions.Add(functionName, function);
                }
                FinishCurrent();
                return;
            }

            if (!lineParser.MatchNext("--"))
            {
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

            if (tag == "type")
            {
                // Example:
                // -- @type MyType
                string name = $"{CurrentModule.Name}#{lineParser.GetNextWord()}";
                CurrentLuaType = GetLuaType(CurrentModule, name);
                return;
            }

            if (tag == "extends")
            {
                // Example:
                // -- @extends #SuperType
                if (CurrentLuaType == null)
                {
                    throw new Exception("Extends tag error: No type");
                }
                string moduleAndTypeName = lineParser.GetNextWord();
                GetModuleName(moduleAndTypeName, out string moduleName);
                LuaModule luaModule = GetLuaModule(moduleName);
                CurrentLuaType.SuperLuaType = GetLuaType(luaModule, moduleAndTypeName);
            }

            if (tag == "field")
            {
                // Example:
                // -- @field #number x The X coordinate
                if (CurrentLuaType == null)
                {
                    throw new Exception("Field tag error: No parent type");
                }
                string moduleAndTypeName = lineParser.GetNextWord();
                GetModuleName(moduleAndTypeName, out string moduleName);
                LuaModule luaModule = GetLuaModule(moduleName);
                LuaType type = GetLuaType(luaModule, moduleAndTypeName);
                var name = lineParser.GetNextWord();
                lineParser.SkipWhitespace();
                var description = lineParser.GetRest();
                LuaVariable lVariable = new LuaVariable(name, type, description);
                CurrentLuaType.Fields.Add(lVariable);
                return;
            }

            if (tag == "return")
            {
                // Example:
                // -- @return #MyType Some data
                string moduleAndTypeName = lineParser.GetNextWord();
                GetModuleName(moduleAndTypeName, out string moduleName);
                lineParser.SkipWhitespace();
                string description = lineParser.GetRest();
                LuaModule module = GetLuaModule(moduleName);
                LuaType type = GetLuaType(module, moduleAndTypeName);
                CurrentLuaFunctionReturn = new LuaFunctionReturn(type, description);
                return;
            }

            if (tag == "param")
            {
                // Example:
                // -- @param #number x The input number
                string moduleAndTypeName = lineParser.GetNextWord();
                GetModuleName(moduleAndTypeName, out string moduleName);
                string name = lineParser.GetNextWord();
                lineParser.SkipWhitespace();
                string description = lineParser.GetRest();
                LuaModule module = GetLuaModule(moduleName);
                LuaType type = GetLuaType(module, moduleAndTypeName);
                LuaVariable parameter = new LuaVariable(name, type, description);
                LuaFunctionParamBuffer.Add(parameter);
            }
        }

        private LuaModule GetLuaModule(string name)
        {
            if (!LuaModules.TryGetValue(name, out LuaModule module))
            {
                module = new LuaModule(name);
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

        private static void GetModuleName(string moduleAndTypeName, out string moduleName)
        {
            if (moduleAndTypeName.Contains("#"))
            {
                var split = moduleAndTypeName.Split('#');
                moduleName = split[0];
            }
            else
            {
                moduleName = "";
            }
        }
    }
}
