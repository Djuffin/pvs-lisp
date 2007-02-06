using System;
using System.Collections.Generic;
using System.Text;

namespace PVSLisp.Common
{
    public class Symbol : Atom 
    {

        private string name;

        public string Name
        {
            get
            {
                return name;
            }
        }

        public Symbol(string name)
        {
            this.name = name;
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
            return string.Compare(Name, s.Name, true, System.Globalization.CultureInfo.InvariantCulture) == 0;
        }

        public override int GetHashCode()
        {
            return Name.ToLowerInvariant().GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
