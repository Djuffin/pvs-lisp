using System;
using System.Collections.Generic;
using System.Text;

namespace PVSLisp.Common
{
    public delegate LObject FunctionEvaluator(LispEnvironment env, LCell args);

    public sealed class SystemFunction : Function
    {
        private FunctionEvaluator evaluator;

        private static Symbol args = new Symbol("system call arguments list"); 

        public SystemFunction(FunctionEvaluator evaluator)
        {
            if (evaluator == null)
                throw new ArgumentNullException("evaluator");
            this.evaluator = evaluator;
        }


        public override LObject Call(LispEnvironment env, LCell arguments, bool tailCall)
        {
            return evaluator(env, arguments);
        }

        public override string ToLispString()
        {
            return ToString();
        }

        public override string ToString()
        {
            return "system function: " + evaluator.Method.Name;
        }
    }
}
