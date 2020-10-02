using LDCLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
            LuaType type = parser.CurrentLuaType;
            Assert.AreEqual("MyType", type.Name);
        }

        [TestMethod]
        public void TestParseSuperType()
        {
            var parser = new Parser();
            parser.ParseLine("-- @type MyType");
            LuaType type = parser.CurrentLuaType;
            Assert.IsTrue(type.SuperLuaType == null);
            parser.ParseLine("-- @extends #SuperType");
            Assert.IsTrue(type.SuperLuaType != null);
            Assert.AreEqual("SuperType", type.SuperLuaType.Name);
        }

        [TestMethod]
        public void TestTypeField()
        {
            var parser = new Parser();
            parser.ParseLine("-- @type MyType");
            LuaType type = parser.CurrentLuaType;
            Assert.AreEqual(0, type.Fields.Count);
            parser.ParseLine("-- @field #number x The X coordinate");
            Assert.AreEqual(1, type.Fields.Count);
            LuaVariable variable = type.Fields[0];
            Assert.AreEqual("x", variable.Name);
            Assert.AreEqual("The X coordinate", variable.Description);
            LuaType numberType = variable.Type;
            Assert.AreEqual("number", numberType.Name);
            Assert.AreEqual(1, type.Fields.Count);
            Assert.AreEqual(0, type.Functions.Count);
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
            LuaFunction function = parser.CurrentLuaFunction;
            Assert.AreEqual("foobar", function.Name);
            Assert.AreEqual("Some function", function.Description);
            LuaFunctionReturn functionReturn = function.Return;
            Assert.AreEqual("MyType", functionReturn.Type.Name);
        }
    }
}
