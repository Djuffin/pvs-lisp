using System;
using System.Collections.Generic;
using System.Text;
using PVSLisp.Lexer;

namespace PVSLisp.Parser
{
    public class ParserException : Exception
    {

        public Token Token;

        public ParserException(string message)
            : base(message)
        { 
        }

        public ParserException(string message, Token tok)
            : base(message)
        {
            Token = tok;
        }
    }
}
