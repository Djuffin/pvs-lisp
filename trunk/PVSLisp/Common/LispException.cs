using System;
using System.Collections.Generic;
using System.Text;

namespace PVSLisp.Common
{
    public class LispException : Exception
    {
        public LispException()
            : base()
        {
        }

        public LispException(string message)
            : base(message)
        { 
        }

        public LispException(string message, Exception e)
            : base(message, e)
        {
        }
    }

    public class WrongFunctionParams : LispException
    {
        public WrongFunctionParams(string message)
            : base(message)
        { 
        }
    }
}
