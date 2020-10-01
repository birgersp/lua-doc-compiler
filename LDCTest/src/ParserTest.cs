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
    public class ParserTest
    {
        [TestMethod]
        public void TestParseType()
        {
            var parser = new Parser();
            Assert.IsTrue(parser.CurrentLuaType == null);
            parser.ParseLine("-- @type MyType");
            Assert.IsTrue(parser.CurrentLuaType != null);
            LuaType lType = parser.CurrentLuaType;
            Assert.AreEqual("MyType", lType.Name);
        }

        [TestMethod]
        public void TestParseSuperType()
        {
            var parser = new Parser();
            parser.ParseLine("-- @type MyType");
            LuaType lType = parser.CurrentLuaType;
            Assert.IsTrue(lType.SuperLuaType == null);
            parser.ParseLine("-- @extends #SuperType");
            Assert.IsTrue(lType.SuperLuaType != null);
            Assert.AreEqual("SuperType", lType.SuperLuaType.Name);
        }

        [TestMethod]
        public void TestTypeField()
        {
            var parser = new Parser();
            parser.ParseLine("-- @type MyType");
            LuaType lType = parser.CurrentLuaType;
            Assert.AreEqual(0, lType.Fields.Count);
            parser.ParseLine("-- @field #number x The X coordinate");
            Assert.AreEqual(1, lType.Fields.Count);
            LuaVariable lVariable = lType.Fields[0];
            Assert.AreEqual("x", lVariable.Name);
            Assert.AreEqual("The X coordinate", lVariable.Description);
            LuaType numberLType = lVariable.Type;
            Assert.AreEqual("number", numberLType.Name);
            Assert.AreEqual(1, lType.Fields.Count);
            Assert.AreEqual(0, lType.Functions.Count);
        }

        [TestMethod]
        public void TestParseFunction()
        {
            var parser = new Parser();
            Assert.IsTrue(parser.CurrentLuaFunction == null);
            parser.ParseLine("-- Some function");
            parser.ParseLine("-- @param #number x The input number");
            parser.ParseLine("-- @return #MyType Some data");
            parser.ParseLine("function foobar(x)");
            Assert.IsTrue(parser.CurrentLuaFunction != null);
            LuaFunction lFunction = parser.CurrentLuaFunction;
            Assert.AreEqual("foobar", lFunction.Name);
            Assert.AreEqual("Some function", lFunction.Description);
            LuaFunctionReturn lFunctionReturn = lFunction.Return;
            Assert.AreEqual("MyType", lFunctionReturn.Type.Name);
        }
    }
}
