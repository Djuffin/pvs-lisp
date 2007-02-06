using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace PVSLisp.Lexer
{
    /// <summary>
    /// Represents a tokens harvester, which parse a sourse code to the tokens' collection.
    /// </summary>
    public class Lexer
    {
        private TokenInfo[] availableTokens = new TokenInfo[]
            {
                IntConstToken.GetInfo(),
                DoubleConstToken.GetInfo(),
                SymbolToken.GetInfo(),
                ListBeginingToken.GetInfo(),
                ListEndToken.GetInfo(),
                StringConstToken.GetInfo(),
                QuoteToken.GetInfo(),
                SpaceToken.GetInfo(),
                InvalidToken.GetInfo()
            };

        private Regex tokensParser;

        private Regex MakeParser()
        {
            string[] tokens = Array.ConvertAll<TokenInfo, string>(availableTokens, delegate(TokenInfo tokenInfo)
            {
                return string.Format("(?<{0}>{1})", tokenInfo.Name, tokenInfo.Pattern);
            });

            string result = string.Format(@"(?<Token>({0}))", string.Join("|", tokens));
            return new Regex(result, RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
        }

        public Lexer()
        {
            tokensParser = MakeParser();
        }

        public Token[] GetTokens(string text, bool includeSpaces)
        {
            List<Token> result = new List<Token>();
            foreach (Match m in tokensParser.Matches(text))
            {
                foreach (TokenInfo tokenInfo in availableTokens)
                {
                    string value = m.Groups[tokenInfo.Name].Value;
                    if (!string.IsNullOrEmpty(value))
                    {
                        TextRegion region = new TextRegion(m.Index, m.Length);    
                        Token token = null;
                        try
                        {
                            token = tokenInfo.Maker(value, region);
                        }
                        catch
                        {
                            token = InvalidToken.GetInfo().Maker(value, region);
                        }

                        if (includeSpaces || !(token is SpaceToken))
                            result.Add(token);
                        break;
                    }
                }
            }
            return result.ToArray();
        }

    }
}
