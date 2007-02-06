using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using PVSLisp.Lexer;
using PVSLisp.Parser;
using PVSLisp.Common;

namespace Tests
{
    [TestFixture]
    public class ParserTests
    {
        Lexer lexer = new Lexer();



        private LObject[] Parse(string text)
        {
            Token[] tokens = lexer.GetTokens(text, false);
            LObject[] result = new Parser(tokens).Parse();
            return result;
        }

        [Test]
        public void ConstsTest()
        {
            int v0 = 12;
            double v1 = 12123.123123;
            string v2 = "Hello World!";

            LObject[] result = Parse(string.Format("{0} {1} \"{2}\"", v0, v1, v2));
            Assert.AreEqual(ScalarFactory.Make(v0), result[0]);
            Assert.AreEqual(ScalarFactory.Make(v1), result[1]);
            Assert.AreEqual(ScalarFactory.Make(v2), result[2]);
        }

        [Test]
        public void SymbolsTest()
        {
            LObject[] result = Parse("a b c");
            Assert.AreEqual(new Symbol("a"), result[0]);
            Assert.AreEqual(new Symbol("b"), result[1]);
            Assert.AreEqual(new Symbol("c"), result[2]);
        }

        [Test]
        public void QuoteTest()
        {
            LObject[] result = Parse("'a '()");
            LCell expect0 = LCell.Make(new LObject[] {new Symbol("quote"), new Symbol("a") });
            LCell expect1 = LCell.Make(new LObject[] { new Symbol("quote"), SpecialValues.NIL });
            Assert.IsTrue(LCell.EqualLists(expect0, result[0]));
            Assert.IsTrue(LCell.EqualLists(expect1, result[1]));
        }

        [Test]
        public void ErrorsTest()
        {
            try
            {
                Parse(" ( a ( ( a b 1 ) 'a )");
                Assert.Fail("No exception about unclosed bracket");
            }
            catch (ParserException)
            {
            }

            try
            {
                Parse(" ( a (a 7a) '(a) )");
                Assert.Fail("No exception about invalid token");
            }
            catch (ParserException)
            {
                
            }

        }

        [Test]
        public void ListTest()
        {
            LCell expect = LCell.Make(new LObject[] 
            {
                new Symbol("where?"), 
                new Symbol("when?"),
                LCell.Make( new LObject[]
                {
                    new Symbol("calc"),
                    LCell.Make( new LObject[]
                    {
                        ScalarFactory.Make(-1.2),
                        ScalarFactory.Make("Yoo!"),
                    })
                }),
                LCell.Make( new LObject[]
                {
                    ScalarFactory.Make(-321),
                    ScalarFactory.Make("Bilbo"),
                    SpecialValues.NIL
                })

            });

            string text = expect.ToLispString();
            LCell result = Parse(text)[0] as LCell;
            Assert.IsTrue(LCell.EqualLists(expect, result));
        }

        [Test]
        public void Complex1()
        {
            LObject[] result = Parse("( a b '( 12 \"Yoo!\" ()) 0.12) ");

            LCell expect = LCell.Make(new LObject[] 
            {
                new Symbol("a"), 
                new Symbol("b"),
                LCell.Make( new LObject[]
                {
                    new Symbol("quote"),
                    LCell.Make( new LObject[]
                    {
                        ScalarFactory.Make(12),
                        ScalarFactory.Make("Yoo!"),
                        SpecialValues.NIL
                    })
                }),
                ScalarFactory.Make(0.12)

            });
            Assert.IsTrue(LCell.EqualLists(expect, result[0]));

        }



    }
}
