using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LDC
{
    [TestClass]
    public class UtilTest
    {
        [TestMethod]
        public void Extract()
        {
            Assert.AreEqual("", Util.Extract("foo(bar)", "<(.*)>"));
            Assert.AreEqual("bar", Util.Extract("foo<bar>", "<(.*)>"));
            Assert.AreEqual("", Util.Extract("#list<#number>", "(^[a-z|A-Z|0-9]{1,}?)#"));
            Assert.AreEqual("foo", Util.Extract("foo#list<#number>", "(^[a-z|A-Z|0-9]{1,}?)#"));
            Assert.AreEqual("f123F1", Util.Extract("f123F1#list<#number>", "(^[a-z|A-Z|0-9]{1,}?)#"));
            Assert.AreEqual("list<#number>", Util.Extract("#list<#number>", "^.*?#(.*)"));
        }

        [TestMethod]
        public void SortAlphabetically()
        {
            string[] strings = { "b", "a", "c" };
            Util.SortAlphabetically(strings);
            Assert.AreEqual("a", strings[0]);
            Assert.AreEqual("b", strings[1]);
            Assert.AreEqual("c", strings[2]);
        }
    }
}
