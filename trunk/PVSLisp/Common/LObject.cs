using System;
using System.Collections.Generic;
using System.Text;

namespace PVSLisp.Common
{
    public abstract class LObject
    {
        public abstract LObject Evaluate(LispEnvironment env);

        public virtual LObject TailEvaluate(LispEnvironment env)
        {
            return Evaluate(env);
        }

        public virtual string ToLispString()
        {
            return ToString();
        }

    }
}
