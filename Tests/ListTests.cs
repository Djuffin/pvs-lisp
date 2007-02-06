using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using PVSLisp.Common;

namespace Tests
{
    [TestFixture]
    public class ListTests
    {
        [Test]
        public void CreationTest()
        {
            Assert.IsNull(LCell.Make(new LObject[0]));

            LObject[] prototype = new LObject[]
            {
                new Symbol("var"),
                ScalarFactory.Make(1),
                new LCell( new Symbol("R"))
            };

            LCell list = LCell.Make(prototype);

            Assert.AreEqual(prototype.Length, list.Count);
            int index = 0;
            foreach (LObject o in list)
                Assert.AreEqual(prototype[index++], o);

        }

        [Test]
        public void TestToString()
        {
            LCell innerList = new LCell(ScalarFactory.Make(2), new LCell(ScalarFactory.Make(3)));
            LCell outerList = new LCell(ScalarFactory.Make(1), new LCell(innerList));

            Assert.AreEqual("( 1 ( 2 3 ) )", outerList.ToString());
        }

        [Test]
        public void SimpleEvaluateTest()
        {
            LCell command = LCell.Make(
                new LObject[]
                {
                    new Symbol("print"),
                    ScalarFactory.Make("Hello "),
                    ScalarFactory.Make("World!")
                }
            );

            GlobalEnvironment consts = new GlobalEnvironment(new Runtime());
            LispEnvironment env = new LispEnvironment(consts);
            LObject result = command.Evaluate(env);
            Assert.AreEqual(result, SpecialValues.NIL);
        }


    }
}
