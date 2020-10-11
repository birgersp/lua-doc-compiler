using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LDC
{
    public class TableBuilder
    {
        readonly StringBuilder SB = new StringBuilder();

        public void Begin()
        {
            SB.Append("<table>\n");
        }

        public void AddRow(params string[] cols)
        {
            SB.Append("<tr>\n");
            foreach (var col in cols)
            {
                SB.Append($"<td>{col}</td>\n");
            }
            SB.Append("</tr>\n");
        }

        public void Finish()
        {
            SB.Append("</table>\n");
        }

        public override string ToString()
        {
            return SB.ToString();
        }
    }
}
