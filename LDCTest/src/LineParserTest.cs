using LDC;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LDC
{
    [TestClass]
    public class LineParserTest
    {
        [TestMethod]
        public void TestMatchNext()
        {
            var lp = new LineParser("");
            Assert.IsFalse(lp.MatchNext(' '));
            lp = new LineParser("Hello");
            Assert.IsTrue(lp.MatchNext('H'));
            Assert.IsTrue(lp.MatchNext('e'));
            Assert.IsTrue(lp.MatchNext('l'));
            Assert.IsTrue(lp.MatchNext('l'));
            Assert.IsTrue(lp.MatchNext('o'));
            Assert.IsFalse(lp.MatchNext('!'));
        }


        [TestMethod]
        public void TestMatchNextString()
        {
            var lp = new LineParser("");
            Assert.IsFalse(lp.MatchNext("Hello"));
            lp = new LineParser("Hello world");
            Assert.IsTrue(lp.MatchNext("Hello"));
            Assert.IsTrue(lp.MatchNext(" wo"));
            Assert.IsTrue(lp.MatchNext("rld"));
            Assert.IsFalse(lp.MatchNext(" "));
        }

        [TestMethod]
        public void TestSkipNext()
        {
            var lp = new LineParser("Hello world\ttest");
            lp.MatchNext("Hello");
            Assert.IsFalse(lp.MatchNext("world"));
            lp.SkipNext(' ');
            Assert.IsTrue(lp.MatchNext("world"));
            lp.SkipNext(' ', '\t');
            Assert.IsTrue(lp.MatchNext("test"));
        }

        [TestMethod]
        public void TestSkipTo()
        {
            var lp = new LineParser("Hello world\ttest");
            lp.SkipTo(' ');
            Assert.IsFalse(lp.MatchNext("world"));
        }

        [TestMethod]
        public void TestGetNextWord()
        {
            var lp = new LineParser("Hello   world\t  \t  test");
            Assert.AreEqual("Hello", lp.GetNextWord());
            Assert.AreEqual("world", lp.GetNextWord());
            Assert.AreEqual("test", lp.GetNextWord());
        }

        [TestMethod]
        public void TestGetRest()
        {
            var lp = new LineParser("Hello   world");
            lp.SkipTo('w');
            Assert.AreEqual("world", lp.GetRest());
        }
    }
}
