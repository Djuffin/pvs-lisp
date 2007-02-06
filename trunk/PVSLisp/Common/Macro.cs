using System;
using System.Collections.Generic;
using System.Text;


namespace PVSLisp.Common
{
    class Macro : UserFunction
    {
        public Macro(LCell body, LCell parameters)
            : base(body, parameters)
        {
        }

        protected override LObject EvaluateParameterValue(LispEnvironment env, LObject value)
        {
            return value;
        }

        public override LObject Evaluate(LispEnvironment env)
        {
            LObject expansionResult = base.Evaluate(env);
            return expansionResult.Evaluate(env.Parent);
        }

        public override string ToLispString()
        {
            string paramsText;
            if (parameters == null) paramsText = "()";
            else paramsText = parameters.ToLispString();

            return string.Format("(macro {0} {1})", paramsText, body.ToLispString());
        }

        public override string ToString()
        {
            return "macro: " + ToLispString();
        }
    }
}
