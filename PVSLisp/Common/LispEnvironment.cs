using System;
using System.Collections.Generic;
using System.Text;

namespace PVSLisp.Common
{
    public class LispEnvironment
    {
        private LispEnvironment parent;
        private Runtime runtime;
        protected Dictionary<Symbol, LObject> globalSymbols = new Dictionary<Symbol, LObject>();
        protected Dictionary<Symbol, LObject> localSymbols = new Dictionary<Symbol, LObject>();
        bool transparent = false;

        public LispEnvironment(LispEnvironment parent)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");
            this.parent = parent;
            this.runtime = parent.Runtime;
        }

        public LispEnvironment(Runtime runtime)
        {
            if (runtime == null)
                throw new ArgumentNullException("runtime");
            this.runtime = runtime;
        }

        public LispEnvironment Parent
        {
            get
            {
                return parent;
            }
        }

        public Runtime Runtime
        {
            get
            {
                return runtime;
            }
        }

        public bool Transparent
        {
            get
            {
                return transparent; 
            }
            set
            {
                transparent = value;
            }
        }


        protected LispEnvironment GetLocalSymbolEnvironment(Symbol s)
        {
            if (localSymbols.ContainsKey(s)) return this;
            if (transparent && parent != null)
                return parent.GetLocalSymbolEnvironment(s);

            return null;
        }

        protected LispEnvironment GetGlobalSymbolEnvironment(Symbol s)
        {
            if (globalSymbols.ContainsKey(s)) return this;
            if (parent != null)
                return parent.GetGlobalSymbolEnvironment(s);

            return null;
        }

        protected LispEnvironment GetSymbolEnvironment(Symbol s, ref bool isLocal)
        {
            LispEnvironment result = GetLocalSymbolEnvironment(s);
            if (result != null)
            {
                isLocal = true;
                return result;
            }

            result = GetGlobalSymbolEnvironment(s);
            if (result != null) 
                isLocal = false;
            return result;
        }

        public virtual LObject GetSymbolValue(Symbol s)
        {
            bool isLocal = true;
            LispEnvironment owner = GetSymbolEnvironment(s, ref isLocal);
            if (owner != null)
                return owner.GetSymbolValueInternal(s, isLocal);

            return null;
        }

        protected virtual LObject GetSymbolValueInternal(Symbol s, bool isLocal)
        {
            if (isLocal)
                return localSymbols[s];
            else
                return globalSymbols[s];
        }

        public virtual void Assign(Symbol s, LObject val)
        {
            bool isLocal = true;
            LispEnvironment owner = GetSymbolEnvironment(s, ref isLocal);
            if (owner != null)
                owner.AssignInternal(s, val, isLocal);
            else
                AssignInternal(s, val, false);
        }

        protected virtual void AssignInternal(Symbol s, LObject val, bool isLocal)
        {
            if (isLocal)
                localSymbols[s] = val;
            else
            {
                if (parent != null)
                    parent.AssignInternal(s, val, false);
                else
                    globalSymbols[s] = val;
            }
        }

        public virtual void AssignLocalSymbol(Symbol s, LObject val)
        {
            localSymbols[s] = val;
        }

        public virtual void AssignGlobalSymbol(Symbol s, LObject val)
        {
            globalSymbols[s] = val;
        }


    }
}
