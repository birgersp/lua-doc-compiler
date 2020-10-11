using System.Collections.Generic;

namespace LDC
{
    public class LuaModule
    {
        public readonly string Name;
        public readonly string Description;
        public readonly Dictionary<string, LuaType> LuaTypes = new Dictionary<string, LuaType>();
        public readonly List<LuaFunction> LuaFunctions = new List<LuaFunction>();

        public LuaModule(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }

    public class LuaType
    {
        public string SuperTypeName = null;
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
        public readonly string TypeName;
        public readonly string Name;
        public readonly string Description;

        public LuaVariable(string name, string typeName, string description)
        {
            Name = name;
            TypeName = typeName;
            Description = description;
        }
    }

    public class LuaFunction
    {
        public List<LuaVariable> Parameters = new List<LuaVariable>();
        public readonly string Name;
        public readonly string Description = "";
        public readonly string ReturnType;
        public readonly string ReturnDescription;

        public LuaFunction(string name, string description, string returnType, string returnDescription)
        {
            Name = name;
            Description = description;
            ReturnType = returnType;
            ReturnDescription = returnDescription;
        }
    }
}
