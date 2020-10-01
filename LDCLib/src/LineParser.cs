using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LDCLib
{
    public class LineParser
    {
        public readonly string Data;

        public int Index { get; private set; } = 0;

        public LineParser(string data)
        {
            this.Data = data;
        }

        public bool MatchNext(char target)
        {
            if (Index >= Data.Length)
            {
                return false;
            }
            if (Data[Index].Equals(target))
            {
                Index++;
                return true;
            }
            return false;
        }

        public bool MatchNext(string target)
        {
            var newIndex = target.Length + Index;
            if (newIndex > Data.Length)
            {
                return false;
            }
            if (target.Equals(Data.Substring(Index, target.Length)))
            {
                Index = newIndex;
                return true;
            }
            return false;
        }

        public void SkipNext(params char[] symbols)
        {
            while (Index < Data.Length &&
                Array.Exists(symbols, Data[Index].Equals))
            {
                Index++;
            }
        }

        public void SkipWhitespace()
        {
            SkipNext(' ', '\t');
        }

        public string GetNextWord()
        {
            SkipWhitespace();
            if (Index >= Data.Length)
            {
                return "";
            }
            var i1 = Index;
            SkipTo(' ', '\t');
            return Data.Substring(i1, Index - i1);
        }

        public void SkipTo(params char[] symbols)
        {
            while (Index < Data.Length &&
                !Array.Exists(symbols, Data[Index].Equals))
            {
                Index++;
            }
        }

        public string GetRest()
        {
            if (Index >= Data.Length)
            {
                return "";
            }
            return Data.Substring(Index);
        }
    }

}
