using System;
using System.Collections.Generic;
using System.Text;

namespace PVSLisp.Common
{

    internal static class SpecialFunctions
    {
        #region Helpers
        private static List<LObject> EvaluateArgs(LispEnvironment env, LCell args)
        {
            List<LObject> result = new List<LObject>();
            if (args != null)
                foreach (LObject arg in args)
                    result.Add(arg.Evaluate(env));
            return result;
        }

        private static double GetNumber(LObject arg, ref bool isDouble)
        {
            Scalar value = arg as Scalar;

            if (value == null || value.Value == null)
                throw new LispException("Bad argument type. It should be scalar.");

            try
            {
                if (!isDouble && (value.Type == typeof(double) || value.Type == typeof(float)))
                    isDouble = true;

                return Convert.ToDouble(value.Value);

            }
            catch (InvalidCastException e)
            {
                throw new LispException("Bad argument type. It should be number.", e);
            }
        }


        private static void AssertParamsCount(LCell args, int n)
        {
            int count = args == null ? 0 : args.Count;

            if (count < n)
                throw new WrongFunctionParams("Too few arguments");

            if (count > n)
                throw new WrongFunctionParams("Too many arguments");
        }

        private static void AssertParamsCount(LCell args, int min, int max)
        {
            int count = args == null ? 0 : args.Count;

            if (min != -1 && count < min)
                throw new WrongFunctionParams("Too few arguments");

            if (max != -1 && count > max)
                throw new WrongFunctionParams("Too many arguments");
        }

        #endregion


        #region Print
        public static SystemFunction Print_ = new SystemFunction(Print);
        private static LObject Print(LispEnvironment env, LCell args)
        {
            foreach (LObject arg in EvaluateArgs(env, args))
            {
                Console.Write(arg.ToString());
            }

            return SpecialValues.NIL;
        }
        #endregion

        #region Lisp Native Functions
        public class LispNative
        {

            #region Atom
            public static SystemFunction Atom_ = new SystemFunction(Atom);
            private static LObject Atom(LispEnvironment env, LCell args)
            {
                AssertParamsCount(args, 1);

                List<LObject> argList = EvaluateArgs(env, args);

                if (argList[0] is Atom)
                    return SpecialValues.T;
                else
                    return SpecialValues.NIL;
            }
            #endregion

            #region Null
            public static SystemFunction Null_ = new SystemFunction(Null);
            private static LObject Null(LispEnvironment env, LCell args)
            {
                AssertParamsCount(args, 1);

                List<LObject> argList = EvaluateArgs(env, args);

                if (SpecialValues.NIL.Equals(argList[0]))
                    return SpecialValues.T;
                else
                    return SpecialValues.NIL;
            }
            #endregion

            #region Cond
            public static SystemFunction Cond_ = new SystemFunction(Cond);
            private static LObject Cond(LispEnvironment env, LCell args)
            {
                foreach (LObject conObj in args)
                {
                    LCell conditioin = conObj as LCell;
                    if (conditioin == null || conditioin.Count != 2)
                        throw new LispException("All arguments of COND should be consitions");

                    if (!SpecialValues.NIL.Equals(conditioin.Head.Evaluate(env)))
                        return conditioin.Tail.Head.Evaluate(env);
                }

                return SpecialValues.NIL;

            }
            #endregion

            #region Closure
            public static SystemFunction Closure_ = new SystemFunction(Closure);
            private static LObject Closure(LispEnvironment env, LCell args)
            {
                AssertParamsCount(args, 2);

                LCell funcArgs = args.Head as LCell;
                LCell body = args.Tail.Head as LCell;

                return new Closure(body, funcArgs, env);
            }
            #endregion

            #region Lambda
            public static SystemFunction Lambda_ = new SystemFunction(Lambda);
            private static LObject Lambda(LispEnvironment env, LCell args)
            {
                AssertParamsCount(args, 2);

                LCell funcArgs = args.Head as LCell;
                LCell body = args.Tail.Head as LCell;

                return new UserFunction(body, funcArgs);
            }
            #endregion

            #region Macro
            public static SystemFunction Macro_ = new SystemFunction(Macro);
            private static LObject Macro(LispEnvironment env, LCell args)
            {
                AssertParamsCount(args, 2);

                LCell funcArgs = args.Head as LCell;
                LCell body = args.Tail.Head as LCell;

                return new Macro(body, funcArgs);
            }
            #endregion

            #region Eval
            public static SystemFunction Eval_ = new SystemFunction(Eval);
            private static LObject Eval(LispEnvironment env, LCell args)
            {
                AssertParamsCount(args, 1);

                LObject result = args.Head.Evaluate(env);

                if (result != null)
                    return result.Evaluate(env);

                return null;
            }
            #endregion

            #region Quote
            public static SystemFunction Quote_ = new SystemFunction(Quote);
            public static LObject Quote(LispEnvironment env, LCell args)
            {
                AssertParamsCount(args, 1);

                return args.Head;
            }
            #endregion

            #region Set
            public static SystemFunction Set_ = new SystemFunction(Set);
            private static LObject Set(LispEnvironment env, LCell args)
            {
                AssertParamsCount(args, 2);

                List<LObject> argList = EvaluateArgs(env, args);

                Symbol target = argList[0] as Symbol;
                if (target == null)
                    throw new LispException("Bad type of first argument");

                env.Assign(target, argList[1]);

                return argList[1];
            }
            #endregion

            #region Eq
            public static SystemFunction Eq_ = new SystemFunction(Eq);
            private static LObject Eq(LispEnvironment env, LCell args)
            {
                AssertParamsCount(args, 2);

                List<LObject> argList = EvaluateArgs(env, args);

                if (object.Equals(argList[0], argList[1]))
                    return SpecialValues.T;
                else
                    return SpecialValues.NIL;
            }
            #endregion

        }
        #endregion

        #region List processing
        public class ListProcessing
        {


            #region Cons
            public static SystemFunction Cons_ = new SystemFunction(Cons);
            private static LObject Cons(LispEnvironment env, LCell args)
            {
                AssertParamsCount(args, 2);
                List<LObject> arguments = EvaluateArgs(env, args);

                LObject head = arguments[0];

                if (arguments[1] == SpecialValues.NIL)
                    return new LCell(head);

                LCell tail = arguments[1] as LCell;

                if (tail == null)
                    throw new LispException("Second argument of cons should be list");

                return new LCell(head, tail);
            }
            #endregion


            #region List
            public static SystemFunction List_ = new SystemFunction(List);
            private static LObject List(LispEnvironment env, LCell args)
            {
                if (args == null) return SpecialValues.NIL;
                return LCell.Make(EvaluateArgs(env, args));
            }
            #endregion

            #region Car
            public static SystemFunction Car_ = new SystemFunction(Car);
            private static LObject Car(LispEnvironment env, LCell args)
            {
                AssertParamsCount(args, 1);

                LCell cell = EvaluateArgs(env, args)[0] as LCell;

                if (cell == null)
                    throw new LispException("CAR is undefied for atom");

                return cell.Head;
            }
            #endregion

            #region Cdr
            public static SystemFunction Cdr_ = new SystemFunction(Cdr);
            private static LObject Cdr(LispEnvironment env, LCell args)
            {
                AssertParamsCount(args, 1);

                LCell cell = EvaluateArgs(env, args)[0] as LCell;

                if (cell == null)
                    throw new LispException("CDR is undefied for atom");

                if (cell.Tail == null) return SpecialValues.NIL;

                return cell.Tail;
            }

            #endregion

        }
        #endregion

        #region Arithmetic Functions
        public class Arithmetic
        {

            #region Add
            public static SystemFunction Add_ = new SystemFunction(Add);
            private static LObject Add(LispEnvironment env, LCell args)
            {
                double result = 0;
                bool isDouble = false;

                foreach (LObject arg in EvaluateArgs(env, args))
                    result += GetNumber(arg, ref isDouble);

                return isDouble ? ScalarFactory.Make(result) : ScalarFactory.Make((int)result);
            }
            #endregion

            #region Subsract
            public static SystemFunction Subsract_ = new SystemFunction(Subsract);
            private static LObject Subsract(LispEnvironment env, LCell args)
            {
                AssertParamsCount(args, 2, -1);
                List<LObject> argList = EvaluateArgs(env, args);

                bool isDouble = false;
                double result = GetNumber(argList[0], ref isDouble); ;

                for (int i = 1; i < argList.Count; i++)
                    result -= GetNumber(argList[i], ref isDouble);

                return isDouble ? ScalarFactory.Make(result) : ScalarFactory.Make((int)result);
            }
            #endregion

            #region Multiply
            public static SystemFunction Multiply_ = new SystemFunction(Multiply);
            private static LObject Multiply(LispEnvironment env, LCell args)
            {
                double result = 1;
                bool isDouble = false;

                foreach (LObject arg in EvaluateArgs(env, args))
                    result *= GetNumber(arg, ref isDouble);

                return isDouble ? ScalarFactory.Make(result) : ScalarFactory.Make((int)result);
            }
            #endregion

            #region Division
            public static SystemFunction Division_ = new SystemFunction(Division);
            private static LObject Division(LispEnvironment env, LCell args)
            {
                AssertParamsCount(args, 2, -1);

                List<LObject> argList = EvaluateArgs(env, args);

                bool isDouble = true;
                double result = GetNumber(argList[0], ref isDouble); ;

                for (int i = 1; i < argList.Count; i++)
                    result /= GetNumber(argList[i], ref isDouble);

                return ScalarFactory.Make(result);
            }
            #endregion
        }

        #endregion

        #region Logical Functions
        public class Logical
        {

            #region And
            public static SystemFunction And_ = new SystemFunction(And);
            private static LObject And(LispEnvironment env, LCell args)
            {
                AssertParamsCount(args, 1, -1);

                foreach (LObject o in EvaluateArgs(env, args))
                    if (SpecialValues.NIL.Equals(o))
                        return SpecialValues.NIL;

                return SpecialValues.T;
            }
            #endregion

            #region Or
            public static SystemFunction Or_ = new SystemFunction(Or);
            private static LObject Or(LispEnvironment env, LCell args)
            {
                AssertParamsCount(args, 1, -1);

                foreach (LObject o in EvaluateArgs(env, args))
                    if (!SpecialValues.NIL.Equals(o))
                        return SpecialValues.T;

                return SpecialValues.NIL;
            }
            #endregion

            #region Not
            public static SystemFunction Not_ = new SystemFunction(Not);
            private static LObject Not(LispEnvironment env, LCell args)
            {
                AssertParamsCount(args, 1);

                return SpecialValues.NIL.Equals(args.Head.Evaluate(env)) ? SpecialValues.T : SpecialValues.NIL;
            }
            #endregion
        }
        #endregion

        #region .NET support
        public class DotNetSupport
        {
            private static object[] ConvertArgs(List<LObject> args)
            {
                return args.ConvertAll<object>(delegate(LObject o)
                {
                    Scalar s = o as Scalar;
                    if (s != null) return s.Value;

                    return o;
                    //throw new WrongFunctionParams("Unexpected argument in .NET call");
                }).ToArray();
            }


            #region New
            public static SystemFunction New_ = new SystemFunction(New);
            private static LObject New(LispEnvironment env, LCell args)
            {
                AssertParamsCount(args, 1, -1);

                object[] constructorArgs = ConvertArgs(EvaluateArgs(env, args.Tail));

                Symbol targetType = args.Head.Evaluate(env) as Symbol;

                if (targetType == null)
                    throw new WrongFunctionParams("First agrument of new must can be evaluated to a symbol");

                return ScalarFactory.Make(DotNet.DotNetInterop.New(targetType.Name, constructorArgs));

            }
            #endregion

            #region Call
            public static SystemFunction Call_ = new SystemFunction(Call);
            private static LObject Call(LispEnvironment env, LCell args)
            {
                AssertParamsCount(args, 2, -1);

                object[] methodArgs = ConvertArgs(EvaluateArgs(env, args.Tail.Tail));

                Symbol targetMethod = args.Head.Evaluate(env) as Symbol;

                if (targetMethod == null)
                    throw new WrongFunctionParams("First agrument of new must can be evaluated to a symbol");

                LObject ThisOrType = args.Tail.Head.Evaluate(env);
                Symbol typeName = ThisOrType as Symbol;
                Scalar scalarThis = ThisOrType as Scalar;

                if (typeName != null)
                    return ScalarFactory.Make(DotNet.DotNetInterop.CallStatic(typeName.Name, targetMethod.Name, methodArgs));

                if (scalarThis != null)
                    return ScalarFactory.Make(DotNet.DotNetInterop.Call(scalarThis.Value, targetMethod.Name, methodArgs));
                else if (ThisOrType != null)
                    return ScalarFactory.Make(DotNet.DotNetInterop.Call(ThisOrType, targetMethod.Name, methodArgs));

                throw new WrongFunctionParams("Cannot process .NET call, this == null");

            }

            #endregion

            #region Throw
            public static SystemFunction Throw_ = new SystemFunction(Throw);
            private static LObject Throw(LispEnvironment env, LCell args)
            {
                AssertParamsCount(args, 1);

                List<LObject> argList = EvaluateArgs(env, args);

                throw new LispException(argList[0].ToString());
            }

            #endregion
        }
        #endregion
    }


}
