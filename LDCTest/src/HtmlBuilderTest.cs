using System;
using LDCApp.src;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LDCTest.src
{
    [TestClass]
    public class HtmlBuilderTest
    {
        [TestMethod]
        public void Add()
        {
            var builder = new HtmlBuilder();
            builder.Add("a", "b");
            Assert.AreEqual("<a>b</a>\n", builder.ToString());
            builder.Add("c");
            Assert.AreEqual("<a>b</a>\nc\n", builder.ToString());
        }
    }
}
