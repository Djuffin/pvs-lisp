using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace PVSLisp.Lexer
{
    /// <summary>
    /// Abstract token. It is the piace of a source code, that have place and inner text
    /// </summary>
    public abstract class Token
    {
        protected TextRegion region;

        /// <summary>
        /// Token inner text
        /// </summary>
        public abstract string Text
        {
            get;
        }



        /// <summary>
        /// Region of this token
        /// </summary>
        public virtual TextRegion Region
        {
            get
            {
                return region;
            }
        }


    }

    public class SymbolToken : Token
    {
        string name;
        private SymbolToken(string Text, TextRegion region)
        {
            this.region = region;
            this.name = Text;
        }

        public override string Text
        {
            get
            {
                return name;
            }
        }

        private const string Pattern = @"( ([a-zA-Z_+\-*/@$%&,.=?\[\]] ) (\w | [_+\-*/@$%&,.=?\[\]] | \d)* )";

        private static Token Make(string Text, TextRegion region)
        {
            return new SymbolToken(Text, region);
        }

        public static TokenInfo GetInfo()
        {
            return new TokenInfo(Pattern, "SymbolToken", Make);
        }
    }

    public class InvalidToken : Token
    {
        string text;
        private InvalidToken(string Text, TextRegion region)
        {
            this.region = region;
            this.text = Text;
        }

        public override string Text
        {
            get
            {
                return text;
            }
        }

        private const string Pattern = @"( (.*?) (?=(\s | $ | [()])) )";

        private static Token Make(string Text, TextRegion region)
        {
            return new InvalidToken(Text, region);
        }

        public static TokenInfo GetInfo()
        {
            return new TokenInfo(Pattern, "InvalidToken", Make);
        }
    }

    public abstract class ConstToken : Token
    {

    }

    public class IntConstToken : ConstToken
    {
        int constValue;
        private IntConstToken(string Text, TextRegion region)
        {
            this.region = region;
            constValue = int.Parse(Text, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
        }

        public int Value
        {
            get
            {
                return constValue;
            }
        }

        public override string Text
        {
            get
            {
                return constValue.ToString();
            }
        }


        private const string Pattern = @" -? (\d+) (?=(\s | $ | [()]))";

        private static IntConstToken Make(string Text, TextRegion region)
        {
            return new IntConstToken(Text, region);
        }

        public static TokenInfo GetInfo()
        {
            return new TokenInfo(Pattern, "IntConstToken", Make);
        }
    }

    public class DoubleConstToken : ConstToken
    {
        double constValue;
        private DoubleConstToken(string Text, TextRegion region)
        {
            this.region = region;
            constValue = double.Parse(Text, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
        }

        public double Value
        {
            get
            {
                return constValue;
            }
        }

        public override string Text
        {
            get
            {
                return constValue.ToString();
            }
        }

        private const string Pattern = @" -? (\d+\.\d*) (?=(\s | $ | [()]))";

        private static DoubleConstToken Make(string Text, TextRegion region)
        {
            return new DoubleConstToken(Text, region);
        }

        public static TokenInfo GetInfo()
        {
            return new TokenInfo(Pattern, "DoubleConstToken", Make);
        }

    }

    public class StringConstToken : ConstToken
    {
        string constValue;
        private StringConstToken(string Text, TextRegion region)
        {
            this.region = region;
            constValue = Text;
            DecodeString(constValue);
        }

        public string Value
        {
            get
            {
                return DecodeString(constValue);
            }
        }

        public override string Text
        {
            get
            {
                return constValue;
            }
        }

        private const string Pattern = @"( "" ([^""\\] | \\\\ | \\"" | \\\w  )*?  "" )";

        private static StringConstToken Make(string Text, TextRegion region)
        {
            return new StringConstToken(Text, region);
        }

        public static TokenInfo GetInfo()
        {
            return new TokenInfo(Pattern, "StringConstToken", Make);
        }

        private static string DecodeString(string input)
        {
            StringBuilder result = new StringBuilder();
            char prevChar = input[0];
            if (input[0] != '"' || input[input.Length - 1] != '"')
                throw new Exception("It is not a string constant.");
            for (int index = 1; index < input.Length - 1; index++) //skip " at the string start and end
            {
                char c = input[index];
                if (prevChar == '\\')
                {
                    switch (c)
                    {
                        case '"': break;
                        case '\\':
                            result.Append('\\');
                            prevChar = '\x0';
                            continue;
                        case 'n':
                            result.Append('\n');
                            prevChar = '\x0';
                            continue;
                        case 't':
                            result.Append('\t');
                            prevChar = '\x0';
                            continue;
                        case 'r':
                            result.Append('\r');
                            prevChar = '\x0';
                            continue;
                        default:
                            throw new Exception("Unsupported escape character: \\" + c);
                    }
                }
                if (c != '\\') 
                    result.Append(c);
                prevChar = c;
            }

            return result.ToString();
        }
    
    }

    public class SpaceToken : Token
    {
        public readonly bool IsComment;
        private string text;

        public override string Text
        {
            get
            {
                return text;
            }
        }

        private SpaceToken(string text, TextRegion region)
        {
            this.region = region;
            this.text = text;
            this.IsComment = text.Contains(";");
        }

        private const string Pattern = @" (\s+ | ;.*$) ";

        private static SpaceToken Make(string Text, TextRegion region)
        {
            return new SpaceToken(Text, region);
        }

        public static TokenInfo GetInfo()
        {
            return new TokenInfo(Pattern, "SpaceToken", Make);
        }

    }

    public class QuoteToken : Token
    {
        public override string Text
        {
            get { return "`"; }
        }

        private QuoteToken(int position)
        {
            this.region = new TextRegion(position);
        }

        private const string Pattern = @" ([`']) ";

        private static QuoteToken Make(string Text, TextRegion region)
        {
            return new QuoteToken(region.Start);
        }

        public static TokenInfo GetInfo()
        {
            return new TokenInfo(Pattern, "QuoteToken", Make);
        }
    }

    public class ListBeginingToken : Token
    {

        public override string Text
        {
            get { return "("; }
        }

        private ListBeginingToken(int position)
        {
            this.region = new TextRegion(position);
        }

        private const string Pattern = @" ([(]) ";

        private static ListBeginingToken Make(string Text, TextRegion region)
        {
            return new ListBeginingToken(region.Start);
        }

        public static TokenInfo GetInfo()
        {
            return new TokenInfo(Pattern, "ListBeginingToken", Make);
        }

    }

    public class ListEndToken : Token
    {
        public override string Text
        {
            get { return ")"; }
        }

        private ListEndToken(int position)
        {
            this.region = new TextRegion(position);
        }

        private const string Pattern = @" ([)]) ";

        private static ListEndToken Make(string Text, TextRegion region)
        {
            return new ListEndToken(region.Start);
        }

        public static TokenInfo GetInfo()
        {
            return new TokenInfo(Pattern, "ListEndToken", Make);
        }

    }
}

