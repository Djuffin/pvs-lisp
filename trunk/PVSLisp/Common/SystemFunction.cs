using System;
using System.Collections.Generic;
using System.Text;

namespace PVSLisp.Common
{
    public delegate LObject FunctionEvaluator(LispEnvironment env, LCell args);

    public class SystemFunction : Function
    {
        private FunctionEvaluator evaluator;

        private static Symbol args = new Symbol("system call arguments list"); 

        public SystemFunction(FunctionEvaluator evaluator)
        {
            if (evaluator == null)
                throw new ArgumentNullException("evaluator");
            this.evaluator = evaluator;
        }

        public override LispEnvironment BindParameters(LispEnvironment parent, LCell values)
        {
            LispEnvironment localEnv = parent; //we need it for ability access to local variables of parent for late binding
            
            if (values != null)
                localEnv.AssignLocalSymbol(args, values);
            else
                localEnv.AssignLocalSymbol(args, SpecialValues.NIL);

            return localEnv;
        }

        public override LObject Evaluate(LispEnvironment env)
        {
            LCell argsList = args.Evaluate(env) as LCell;

            //Console.WriteLine(string.Format("log: [system call] {0} {1}", evaluator.Method.Name, argsList));

            return evaluator(env, argsList);
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
