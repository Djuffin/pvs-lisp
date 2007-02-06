using System;
using System.Collections.Generic;
using System.Text;

namespace PVSLisp.Common
{
    class UserFunction : Function 
    {
        protected LCell body;
        protected LCell parameters;

        public UserFunction(LCell body, LCell parameters)
        {
            if (body == null)
                throw new LispException("Body cannot be empty.");

            this.body = body;
            this.parameters = parameters;

            if (parameters != null)
            {
                foreach (LObject o in parameters)
                    if (!(o is Symbol))
                        throw new LispException("Bad format of parameters list. Each item must be symbol.");
            }
        }

        protected virtual LObject EvaluateParameterValue(LispEnvironment env, LObject value)
        {
            return value.Evaluate(env);
        }

        public override LispEnvironment BindParameters(LispEnvironment parent, LCell values)
        {
            int pCount = parameters != null ? parameters.Count : 0;
            int vCount = values != null ? values.Count : 0;
            if (pCount > vCount)
                throw new LispException("Too few arguments for " + ToLispString());
            if (pCount < vCount)
                throw new LispException("Too many arguments for " + ToLispString());

            LispEnvironment localEnv = new LispEnvironment(parent);

            IEnumerator<LObject> vEnumerator = values.GetEnumerator();
            vEnumerator.MoveNext();
            foreach (Symbol s in parameters)
            {
                localEnv.AssignLocalSymbol(s, EvaluateParameterValue(parent, vEnumerator.Current));
                vEnumerator.MoveNext();
            }

            return localEnv;
        }

        public override LObject Evaluate(LispEnvironment env)
        {
            return body.Evaluate(env);
        }

        public override string ToLispString()
        {
            string paramsText;
            if (parameters == null) paramsText = "()";
            else paramsText = parameters.ToLispString();

            return string.Format("(lambda {0} {1})", paramsText, body.ToLispString() );
        }

        public override string ToString()
        {
            return "function: " + ToLispString();
        }
    }
}
