using System;
using System.Collections.Generic;
using System.Text;

namespace PVSLisp.Common
{
    public class Symbol : Atom 
    {

        private string name;
		private int hashCode;


        public string Name
        {
            get
            {
                return name;
            }
        }

        public Symbol(string name)
        {
            this.name = string.Intern(name.ToLowerInvariant());
			hashCode = this.name.GetHashCode();
        }

        public override LObject Evaluate(LispEnvironment env)
        {
            LObject result = env.GetSymbolValue(this);

            if (result == null)
                return this;
                //throw new LispException(string.Format("Value for symbol '{0}' is undefined.", name));

            return result;
        }

        public override bool Equals(object obj)
        {
			Symbol s = obj as Symbol;
			if (s == null) return false;
			else return object.ReferenceEquals(name, s.name);
        }

        public override int GetHashCode()
        {
            return hashCode;
        }

        public override string ToString()
        {
            return name;
        }
    }
}
