using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LDCLib
{
    public class LModule
    {
        public readonly string Name;
        public readonly Dictionary<string, LType> LTypes = new Dictionary<string, LType>();

        public LModule(string name)
        {
            Name = name;
        }
    }

    public class LType
    {
        public LType SuperLType = null;
        public readonly List<LVariable> Fields = new List<LVariable>();
        public readonly List<LFunction> Functions = new List<LFunction>();
        public readonly string Name;

        public LType(string name)
        {
            Name = name;
        }
    }

    public class LVariable
    {
        public readonly LType Type;
        public readonly string Name;
        public readonly string Description;

        public LVariable(string name, LType type, string description)
        {
            Name = name;
            Type = type;
            Description = description;
        }
    }

    public class LFunction
    {
        public List<LVariable> Parameters = new List<LVariable>();
        public readonly string Name;
        public readonly LFunctionReturn Return;
        public readonly string Description = "";

        public LFunction(string name, string description, LFunctionReturn fReturn)
        {
            Name = name;
            Description = description;
            Return = fReturn;
        }
    }

    public class LFunctionReturn
    {
        public readonly LType Type;
        public readonly string Description;

        public LFunctionReturn(LType type, string description)
        {
            Type = type;
            Description = description;
        }
    }

}
