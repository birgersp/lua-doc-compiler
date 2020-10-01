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
        public LFunction CurrentLFunction;
        private LFunctionReturn CurrentLFunctionReturn;
        public LType CurrentLType;
        public readonly Dictionary<string, LModule> LModules = new Dictionary<string, LModule>();
        private readonly List<LVariable> lFunctionParamBuffer = new List<LVariable>();
        public LModule CurrentModule;
        private StringBuilder DescriptionBuffer = new StringBuilder();

        public Parser()
        {
            LModules.Add("", new LModule(""));
            NewDoc();
        }

        private void FinishCurrent()
        {
            CurrentLFunction = null;
            CurrentLType = null;
            lFunctionParamBuffer.Clear();
            DescriptionBuffer.Clear();
        }

        public void NewDoc()
        {
            FinishCurrent();
            CurrentModule = LModules[""];
        }

        public void ParseLine(string line)
        {
            var lineParser = new LineParser(line);

            if (lineParser.MatchNext("function"))
            {
                lineParser.SkipWhitespace();
                var fullName = Regex.Replace(lineParser.GetRest(), "\\(.*", "");
                string lFunctionName;
                LType parentLType = null;
                if (fullName.Contains(":"))
                {
                    var split = fullName.Split(':');
                    parentLType = GetLType(CurrentModule, split[0]);
                    lFunctionName = split[1];
                }
                else
                {
                    lFunctionName = fullName;
                }
                CurrentLFunction = new LFunction(lFunctionName, DescriptionBuffer.ToString(), CurrentLFunctionReturn);
                if (parentLType != null)
                {
                    parentLType.Functions.Add(CurrentLFunction);
                }
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
                string name = lineParser.GetNextWord();
                CurrentLType = GetLType(CurrentModule, name);
                return;
            }

            if (tag == "extends")
            {
                // Example:
                // -- @extends #SuperType
                if (CurrentLType == null)
                {
                    throw new Exception("Extends tag error: No type");
                }
                string moduleAndTypeName = lineParser.GetNextWord();
                SplitTypeName(moduleAndTypeName, out string moduleName, out string typeName);
                LModule lModule = GetLModule(moduleName);
                CurrentLType.SuperLType = GetLType(lModule, typeName);
            }

            if (tag == "field")
            {
                // Example:
                // -- @field #number x The X coordinate
                if (CurrentLType == null)
                {
                    throw new Exception("Field tag error: No parent type");
                }
                SplitTypeName(lineParser.GetNextWord(), out string moduleName, out string typeName);
                LModule lModule = GetLModule(moduleName);
                LType lType = GetLType(lModule, typeName);
                var name = lineParser.GetNextWord();
                lineParser.SkipWhitespace();
                var description = lineParser.GetRest();
                LVariable lVariable = new LVariable(name, lType, description);
                CurrentLType.Fields.Add(lVariable);
                return;
            }

            if (tag == "return")
            {
                // Example:
                // -- @return #MyType Some data
                string moduleAndTypeName = lineParser.GetNextWord();
                SplitTypeName(moduleAndTypeName, out string moduleName, out string typeName);
                lineParser.SkipWhitespace();
                string description = lineParser.GetRest();
                LModule lModule = GetLModule(moduleName);
                LType lType = GetLType(lModule, typeName);
                CurrentLFunctionReturn = new LFunctionReturn(lType, description);
                return;
            }
        }

        private LModule GetLModule(string name)
        {
            LModule lModule;
            if (!LModules.TryGetValue(name, out lModule))
            {
                lModule = new LModule(name);
                LModules.Add(name, lModule);
            }
            return lModule;
        }

        private static LType GetLType(LModule lModule, string typeName)
        {
            LType lType;
            if (!lModule.LTypes.TryGetValue(typeName, out lType))
            {
                lType = new LType(typeName);
                lModule.LTypes.Add(typeName, lType);
            }
            return lType;
        }

        private static void SplitTypeName(string moduleAndTypeName, out string moduleName, out string typeName)
        {
            if (moduleAndTypeName.Contains("#"))
            {
                var split = moduleAndTypeName.Split('#');
                moduleName = split[0];
                typeName = split[1];
            }
            else
            {
                moduleName = "";
                typeName = moduleAndTypeName;
            }
        }
    }

}
