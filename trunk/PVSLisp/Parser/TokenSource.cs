using System;
using System.Collections.Generic;
using System.Text;
using PVSLisp.Lexer;

namespace PVSLisp.Parser
{
    public class TokensSource
    {
        private IEnumerator<Token> enumerator;
        private bool isItEnd;

        public TokensSource(IEnumerable<Token> tokens)
        {
            if (tokens == null)
                throw new ArgumentNullException("tokens");

            enumerator = tokens.GetEnumerator();
            isItEnd = !enumerator.MoveNext();
        }

        public bool IsItEnd
        {
            get
            {
                return isItEnd;
            }
        }

        public Token GetNextToken()
        {
            if (isItEnd)
                throw new InvalidOperationException("There are no tokens the source");

            Token result = enumerator.Current;
            isItEnd = !enumerator.MoveNext();
            return result;
        }
    }
}
