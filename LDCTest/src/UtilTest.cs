using LDCLib.src;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LDCTest.src
{
    [TestClass]
    public class UtilTest
    {
        [TestMethod]
        public void TestParseLType()
        {
            Assert.AreEqual("", Util.Extract("foo(bar)", "<(.*)>"));
            Assert.AreEqual("bar", Util.Extract("foo<bar>", "<(.*)>"));
            Assert.AreEqual("", Util.Extract("#list<#number>", "(^[a-z|A-Z|0-9]{1,}?)#"));
            Assert.AreEqual("foo", Util.Extract("foo#list<#number>", "(^[a-z|A-Z|0-9]{1,}?)#"));
            Assert.AreEqual("f123F1", Util.Extract("f123F1#list<#number>", "(^[a-z|A-Z|0-9]{1,}?)#"));
            Assert.AreEqual("list<#number>", Util.Extract("#list<#number>", "^.*?#(.*)"));
        }
    }
}
