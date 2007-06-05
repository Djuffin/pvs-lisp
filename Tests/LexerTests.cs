using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using PVSLisp.Lexer;

namespace Tests
{
    [TestFixture]
    public class LexerTests
    {

        Type LB = typeof(ListBeginingToken);
        Type LE = typeof(ListEndToken);
        Type QUOTE = typeof(QuoteToken);
        Type INT = typeof(IntConstToken);
        Type DBL = typeof(DoubleConstToken);
        Type INVALID = typeof(InvalidToken);
        Type SYMB = typeof(SymbolToken);
        Type STR = typeof(StringConstToken);
        Lexer lexer = new Lexer();


        public Token[] TestTokenTypes(string input, Type[] types)
        {
            Token[] tokens = lexer.GetTokens(input, false);
            Assert.AreEqual(types.Length, tokens.Length);

            for (int i = 0; i < types.Length; i++)
                Assert.AreEqual(types[i], tokens[i].GetType());
            return tokens;
        }

        [Test]
        public void TestOfTests()
        {
            Assert.AreEqual(1, 1);
        }

        [Test]
        public void ListTest()
        {
            TestTokenTypes("()", new Type[] { LB, LE });
        }

        [Test]
        public void SymbolTest()
        {
            Token[] toks = TestTokenTypes("a-zA-Z_+*/@$%&,.=?[] - 0a-zA-Z_+*/@$%&,.=?", new Type[] { SYMB, SYMB, INVALID });
            Assert.AreEqual("a-zA-Z_+*/@$%&,.=?[]", toks[0].Text);
        }

        [Test]
        public void StringText()
        {
            Type[] types = new Type[] { STR, STR, INVALID, INVALID, SYMB, SYMB };
            Token[] toks = TestTokenTypes(@"""abba abba baobab"" "" \tasd \"" bac\n as\r \\ "" "" \k "" "" no end", types);
            Assert.AreEqual(@"""abba abba baobab""", toks[0].Text);
            Assert.AreEqual(@""" \tasd \"" bac\n as\r \\ """, toks[1].Text);
            Assert.AreEqual(" \tasd \" bac\n as\r \\ ", (toks[1] as StringConstToken).Value);
        }

        [Test]
        public void CommentTest()
        {
            TestTokenTypes(" \"string;comment\" ; real comment \n a 123 ; onother comment ", new Type[] { STR, SYMB, INT });
        }

        [Test]
        public void IntegerTest()
        {
            Token[] toks = TestTokenTypes("123 123_ 999999999999999999999999999999 -2", new Type[] { INT, INVALID, INVALID, INT });
            Assert.AreEqual("123", toks[0].Text);
            Assert.AreEqual("-2", toks[3].Text);
        }

        [Test]
        public void DoubleTest()
        {
            Token[] toks = TestTokenTypes("123.112 123. -0.321 .0 999999999999999999999999999999.123", new Type[] { DBL, DBL, DBL, SYMB, DBL });
        }

        [Test]
        public void QuoteTest()
        {
            Token[] toks = TestTokenTypes("`' `'", new Type[] { QUOTE, QUOTE, QUOTE, QUOTE });
            Assert.AreEqual(false, (toks[0] as QuoteToken).FunctionQuote);
            toks = TestTokenTypes("#'a", new Type[] { QUOTE, SYMB });
            Assert.AreEqual(true, (toks[0] as QuoteToken).FunctionQuote);
        }


        [Test]
        public void Complex1()
        {
            string text = "((asd 1 2.123k0.23 21 \"asdasd\")`(a)(4))";
            Type[] types = new Type[] { LB, LB, SYMB, INT, INVALID, INT, STR, LE, QUOTE, LB, SYMB, LE, LB, INT, LE, LE };
            TestTokenTypes(text, types);
        }

        [Test]
        public void Complex2()
        {
            string text = "(eql (nth 7 (append '(1 2 3) '(4 5 6) '(7 8 9))) 8)";
            Type[] types = new Type[] { LB, SYMB, LB, SYMB, INT, LB, SYMB, QUOTE, LB, INT, INT, INT, LE, QUOTE, LB, INT, INT, INT, LE, QUOTE, LB, INT, INT, INT, LE, LE, LE, INT, LE };
            TestTokenTypes(text, types);
        }

        [Test]
        public void Complex3()
        {
            string text = "`(= ,lst (cons ;comment\n ,item ,lst)))";
            Type[] types = new Type[] { QUOTE, LB, SYMB, SYMB, LB, SYMB, SYMB, SYMB, LE, LE, LE };
            TestTokenTypes(text, types);
        }

        [Test]
        public void Complex4()
        {
            string text = "( symbol 123 123.231 123k 'qaz \"text\")";
            Type[] types = new Type[] { LB, SYMB, INT, DBL, INVALID, QUOTE, SYMB, STR, LE };

            Token[] tokens = TestTokenTypes(text, types);
            Assert.AreEqual("123", tokens[2].Text);
            Assert.AreEqual("123.231", tokens[3].Text);
            Assert.AreEqual("123k", tokens[4].Text);
            Assert.AreEqual("qaz", tokens[6].Text);
            Assert.AreEqual("\"text\"", tokens[7].Text);
        }

    }
}
