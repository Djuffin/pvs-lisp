using System;
using System.Collections.Generic;
using System.Text;

namespace PVSLisp.Lexer
{
    public class TextRegion
    {
        public static TextRegion Empty = new TextRegion();

        public readonly bool IsEmpty;
        public readonly int Start, End;
        public TextRegion (int start, int end)
        {
            Start = start;
            End = end;
            IsEmpty = false;
        }

        public TextRegion (int place)
        {
            Start = place;
            End = place;
            IsEmpty = false;
        }

        //empty region
        private TextRegion ()
        {
            IsEmpty = true;
        }

        public static TextRegion operator | (TextRegion reg1, TextRegion reg2)
        {
            if (reg1.IsEmpty)
                return reg2;
            if (reg2.IsEmpty)
                return reg1;
            return new TextRegion(Math.Min(reg1.Start, reg2.Start), Math.Max(reg1.End, reg2.End));
        }


        public override string ToString ()
        {
            if (IsEmpty)
                return "[]";
            return string.Format("[{0} - {1}]", Start, End);
        }

        public string ToString (string text)
        {
            if (IsEmpty)
                return "[]";
            int lines = 1;
            int lastNewLine = 0;
            int index;
            for (index = 0; index < Start; index++)
                if (text[index] == '\x0d')
                {
                    lines++;
                    lastNewLine = index + 1;
                }
            string result = string.Format("[({0}, {1}) - ", lines, index - lastNewLine);
            for (; index < End; index++)
                if (text[index] == '\x0d')
                {
                    lines++;
                    lastNewLine = index + 1;
                }
            return result + string.Format("({0}, {1})]", lines, index - lastNewLine);
        }

        public TextRegion Clone ()
        {
            if (IsEmpty)
                return new TextRegion();
            return new TextRegion(Start, End);
        }

    }
}
