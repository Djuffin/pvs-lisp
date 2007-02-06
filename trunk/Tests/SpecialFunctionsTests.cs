using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using PVSLisp.Parser;
using PVSLisp.Common;
using PVSLisp;


namespace Tests
{
    [TestFixture]
    public class SpecialFunctionsTests
    {


        #region Arithmetic Functions
        [Test]
        public void AddTest()
        {
            LispAssert.IsT("(eq 5.5 (+ 1 2 2.5))");
        }

        [Test]
        public void SubtractTest()
        {
            LispAssert.IsT("(eq -2 (- 1 2 1))");
        }

        [Test]
        public void MultiplyTest()
        {
            LispAssert.IsT("(eq 3.0 (* 0.5 2 3))");
        }

        [Test]
        public void DivisionTest()
        {
            LispAssert.IsT("(eq 0.5 (/ 1 2))");
        }
        #endregion

        #region Lisp Native Functions

        [Test]
        public void AtomTest()
        {
            LispAssert.IsT("(atom 1)");
            LispAssert.IsT("(atom 'x)");
            LispAssert.IsT("(atom t)");
            LispAssert.IsT("(atom '())");
            LispAssert.IsNIL("(atom '(1 2 3))");
            LispAssert.IsNIL("(atom (lambda (x, y) (+ x y)))");
        }

        [Test]
        public void NullTest()
        {
            LispAssert.IsT("(null '())");
            LispAssert.IsT("(null ())");
            LispAssert.IsNIL("(null '(1 2 3))");
            LispAssert.IsNIL("(null 'x)");
        }

        [Test]
        public void CondTest()
        {
            string code = "(eq 3 (cond ((eq 1 2) 2) ((eq t t) 3) ))";
            LispAssert.IsT(code);

            code = "(eq 0 (cond ((eq 1 2) 2) ((eq t nil) 3) (t 0) ))";
            LispAssert.IsT(code);
        }

        [Test]
        public void ClosureTest()
        {
            string define = "( (lambda (x) (set 'addX (closure (y) (+ x y)) ) )  100)";
            string exec = "( eq 102 (addX 2) )";

            LispAssert.IsT(define + exec);
        }

        [Test]
        public void LambdaTest()
        {
            Assert.AreEqual(ScalarFactory.Make(3), Interpreter.ExecuteOne("( (lambda (x y) (+ x y)) 1 2 )"));
        }

        [Test]
        public void MacroTest()
        {
            string defSetq = "(set 'setq (macro (x y) (list 'set (list 'quote x) y)) )";
            string execSetq = "( setq x 100 ) ( eq x 100 )";
            LispAssert.IsT(defSetq + execSetq);

            string defDefun = "(set 'defun (macro (name params body) (list 'setq name (list 'lambda params body)) ) )";
            string execDefun = "( defun add (x y) (+ x y) ) ( eq 100 (add 25 75) )";
            LispAssert.IsT(defSetq + defDefun + execDefun);
        }

        [Test]
        public void QuoteTest()
        {
            LCell list = LCell.Make(new LObject[] { ScalarFactory.Make(1), ScalarFactory.Make(2) });
            Assert.IsTrue(LCell.EqualLists(list, Interpreter.ExecuteOne("'(1 2)")));
        }

        [Test]
        public void EvalTest()
        {
            LispAssert.IsT("(set 'x 1) (eq 1 (eval 'x))");
        }

        [Test]
        public void SetAndEqTest()
        {
            LispAssert.IsT("(set 'x 123) (eq x 123)");
        }

        [Test]
        public void EqTest()
        {
            LispAssert.IsT("(eq 'x 'x)");
            LispAssert.IsNIL("(eq 'x 'y)");
        }
        #endregion

        #region Logical Functions
        [Test]
        public void AndTest()
        {
            LispAssert.IsT("(and 1 2 3)");
            LispAssert.IsNIL("(and 1 () 3)");
            LispAssert.IsNIL("(and nil)");
            LispAssert.IsT("(and t)");
        }

        [Test]
        public void OrTest()
        {
            LispAssert.IsT("(or 1 2 3)");
            LispAssert.IsT("(or 1 () 3)");
            LispAssert.IsNIL("(or nil)");
            LispAssert.IsNIL("(or nil (eq 1 0))");
        }

        [Test]
        public void NotTest()
        {
            LispAssert.IsT("(not nil)");
            LispAssert.IsNIL("(not t)");
        }
        #endregion

        #region List processing

        [Test]
        public void ConsTest()
        {
            LCell.EqualLists(Interpreter.ExecuteOne("(cons 1 '(2 3))"), Interpreter.ExecuteOne("(list 1 2 3)"));
            LispAssert.AssertLispException("Second argument of cons is atom, but no exception",
                delegate
                {
                    Interpreter.ExecuteOne("(cons 1 1)");
                });
        }


        [Test]
        public void ListTest()
        {
            LCell list = LCell.Make(new LObject[] { ScalarFactory.Make(1), new Symbol("x"), ScalarFactory.Make(3) });
            LCell.EqualLists(list, Interpreter.ExecuteOne("(list 1 'x 3)"));

            LispAssert.IsT("(eq nil (list))");
        }

        [Test]
        public void CarTest()
        {
            LispAssert.IsT("(eq 1 (car '(1 2 3)))");
        }

        [Test]
        public void CdrTest()
        {
            LCell list = LCell.Make(new LObject[] { ScalarFactory.Make(1), new Symbol("x"), ScalarFactory.Make(3) });
            LCell.EqualLists(list, Interpreter.ExecuteOne("(cdr '(a 1 x 3))"));
            LispAssert.IsT("(eq nil (cdr '(1)))");
        }

        #endregion

        #region .NET support

        [Test]
        public void NewTest()
        {
            Scalar s = (Scalar)Interpreter.ExecuteOne("(.new System.Text.StringBuilder )");
            Assert.AreEqual(typeof(System.Text.StringBuilder), s.Value.GetType());

            s = (Scalar)Interpreter.ExecuteOne("(.new System.Text.StringBuilder \"Hello\" )");
            Assert.AreEqual(typeof(System.Text.StringBuilder), s.Value.GetType());

            Assert.AreEqual("Hello", s.Value.ToString());
        }

        [Test]
        public void CallTest()
        {
            Scalar s = (Scalar)Interpreter.ExecuteOne("(. 'tostring ( dayOfWeek ( now DateTime) ) )");
            Assert.AreEqual(DateTime.Now.DayOfWeek.ToString(), s.Value);
        }

        [Test]
        public void StaticCallTest()
        {
            Scalar s = (Scalar)Interpreter.ExecuteOne("(. format string \"number {0}\" 1 )");
            Assert.AreEqual(string.Format("number {0}", 1), s.Value);

        }

        [Test]
        public void NewExceptionsTest()
        {
            LispAssert.AssertLispException("Call new without typename, but no exception",
                delegate
                {
                    Interpreter.ExecuteOne("(.new )");
                });

            LispAssert.AssertLispException("First agrument of new cannot be evaluated symbol, but no exception",
                delegate
                {
                    Interpreter.ExecuteOne("(.new 123)");
                });

            LispAssert.AssertLispException("Wrong argument for new, but no exception",
                delegate
                {
                    Interpreter.ExecuteOne("(.new system.text.stringbuilder '(1) )");
                });

            LispAssert.AssertLispException("Wrong constructor was called, but no exception",
                delegate
                {
                    Interpreter.ExecuteOne("(.new string)");
                });
        }

        [Test]
        public void CallExceptionsTest()
        {
            LispAssert.AssertLispException("Wrong arguments, but no exception",
                delegate
                {
                    Interpreter.ExecuteOne("(. tostring (.new system.text.stringBuilder) 1 b)");
                });

            LispAssert.AssertLispException("this == null, but no exception",
                delegate
                {
                    Interpreter.ExecuteOne("(set 'x nil)(. tostring x )");
                });

            LispAssert.AssertLispException("write to readonly property, but no exception",
                delegate
                {
                    Interpreter.ExecuteOne("(. second (. now datetime) 123 )");
                });
            LispAssert.AssertLispException("Unresolved method/property/field, but no exception",
                delegate
                {
                    Interpreter.ExecuteOne("(. second1 (. now datetime) 123 )");
                });
        }

        [Test]
        public void ThrowTest()
        {
            try
            {
                Interpreter.ExecuteOne("(throw 123)");
            }
            catch (LispException e)
            {
                Assert.AreEqual("123", e.Message);
                return;
            }
            Assert.Fail("Throw! but no exception");
        }
        #endregion
    }
}
