using System;
using System.Collections.Generic;
using System.Text;

namespace PVSLisp.Common
{
    public class LateEvaluator : LObject
    {
        private LObject targetObject;
        private LispEnvironment targetEnv;

        private LateEvaluator(LispEnvironment env, LObject target)
        {
            this.targetEnv = env;
            this.targetObject = target;
            //Log.LateEvaluatorCreate(this);
        }

        public static LObject Create(LispEnvironment env, LObject target)
        {
            return new LateEvaluator(env, target);

            //return target.TailEvaluate(env);

            //LateEvaluator result = target as LateEvaluator;
            //if (result == null)
            //    result = new LateEvaluator(env, target);
            //return result;
        }

        private LObject EvaluateOnce()
        {
            return targetObject.TailEvaluate(targetEnv);
        }

        public override LObject Evaluate(LispEnvironment env)
        {
            LObject result;
            LateEvaluator evaluator = this;
            do
            {
                result = evaluator.EvaluateOnce();
                evaluator = result as LateEvaluator;
            } while (evaluator != null);
            return result;
        }

        public override LObject TailEvaluate(LispEnvironment env)
        {
            return this;
        }
    }
}
