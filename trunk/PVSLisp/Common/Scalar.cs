using System;
using System.Collections.Generic;
using System.Text;

namespace PVSLisp.Common
{
    public abstract class Scalar : Atom
    {
        public abstract object Value
        {
            get;
        }

        public virtual Type Type
        {
            get
            {
                if (Value == null) return null;
                return Value.GetType();
            }
        }

        public override string ToString()
        {
            if (Value == null) return "NIL";
            string result = Value.ToString();

            return result;
        }

        public override string ToLispString()
        {
            if (Value == null) return "()";
            string result = Value.ToString();

            if (this.Type == typeof(string))
                result = string.Format("\"{0}\"", result);

            return result;
        }

        protected Scalar()
        {
        }

        public override LObject Evaluate(LispEnvironment env)
        {
            return this;
        }

        public override bool Equals(object obj)
        {
            if (obj == null && Value == null) return true;

            Scalar scalar = obj as Scalar;
            if (scalar == null) return false;
            return object.Equals(Value, scalar.Value);
        }

        public override int GetHashCode()
        {
            if (Value == null) return 0;
            return Value.GetHashCode();
        }
    }

    public class TypedScalar<T> : Scalar
    {
        T value;

        public T TypedValue
        {
            get
            {
                return value;
            }
        }

        public override object Value
        {
            get { return value;  }
        }

        public override Type Type
        {
            get
            {
                return typeof(T);
            }
        }

        public TypedScalar(T value)
        {
            this.value = value;
        }
    }

    public static class ScalarFactory
    {
        public static Scalar Make<T>(T t)
        {
            return new TypedScalar<T>(t);
        }
    }
}
