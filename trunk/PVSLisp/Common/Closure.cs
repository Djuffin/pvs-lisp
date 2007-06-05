using System;
using System.Collections.Generic;
using System.Text;

namespace PVSLisp.Common
{
    class Closure : Function
    {
        protected LCell body;
        protected LCell parameters;
        protected LispEnvironment capturedEnvironment;

        public Closure(LCell body, LCell parameters)
            : this(body, parameters, null)
        {
        }

        public Closure(Closure prototype, LispEnvironment capturedEnvironment)
            : this(prototype.body, prototype.parameters, capturedEnvironment)
        {
        }

        public Closure(LCell body, LCell parameters, LispEnvironment capturedEnvironment)
        {
            if (body == null)
                throw new LispException("Body cannot be empty.");

            this.body = body;
            this.parameters = parameters;
            this.capturedEnvironment = capturedEnvironment;

            if (parameters != null)
            {
                foreach (LObject o in parameters)
                    if (!(o is Symbol))
                        throw new LispException("Bad format of parameters list. Each item must be symbol.");
            }
        }

        public override LObject Call(LispEnvironment env, LCell arguments, bool tailCall)
        {
            LispEnvironment localEnv = GetLocalEnvironment(env, tailCall);
            BindParameters(env, arguments, localEnv, tailCall);
            
            return LateEvaluator.Create(localEnv, body);
            //return body.TailEvaluate(localEnv);
        }

        protected virtual LObject EvaluateParameterValue(LispEnvironment env, LObject value)
        {
            return value.Evaluate(env);
        }


        protected virtual LispEnvironment GetLocalEnvironment(LispEnvironment parent, bool tailCall)
        {
            LispEnvironment localEnv;
            if (capturedEnvironment == null)
            {
                //it's simple function
                if (tailCall)
                {
                    localEnv = parent;
                }
                else
                    localEnv = new LispEnvironment(parent);
            }
            else
            {
                //it's closure i.e. function with captured environment
                localEnv = new LispEnvironment(capturedEnvironment);
                localEnv.Transparent = true;
            }
            return localEnv;
        }

        protected void BindParameters(LispEnvironment parent, LCell values, LispEnvironment localEnv, bool clearLocals)
        {
            LCell argsNames = parameters;
            LCell argsValues = values;

            List<KeyValuePair<Symbol, LObject>> newLocalVars = new List<KeyValuePair<Symbol, LObject>>();

            while (argsNames != null)
            {
                Symbol p = argsNames.Head as Symbol;

                switch (p.Name)
                {
                    //Add here &key and &opetional 
                    case "&rest":
                        if (argsValues == null)
                        {
                            localEnv.AssignLocalSymbol(p, SpecialValues.NIL);
                        }
                        else
                        {
                            List<LObject> evalutedArgs = new List<LObject>(argsValues);
                            for (int i = 0; i < evalutedArgs.Count; i++)
                                evalutedArgs[i] = EvaluateParameterValue(parent, evalutedArgs[i]);

                            newLocalVars.Add(new KeyValuePair<Symbol, LObject>(p, LCell.Make(evalutedArgs)));
                        }
                        argsNames = null;
                        argsValues = null;
                        break;
                    default:
                        if (argsValues == null)
                            throw new LispException("Not few parameters given.");

                        newLocalVars.Add(new KeyValuePair<Symbol, LObject>(p, EvaluateParameterValue(parent, argsValues.Head)));
                        argsNames = argsNames.Tail;
                        argsValues = argsValues.Tail;
                        break;
                }

            }

            if (argsValues != null)
                throw new LispException("Too many paramentes given");

            if (clearLocals) localEnv.ClearLocals();
            foreach (KeyValuePair<Symbol, LObject> localVar in newLocalVars)
                localEnv.AssignLocalSymbol(localVar.Key, localVar.Value);

        }

        public override string ToLispString()
        {
            string paramsText;
            if (parameters == null) paramsText = "()";
            else paramsText = parameters.ToLispString();

            string template;
            if (capturedEnvironment == null)
                template = "(lambda {0} {1})";
            else
                template = "(function (lambda {0} {1}))";

            return string.Format(template, paramsText, body.ToLispString());
        }

        public override string ToString()
        {
            return "function: " + ToLispString();
        }
    }
}
