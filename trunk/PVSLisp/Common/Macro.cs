using System;
using System.Collections.Generic;
using System.Text;


namespace PVSLisp.Common
{
    class Macro : Closure
    {
        public Macro(LCell body, LCell parameters)
            : base(body, parameters)
        {
        }

        protected override LObject EvaluateParameterValue(LispEnvironment env, LObject value)
        {
            return value;
        }

        protected override LispEnvironment GetLocalEnvironment(LispEnvironment parent, bool tailCall)
        {
            return new LispEnvironment(parent);
            //return tailCall ? parent : new LispEnvironment(parent);
        }

        public override LObject Call(LispEnvironment env, LCell arguments, bool tailCall)
        {
            tailCall = false; //Tail call is not supported for Macro now

			LObject expansionResult = Expand(env, arguments, tailCall);
            return LateEvaluator.Create(env, expansionResult);
            //return expansionResult.TailEvaluate(env);
        }

		public LObject Expand(LispEnvironment env, LCell arguments, bool tailCall)
		{
			LispEnvironment localEnv = GetLocalEnvironment(env, tailCall);

			BindParameters(env, arguments, localEnv, tailCall);

			return body.Evaluate(localEnv);
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
