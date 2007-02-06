using System;
using System.Collections.Generic;
using System.Text;
using PVSLisp.Common;
using PVSLisp.DotNet;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class DotNetTests
    {
        [Test]
        public void TypeResolverTest()
        {
            TypeResolver.Instance.ClearUsings();
            Assert.AreEqual(typeof(Int32), TypeResolver.Instance.GetTypeByName("Int32"));

            Assert.AreEqual(null, TypeResolver.Instance.GetTypeByName("Symbol"));

            TypeResolver.Instance.AddUsing("PVSLisp.Common");
            Type result = TypeResolver.Instance.GetTypeByName("SymBoL");
            Assert.AreEqual(typeof(Symbol), result);
        }

        [Test]
        public void LoadAssemblyTest()
        {
            TypeResolver.Instance.ClearUsings();
            Assert.IsNotNull(TypeResolver.Instance.LoadAssembly("System.Xml"));
            Assert.IsNotNull(TypeResolver.Instance.GetTypeByName("System.Xml.XmlNode"));
        }

        [Test]
        public void StaticCallTest()
        {
            string result = (string)DotNetInterop.CallStatic("Environment", "MachineName", null);
            Assert.AreEqual(System.Environment.MachineName, result);
        }

        [Test]
        public void NewTest()
        {
            string value = "text-value";
            object o = DotNetInterop.New("system.text.StringBuilder",  value );
            Assert.AreEqual(typeof(System.Text.StringBuilder), o.GetType());
            Assert.AreEqual(value, DotNetInterop.Call(o, "tostring", null) );
        }

        [Test]
        public void CallTest()
        {
            string value = "0123456789";
            StringBuilder sb = (StringBuilder)DotNetInterop.New("System.Text.StringBuilder", value );
            string substring = (string)DotNetInterop.Call(sb, "toString", 3, 3 );
            Assert.AreEqual(value.Substring(3, 3), substring);
            
        }

    }
}
