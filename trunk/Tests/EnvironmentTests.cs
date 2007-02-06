using System;
using System.Collections.Generic;
using System.Text;
using PVSLisp.Common;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class EnvironmentTests
    {
        Runtime runtime = new Runtime();

        [Test]
        public void TestCreateHierarchy()
        {
            GlobalEnvironment global = new GlobalEnvironment(runtime);
            LispEnvironment level1 = new LispEnvironment(global);

            Assert.AreEqual(runtime, level1.Runtime);
            Assert.AreEqual(global, level1.Parent);

            LispEnvironment level2 = new LispEnvironment(level1);
            Assert.AreEqual(level1, level2.Parent);
        }

        [Test]
        public void TestIsItConst()
        {
            GlobalEnvironment consts = new GlobalEnvironment(runtime);
            Assert.AreEqual(true, consts.IsItConst(new Symbol("t")));
            Assert.AreEqual(false, consts.IsItConst(new Symbol("long-system-id")));
        }

        [Test]
        public void TestConsts()
        {
            GlobalEnvironment global = new GlobalEnvironment(runtime);
            LispEnvironment env = new LispEnvironment(global);

            LObject val = env.GetSymbolValue(new Symbol("T"));
            Assert.IsInstanceOfType(typeof(Scalar), val);
            Assert.AreEqual(true, (val as Scalar).Value);

            val = env.GetSymbolValue(new Symbol("NIL"));
            Assert.IsInstanceOfType(typeof(Scalar), val);
            Assert.IsNull((val as Scalar).Value);

            LispAssert.AssertLispException("We can assign to a const!", 
            delegate()
            {
                env.Assign(new Symbol("t"), ScalarFactory.Make(false));
            });

        }

        [Test]
        public void TestGlobalVariables()
        {
            LispEnvironment global = new GlobalEnvironment(runtime);
            LispEnvironment level1 = new LispEnvironment(global);
            LispEnvironment level2 = new LispEnvironment(level1);

            Symbol s = new Symbol("global-var");
            LObject value = ScalarFactory.Make("value");
            LObject value2 = ScalarFactory.Make("value2");

            level1.Assign(s, value); 
            Assert.AreEqual(value, global.GetSymbolValue(s));
            Assert.AreEqual(value, level1.GetSymbolValue(s));
            Assert.AreEqual(value, level2.GetSymbolValue(s));

            level2.Assign(s, value2); 
            Assert.AreEqual(value2, global.GetSymbolValue(s));
            Assert.AreEqual(value2, level1.GetSymbolValue(s));
            Assert.AreEqual(value2, level2.GetSymbolValue(s));
        }

        [Test]
        public void AssignGlobalSymbol()
        {
            LispEnvironment global = new GlobalEnvironment(runtime);
            LispEnvironment level1 = new LispEnvironment(global);
            LispEnvironment level2 = new LispEnvironment(level1);
            LispEnvironment level3 = new LispEnvironment(level2);

            Symbol s = new Symbol("global-var");
            LObject value = ScalarFactory.Make("value");
            LObject value2 = ScalarFactory.Make("value2");

            level1.AssignGlobalSymbol(s, value);
            Assert.AreEqual(value, level1.GetSymbolValue(s));
            Assert.AreEqual(value, level2.GetSymbolValue(s));
            Assert.AreEqual(value, level3.GetSymbolValue(s));

            level2.AssignGlobalSymbol(s, value2);
            Assert.AreEqual(value, level1.GetSymbolValue(s));
            Assert.AreEqual(value2, level2.GetSymbolValue(s));
            Assert.AreEqual(value2, level3.GetSymbolValue(s));
        }

        [Test]
        public void TestLocalVariables()
        {
            LispEnvironment global = new GlobalEnvironment(runtime);
            LispEnvironment level1 = new LispEnvironment(global);
            LispEnvironment level2 = new LispEnvironment(level1);

            Symbol s = new Symbol("local-var");
            LObject value = ScalarFactory.Make("value");
            LObject value2 = ScalarFactory.Make("value2");
            LObject value3 = ScalarFactory.Make("value3");

            level1.AssignLocalSymbol(s, value);
            Assert.AreEqual(value, level1.GetSymbolValue(s));
            Assert.AreEqual(null, level2.GetSymbolValue(s));

            level2.Assign(s, value2);
            Assert.AreEqual(value2, global.GetSymbolValue(s));
            Assert.AreEqual(value, level1.GetSymbolValue(s));
            Assert.AreEqual(value2, level2.GetSymbolValue(s));

            level2.AssignLocalSymbol(s, value3);
            Assert.AreEqual(value2, global.GetSymbolValue(s));
            Assert.AreEqual(value, level1.GetSymbolValue(s));
            Assert.AreEqual(value3, level2.GetSymbolValue(s));
        }

        [Test]
        public void TestTransparent()
        {
            LispEnvironment global = new GlobalEnvironment(runtime);
            LispEnvironment level1 = new LispEnvironment(global);
            LispEnvironment level2 = new LispEnvironment(level1);

            Symbol s = new Symbol("local-var");
            LObject value = ScalarFactory.Make("value");
            LObject value2 = ScalarFactory.Make("value2");
            LObject value3 = ScalarFactory.Make("value3");

            level1.AssignLocalSymbol(s, value);
            Assert.AreEqual(value, level1.GetSymbolValue(s));
            Assert.AreEqual(null, level2.GetSymbolValue(s));

            level2.Transparent = true;
            Assert.AreEqual(value, level2.GetSymbolValue(s));

            level2.Assign(s, value2);
            Assert.AreEqual(value2, level1.GetSymbolValue(s));
            Assert.AreEqual(value2, level2.GetSymbolValue(s));

            level2.AssignLocalSymbol(s, value3);
            Assert.AreEqual(value2, level1.GetSymbolValue(s));
            Assert.AreEqual(value3, level2.GetSymbolValue(s));
        }

        [Test]
        public void Complex1()
        {
            LispEnvironment global = new GlobalEnvironment(runtime);
            LispEnvironment func = new LispEnvironment(global);

            Symbol varL1 = new Symbol("value1");
            Symbol varL2 = new Symbol("value2");
            LObject varL1value = ScalarFactory.Make(123);
            LObject varL2value = ScalarFactory.Make("text");
            global.Assign(varL1, varL1value);
            func.Assign(varL2, varL2value);

            Assert.AreEqual(varL1value, global.GetSymbolValue(varL1));
            Assert.AreEqual(varL2value, global.GetSymbolValue(varL2));
            Assert.AreEqual(varL1value, func.GetSymbolValue(varL1));
            Assert.AreEqual(varL2value, func.GetSymbolValue(varL2));

            LObject newValue1 = ScalarFactory.Make(false);
            LObject newValue2 = new Symbol("value");
            func.Assign(varL1, newValue1);
            func.AssignLocalSymbol(varL2, newValue2);

            Assert.AreEqual(newValue1, global.GetSymbolValue(varL1));
            Assert.AreEqual(varL2value, global.GetSymbolValue(varL2));
            Assert.AreEqual(newValue1, func.GetSymbolValue(varL1));
            Assert.AreEqual(newValue2, func.GetSymbolValue(varL2));

            Symbol s = new Symbol("s");
            func.AssignLocalSymbol(s, ScalarFactory.Make(0));
            Assert.IsNull(global.GetSymbolValue(s));
        }

    }
}
