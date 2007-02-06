using System;
using System.Collections.Generic;
using System.Text;

namespace PVSLisp.Common
{
    class Closure : UserFunction
    {
        protected LispEnvironment baseEvironment;

        public Closure(LCell body, LCell parameters, LispEnvironment baseEnvironment)
            : base (body, parameters)
        {
            this.baseEvironment = baseEnvironment;
        }

        public override LispEnvironment BindParameters(LispEnvironment parent, LCell values)
        {
            if (baseEvironment != null) parent = baseEvironment;

            LispEnvironment result = base.BindParameters(parent, values);

            result.Transparent = true;
            return result;
        }

        public override string ToLispString()
        {
            string paramsText;
            if (parameters == null) paramsText = "()";
            else paramsText = parameters.ToLispString();

            return string.Format("(closure {0} {1})", paramsText, body.ToLispString());
        }

        public override string ToString()
        {
            return "closure: " + ToLispString();
        }

    }
}
