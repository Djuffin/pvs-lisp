using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using PVSLisp.Parser;
using PVSLisp.Common;
using PVSLisp;
using System.IO;
using PVSLisp.Lexer;

namespace Tests
{
    [TestFixture]
    public class InitScriptTest
    {
        Program consoleInterpreter = new Program();

        [TestFixtureSetUp]
        public void Setup()
        {
            consoleInterpreter.RunInitScript();
            string defAssert = "( defun assert (condition msg) (cond (condition t) (t (throw msg)) ) )";
            consoleInterpreter.Execute(defAssert);
        }

        [Test]
        public void AssertTest()
        {
            consoleInterpreter.Execute("(eq (assert t \"Impossible!\") t)");
            LispAssert.AssertLispException("Should be LispExcetion. ASSERT(FALSE)",
                delegate
                {
                    consoleInterpreter.Execute("(assert nil \"Always!\")");
                });
        }

        [Test]
        public void TestFromFile()
        {
            using (TextReader reader = new StreamReader("tests.ls", Encoding.Default))
            {
                consoleInterpreter.Execute(reader.ReadToEnd());
            }
        }
    }
}
