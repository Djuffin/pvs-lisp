using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using PVSLisp.Parser;
using PVSLisp.Common;
using PVSLisp;
using PVSLisp.Lexer;

namespace Tests
{
    [TestFixture]
    public class SpecialTests
    {

        [Test]
        public void BindUserFunctionParamsTest()
        {
            string f1 = "(set 'f1 (lambda (x) (+ 1 x)))";
            string f2 = "(set 'f2 (lambda (x) (f1 x)))";
            string call = "(eq 1 (f2 0))";
            LispAssert.IsT(f1 + f2 + call);
        }

        [Test]
        public void FibonacciTest()
        {
            string defFib = "( set 'fib (lambda (x) ("
                + "cond ((eq x 0) 1) ((eq x 1) 1) (t (+ (fib (- x 1)) (fib (- x 2)) )) "
                + ") ) )";

            string call = "(eq 34 (fib 8))";

            LispAssert.IsT(defFib + call);
        }

        [Test]
        public void FactorialTest()
        {
            string defFib = "( set 'factorial (lambda (x) ("
                + "cond ((eq x 0) 1) (t (* x (factorial (- x 1)) )) "
                + ") ) )";

            string call = "(eq (* 1 2 3 4 5 6 7 8 9 10) (factorial 10))";

            LispAssert.IsT(defFib + call);
        }


        [Test]
        public void Quine() //self-write program
        {
            string code = "((lambda (x) (list x (list 'quote x))) '(lambda (x) (list x (list 'quote x))))";
            Token[] tokens = new Lexer().GetTokens(code, false);

            string program = new Parser(tokens).Parse()[0].ToString() ;
            string executeResult = Interpreter.ExecuteOne(code).ToLispString();

            Assert.AreEqual(program, executeResult);
        }

        [Test]
        public void VarScopeTest()
        {
            string defun = "(set 'x-is-atom (lambda () (atom x) ))";
            string call = "((lambda (x) (x-is-atom)) '(1 2 3))";
            LispAssert.IsT(defun + call);
        }

    }
}
