using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using PVSLisp.Parser;
using PVSLisp.Common;
using PVSLisp;

namespace Tests
{
    public class LispAssert
    {

        public static void IsT(string lispCode)
        {
            Assert.AreEqual(SpecialValues.T, Interpreter.ExecuteOne(lispCode));
        }

        public static void IsNIL(string lispCode)
        {
            Assert.AreEqual(SpecialValues.NIL, Interpreter.ExecuteOne(lispCode));
        }

        public delegate void TestRoutine();
        public static void AssertLispException(string message, TestRoutine test)
        {
            try
            {
                test();
            }
            catch (LispException e)
            {
                return;
            }
            Assert.Fail(message);
        }


    }
}
