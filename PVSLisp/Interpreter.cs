using System;
using System.Collections.Generic;
using System.Text;
using PVSLisp.Common;
using PVSLisp.Lexer;
using PVSLisp.Parser;

namespace PVSLisp
{
    public class Interpreter
    {
        private Runtime runtime;
        private GlobalEnvironment environment;
        private Lexer.Lexer lexer = new Lexer.Lexer();

        public Interpreter()
        {
            runtime = new Runtime();
            environment = new GlobalEnvironment(runtime);
        }

        public LObject Execute(string text)
        {
            Token[] tokens = lexer.GetTokens(text, false);
            LObject[] commands = new Parser.Parser(tokens).Parse();

            LObject result = null;
            foreach (LObject cmd in commands)
                result = cmd.Evaluate(environment);

            return result;
        }


        public static LObject ExecuteOne(string text)
        {
            return new Interpreter().Execute(text);
        }
    }
}
