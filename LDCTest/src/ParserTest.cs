using LDCLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace LDCTest
{
    [TestClass]
    public class ParserTest
    {
        [TestMethod]
        public void ParseType()
        {
            var parser = new Parser();
            Assert.IsTrue(parser.CurrentLuaType == null);
            parser.ParseLine("-- @type MyType");
            Assert.IsTrue(parser.CurrentLuaType != null);
            LuaType type = parser.CurrentLuaType;
            Assert.AreEqual("#MyType", type.Name);
        }

        [TestMethod]
        public void ParseSuperType()
        {
            var parser = new Parser();
            parser.ParseLine("-- @type MyType");
            LuaType type = parser.CurrentLuaType;
            Assert.IsTrue(type.SuperLuaType == null);
            parser.ParseLine("-- @extends #SuperType");
            Assert.IsTrue(type.SuperLuaType != null);
            Assert.AreEqual("#SuperType", type.SuperLuaType.Name);
        }

        [TestMethod]
        public void ParseTypeField()
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
            Assert.AreEqual("#number", numberType.Name);
            Assert.AreEqual(1, type.Fields.Count);
            Assert.AreEqual(0, type.Functions.Count);
        }

        [TestMethod]
        public void ParseFunction()
        {
            var parser = new Parser();
            parser.ParseLine("-- Some function");
            parser.ParseLine("-- @param #number x The input number");
            parser.ParseLine("-- @return #MyType Some data");
            parser.ParseLine("function foobar(x)");

            Assert.IsTrue(parser.LuaModules.ContainsKey(""));
            Assert.AreEqual(1, parser.LuaModules[""].LuaFunctions.Count);

            // Function header
            LuaFunction function = parser.LuaModules[""].LuaFunctions[0];
            Assert.AreEqual("foobar", function.Name);

            // Function description
            Assert.AreEqual("Some function", function.Description);

            // Function parameters
            Assert.AreEqual(1, function.Parameters.Count);
            var parameter = function.Parameters[0];
            Assert.AreEqual("#number", parameter.Type.Name);
            Assert.AreEqual("x", parameter.Name);
            Assert.AreEqual("The input number", parameter.Description);

            // Function return type
            LuaFunctionReturn functionReturn = function.Return;
            Assert.AreEqual("#MyType", functionReturn.Type.Name);
        }

        [TestMethod]
        public void ParseTypeFunction()
        {
            var parser = new Parser();
            parser.ParseLine("-- @type MyType");
            parser.ParseLine("");
            parser.ParseLine("-- Some function");
            parser.ParseLine("-- @param #MyType self");
            parser.ParseLine("function MyType:foobar()");
            var type = parser.LuaModules[""].LuaTypes["#MyType"];
            Assert.AreEqual(1, type.Functions.Count);
            var function = type.Functions[0];
            Assert.AreEqual("foobar", function.Name);
            Assert.AreEqual(1, function.Parameters.Count);

            Assert.AreEqual(1, function.Parameters.Count);
        }
    }
}
