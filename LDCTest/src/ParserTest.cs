﻿using LDC;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace LDC
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
            Assert.IsTrue(type.SuperTypeName == null);
            parser.ParseLine("-- @extends #SuperType");
            Assert.IsTrue(type.SuperTypeName != null);
            Assert.AreEqual("#SuperType", type.SuperTypeName);
        }

        [TestMethod]
        public void ParseTypeField()
        {
            var parser = new Parser();
            parser.ParseLine("-- @type MyType");
            LuaType type = parser.CurrentLuaType;
            Assert.AreEqual(0, type.Fields.Count);
            parser.ParseLine("-- @field #number x The X coordinate");
            parser.ParseLine("-- @field #list<#number> list");
            Assert.AreEqual(2, type.Fields.Count);
            LuaVariable field1 = type.Fields[0];
            Assert.AreEqual("#number", field1.TypeName);
            Assert.AreEqual("x", field1.Name);
            Assert.AreEqual("The X coordinate", field1.Description);
            Assert.AreEqual(0, type.Functions.Count);
            LuaVariable field2 = type.Fields[1];
            Assert.AreEqual("#list<#number>", field2.TypeName);
            Assert.AreEqual("list", field2.Name);
        }

        [TestMethod]
        public void ParseFunction()
        {
            var parser = new Parser();
            parser.ParseLine("-- Some function");
            parser.ParseLine("-- @param #number x The input number");
            parser.ParseLine("-- @param #list<#number> list");
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
            Assert.AreEqual(2, function.Parameters.Count);
            var param1 = function.Parameters[0];
            Assert.AreEqual("#number", param1.TypeName);
            Assert.AreEqual("x", param1.Name);
            Assert.AreEqual("The input number", param1.Description);
            var param2 = function.Parameters[1];
            Assert.AreEqual("#list<#number>", param2.TypeName);
            Assert.AreEqual("list", param2.Name);

            // Function return type
            Assert.AreEqual("#MyType", function.ReturnType);
            Assert.AreEqual("Some data", function.ReturnDescription);
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
        }

        [TestMethod]
        public void ParseModule()
        {
            var parser = new Parser();
            parser.ParseLine("-- @module MyModule");
            Assert.IsTrue(parser.LuaModules.ContainsKey("MyModule"));
            var module = parser.LuaModules["MyModule"];
            Assert.AreEqual(0, module.LuaTypes.Count);
            parser.ParseLine("");
            parser.ParseLine("-- @type MyType");
            Assert.IsTrue(module.LuaTypes.ContainsKey("#MyType"));
            Assert.AreEqual(0, module.LuaFunctions.Count);
            parser.ParseLine("function foobar()");
            Assert.AreEqual(1, module.LuaFunctions.Count);
        }

        [TestMethod]
        public void ParseModuleAndTypeAndFunction()
        {
            var parser = new Parser();
            parser.ParseLine("-- @module MyModule");
            parser.ParseLine("-- @type MyType");
            parser.ParseLine("");
            parser.ParseLine("-- Some function");
            parser.ParseLine("-- @param #MyType self");
            parser.ParseLine("function MyType:foobar()");
            var type = parser.LuaModules["MyModule"].LuaTypes["#MyType"];
            Assert.AreEqual(1, type.Functions.Count);
            var function = type.Functions[0];
            Assert.AreEqual("foobar", function.Name);
            Assert.AreEqual(1, function.Parameters.Count);
            var parameter = function.Parameters[0];
            Assert.AreEqual("#MyType", parameter.TypeName);
        }

        [TestMethod]
        public void ParseTypeAlias()
        {
            var parser = new Parser();
            parser.ParseLine("-- @module MyModule");
            parser.ParseLine("-- @type MyType");
            parser.ParseLine("mm_MyType = someMethod()");
            parser.ParseLine("");
            parser.ParseLine("-- Some function");
            parser.ParseLine("-- @param #MyType self");
            parser.ParseLine("function mm_MyType:foobar()");
            Assert.AreEqual(2, parser.LuaModules.Count);
            var module = parser.LuaModules["MyModule"];
            Assert.AreEqual(1, module.LuaTypes.Count);
            Assert.IsTrue(module.LuaTypes.ContainsKey("#MyType"));
        }

        [TestMethod]
        public void ParseModuleDescription()
        {
            var parser = new Parser();
            parser.ParseLine("-- line 1");
            parser.ParseLine("-- line 2");
            parser.ParseLine("-- @module MyModule");
            var module = parser.LuaModules["MyModule"];
            Assert.AreEqual("line 1\nline 2", module.Description);
        }
    }
}
