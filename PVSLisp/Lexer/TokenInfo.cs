using System;
using System.Collections.Generic;
using System.Text;

namespace PVSLisp.Lexer
{
    public delegate Token TokenMaker(string text, TextRegion region);

    public class TokenInfo
    {
        public readonly string Pattern;
        public readonly string Name;
        public readonly TokenMaker Maker;

        public TokenInfo(string Pattern, string Name, TokenMaker Maker)
        {
            this.Pattern = Pattern;
            this.Name = Name;
            this.Maker = Maker;
        }

    }
}
