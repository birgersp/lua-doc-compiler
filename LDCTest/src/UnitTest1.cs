using System;
using LDCLib.src;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LDCTest.src
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
