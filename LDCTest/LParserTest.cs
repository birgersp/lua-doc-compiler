using LDCLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LDCTest
{
    [TestClass]
    public class LParserTest
    {
        [TestMethod]
        public void TestParseLType()
        {
            var parser = new LParser();

            // Parse a type
            Assert.IsTrue(parser.CurrentLType == null);
            parser.ParseLine("-- @type MyType");
            Assert.IsTrue(parser.CurrentLType != null);
            LType lType = parser.CurrentLType;
            Assert.AreEqual("MyType", lType.Name);

            // Parse type super type
            Assert.IsTrue(lType.SuperLType == null);
            parser.ParseLine("-- @extends #SuperType");
            Assert.IsTrue(lType.SuperLType != null);
            Assert.AreEqual("SuperType", lType.SuperLType.Name);

            // Parse type field (variable)
            Assert.AreEqual(0, lType.Fields.Count);
            parser.ParseLine("-- @field #number x The X coordinate");
            Assert.AreEqual(1, lType.Fields.Count);

            // Check type field variable
            LVariable lVariable = lType.Fields[0];
            Assert.AreEqual("x", lVariable.Name);
            Assert.AreEqual("The X coordinate", lVariable.Description);
            LType numberLType = lVariable.Type;
            Assert.AreEqual("number", numberLType.Name);
            Assert.AreEqual(1, lType.Fields.Count);
            Assert.AreEqual(0, lType.Functions.Count);
        }

        [TestMethod]
        public void TestParseFunction()
        {
            var parser = new LParser();
            Assert.IsTrue(parser.CurrentLFunction == null);
            parser.ParseLine("-- Some function");
            parser.ParseLine("-- @param #number x The input number");
            parser.ParseLine("-- @return #MyType Some data");
            parser.ParseLine("function foobar(x)");
            Assert.IsTrue(parser.CurrentLFunction != null);
            LFunction lFunction = parser.CurrentLFunction;
            Assert.AreEqual("foobar", lFunction.Name);
            Assert.AreEqual("Some function", lFunction.Description);
            LFunctionReturn lFunctionReturn = lFunction.Return;
            Assert.AreEqual("MyType", lFunctionReturn.Type.Name);
        }
    }
}
