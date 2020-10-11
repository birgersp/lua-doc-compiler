using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LDC
{
    [TestClass]
    public class ArgParserTest
    {
        [TestMethod]
        public void Parse()
        {
            var data = "";
            var argParser = new ArgParser();
            argParser.AddAlternative("foo", (str) =>
            {
                data = str;
            });
            argParser.Parse("foo=bar");
            Assert.AreEqual("bar", data);
        }
    }
}
