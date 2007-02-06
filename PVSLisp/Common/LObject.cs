using System;
using System.Collections.Generic;
using System.Text;

namespace PVSLisp.Common
{
    public abstract class LObject
    {
        public abstract LObject Evaluate(LispEnvironment env);

        public virtual string ToLispString()
        {
            return ToString();
        }

    }
}
