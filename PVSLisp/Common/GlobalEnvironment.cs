using System;
using System.Collections.Generic;
using System.Text;

namespace PVSLisp.Common
{
    public class GlobalEnvironment : LispEnvironment
    {
        private Dictionary<Symbol, object> constsSet = new Dictionary<Symbol, object>();

        public GlobalEnvironment(Runtime runtime)
            : base(runtime)
        {
            InitConstants();
        }

        protected override void AssignInternal(Symbol s, LObject val, bool isLocal)
        {
            if (isLocal)
                throw new Exception("GlobalEnvironment cannot contains local variables");

            if (IsItConst(s))
                throw new LispException("Assigning value to the constant: " + s);

            base.AssignInternal(s, val, false);
        }

        public override void AssignGlobalSymbol(Symbol s, LObject val)
        {
            if (IsItConst(s))
                throw new LispException("Assigning value to the constant: " + s);
            base.AssignGlobalSymbol(s, val);
        }


        public void DefineConst(Symbol s, LObject val)
        {
            if (IsItConst(s))
                throw new LispException(string.Format("Constant '{0}' already exists", s));

            globalSymbols[s] = val;
            constsSet.Add(s, null);
        }

        public bool IsItConst(Symbol s)
        {
            return constsSet.ContainsKey(s);
        }

        private void InitConstants()
        {
            DefineConst(new Symbol("t"), SpecialValues.T);
            DefineConst(new Symbol("nil"), SpecialValues.NIL);

            DefineConst(new Symbol("eq"), SpecialFunctions.LispNative.Eq_);
            DefineConst(new Symbol("set"), SpecialFunctions.LispNative.Set_);
            DefineConst(new Symbol("quote"), SpecialFunctions.LispNative.Quote_);
            DefineConst(new Symbol("lambda"), SpecialFunctions.LispNative.Lambda_);
            DefineConst(new Symbol("macro"), SpecialFunctions.LispNative.Macro_);
            DefineConst(new Symbol("function"), SpecialFunctions.LispNative.Function_);
            DefineConst(new Symbol("cond"), SpecialFunctions.LispNative.Cond_);
            DefineConst(new Symbol("eval"), SpecialFunctions.LispNative.Eval_);
            DefineConst(new Symbol("atom"), SpecialFunctions.LispNative.Atom_);
            DefineConst(new Symbol("null"), SpecialFunctions.LispNative.Null_);
            DefineConst(new Symbol("do"), SpecialFunctions.LispNative.Do_);

            DefineConst(new Symbol("print"), SpecialFunctions.Print_);

            DefineConst(new Symbol("+"), SpecialFunctions.Arithmetic.Add_);
            DefineConst(new Symbol("-"), SpecialFunctions.Arithmetic.Subsract_);
            DefineConst(new Symbol("*"), SpecialFunctions.Arithmetic.Multiply_);
            DefineConst(new Symbol("/"), SpecialFunctions.Arithmetic.Division_);

            DefineConst(new Symbol("and"), SpecialFunctions.Logical.And_);
            DefineConst(new Symbol("or"), SpecialFunctions.Logical.Or_);
            DefineConst(new Symbol("not"), SpecialFunctions.Logical.Not_);

            DefineConst(new Symbol("cons"), SpecialFunctions.ListProcessing.Cons_);
            DefineConst(new Symbol("list"), SpecialFunctions.ListProcessing.List_);
            DefineConst(new Symbol("car"), SpecialFunctions.ListProcessing.Car_);
            DefineConst(new Symbol("cdr"), SpecialFunctions.ListProcessing.Cdr_);

            DefineConst(new Symbol(".new"), SpecialFunctions.DotNetSupport.New_);
            DefineConst(new Symbol(".call"), SpecialFunctions.DotNetSupport.Call_);
            DefineConst(new Symbol("."), SpecialFunctions.DotNetSupport.Call_);
            DefineConst(new Symbol(".using"), SpecialFunctions.DotNetSupport.Using_);
			DefineConst(new Symbol(".reference"), SpecialFunctions.DotNetSupport.Reference_);
			DefineConst(new Symbol(".add-handler"), SpecialFunctions.DotNetSupport.AddHandler_);

            DefineConst(new Symbol("throw"), SpecialFunctions.Exceptions.Throw_);
            DefineConst(new Symbol("try"), SpecialFunctions.Exceptions.Try_);
        }
    }
}
