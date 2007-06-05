using System;
using System.Collections.Generic;
using System.Text;
using PVSLisp.Common;
using PVSLisp.Lexer;

namespace PVSLisp.Parser
{

    public class Parser
    {
        TokensSource source;

        public Parser(TokensSource source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            this.source = source;
        }

        public Parser(IEnumerable<Token> tokens)
            :this (new TokensSource(tokens))
        {
        }

        public LObject[] Parse()
        {
            List<LObject> result = new List<LObject>();
            while (!source.IsItEnd)
            {
                result.Add(ParseOne(source.GetNextToken()));
            }

            return result.ToArray();
        }

        private LObject ParseOne(Token tok )
        {
            if (tok is ConstToken)
                return ParseConst(tok as ConstToken);

            if (tok is SymbolToken)
                return ParseSymbol(tok as SymbolToken);

            if (tok is ListBeginingToken)
                return ParseListWithoutStart();

            if (tok is QuoteToken)
                return ParseQuoteToken(tok as QuoteToken);

            if (tok is ListEndToken)
                RiseError("Unexpected end of a list", tok);

            if (tok is InvalidToken)
                RiseError("Invalid token", tok);

            if (tok is SpaceToken)
                throw new Exception("There is SpaceToke in the parser's input");

            throw new Exception("Unknown token");
        }

        private LObject ParseQuoteToken(QuoteToken tok)
        {
            if (source.IsItEnd)
                throw new LispException("Unexpected end of a list");

            LObject obj = ParseOne(source.GetNextToken());
            
            Symbol quote = tok.FunctionQuote ? new Symbol("function") : new Symbol("quote");

            return LCell.Make(new LObject[] { quote, obj });
        }

        private LObject ParseSymbol(SymbolToken symbolToken)
        {
            return new Symbol(symbolToken.Text);
        }

        private LObject ParseConst(ConstToken tok)
        {
            if (tok is IntConstToken)
                return ScalarFactory.Make((tok as IntConstToken).Value);

            if (tok is DoubleConstToken)
                return ScalarFactory.Make((tok as DoubleConstToken).Value);

            if (tok is StringConstToken)
                return ScalarFactory.Make((tok as StringConstToken).Value);

            throw new Exception("Unknown constant token");
        }

        private LObject ParseListWithoutStart()
        {
            List<LObject> list = new List<LObject>();
            for(;;)
            {
                if (source.IsItEnd)
                    RiseError("Unexpected end of a code");

                Token tok = source.GetNextToken();
                if (tok is ListEndToken) break;

                list.Add(ParseOne(tok));
            }

            LObject result = LCell.Make(list);
            return result ?? SpecialValues.NIL;
        }

        private void RiseError(string message, Token tok)
        {
            throw new ParserException(message, tok);
        }

        private void RiseError(string message)
        {
            throw new ParserException(message);
        }

    }
}
