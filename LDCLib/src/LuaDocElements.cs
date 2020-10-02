using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LDCLib
{
    public class LuaModule
    {
        public readonly string Name;
        public readonly Dictionary<string, LuaType> LuaTypes = new Dictionary<string, LuaType>();
        public readonly Dictionary<string, LuaFunction> LuaFunctions = new Dictionary<string, LuaFunction>();

        public LuaModule(string name)
        {
            Name = name;
        }
    }

    public class LuaType
    {
        public LuaType SuperLuaType = null;
        public readonly List<LuaVariable> Fields = new List<LuaVariable>();
        public readonly List<LuaFunction> Functions = new List<LuaFunction>();
        public readonly string Name;

        public LuaType(string name)
        {
            Name = name;
        }
    }

    public class LuaVariable
    {
        public readonly LuaType Type;
        public readonly string Name;
        public readonly string Description;

        public LuaVariable(string name, LuaType type, string description)
        {
            Name = name;
            Type = type;
            Description = description;
        }
    }

    public class LuaFunction
    {
        public List<LuaVariable> Parameters = new List<LuaVariable>();
        public readonly string Name;
        public readonly LuaFunctionReturn Return;
        public readonly string Description = "";

        public LuaFunction(string name, string description, LuaFunctionReturn fReturn)
        {
            Name = name;
            Description = description;
            Return = fReturn;
        }
    }

    public class LuaFunctionReturn
    {
        public readonly LuaType Type;
        public readonly string Description;

        public LuaFunctionReturn(LuaType type, string description)
        {
            Type = type;
            Description = description;
        }
    }

}
