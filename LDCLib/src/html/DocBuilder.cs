using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LDCLib.src
{
    public class DocBuilder
    {
        private StringBuilder sb = new StringBuilder();

        public DocBuilder()
        {
            sb.Append("<html>");
        }

        public void AddModuleHeader(string name)
        {
            sb.Append($"<h1>Module {name}</h1>\n");
        }

        public void Begin()
        {
            sb.Append("<html><head></head><body>\n");
        }

        public void AddMajorHeader(string text)
        {
            sb.Append($"<h2>{text}</h2>\n");
        }

        public void AddMinorHeader(string text)
        {
            sb.Append($"<h3>{text}</h3>\n");
        }

        public void Finish()
        {
            sb.Append("</body>\n");
        }

        public override string ToString()
        {
            return sb.ToString();
        }

        public void Add(string text)
        {
            sb.Append(text);
        }
    }
}
