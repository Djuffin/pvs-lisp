using System;
using System.Collections.Generic;
using System.Text;

namespace PVSLisp.Common
{
    public abstract class Function : LObject
    {
        public abstract LObject Call(LispEnvironment env, LCell arguments, bool tailCall);

        public override LObject Evaluate(LispEnvironment env)
        {
            throw new NotImplementedException("Do not call Evaluate(LispEnvironment env) for " + GetType().ToString());
        }

        public override LObject TailEvaluate(LispEnvironment env)
        {
            throw new NotImplementedException("Do not call TailEvaluate(LispEnvironment env) for " + GetType().ToString());
        }

    }
}
