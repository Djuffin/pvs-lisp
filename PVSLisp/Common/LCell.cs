using System;
using System.Collections.Generic;
using System.Text;

namespace PVSLisp.Common
{
    public class LCell : LObject, IEnumerable<LObject>
    {
        private LObject head;
        private LCell tail;

        public LObject Head
        {
            get
            {
                return head;
            }
        }

        public LCell Tail
        {
            get
            {
                return tail;
            }
        }

        public LCell(LObject head)
        {
            this.head = head;
            this.tail = null;
            if (head == null)
                throw new Exception("LCell.head = null");
        }

        public LCell(LObject head, LCell tail)
        {
            this.head = head;
            this.tail = tail;
            if (head == null)
                throw new Exception("LCell.head = null");
        }

        public static LCell Make(IEnumerable<LObject> prototype)
        {
            if (prototype == null) return null;
            LCell root = null;
            LCell currentList = null;

            foreach (LObject item in prototype)
            {
                if (root == null)
                    currentList = root = new LCell(item);
                else
                {
                    currentList.tail = new LCell(item);
                    currentList = currentList.tail;
                }
            }

            return root;
        }

        public int Count
        {
            get
            {
                return tail != null ? tail.Count + 1 : 1;
            }
        }

        public override LObject Evaluate(LispEnvironment env)
        {
            LObject result = EvaluateInternal(env, false);

            if (result is LateEvaluator)
                result = result.Evaluate(null);

            return result;
        }

        public override LObject TailEvaluate(LispEnvironment env)
        {
            return EvaluateInternal(env, true);
        }

        private LObject EvaluateInternal(LispEnvironment env, bool tailCall)
        {
            //Log.LCellEvaluate(this, env);

            if (head == null)
                throw new LispException("List head is not a valid function.");

            LObject evaluatedHead = head.Evaluate(env);
            Function func = evaluatedHead as Function;
            LCell arguments = tail;

            if (func == null && TryGuessDotNetCall(env, evaluatedHead))
            {
                func = SpecialFunctions.DotNetSupport.Call_;
                arguments = this;
            }

            if (func == null)
                throw new LispException("List head is not a valid function.");

            return func.Call(env, arguments, tailCall);
        }

        private bool TryGuessDotNetCall(LispEnvironment env, LObject evaluatedHead)
        {
            if (Count < 2)
                return false;

            if (!(evaluatedHead is Symbol))
                return false;

            return true;
        }


        public override string ToString()
        {
            return ToLispString();
        }

        public override string ToLispString()
        {
            StringBuilder result = new StringBuilder("( ");
            foreach (LObject item in this)
            {
                result.Append(item.ToLispString());
                result.Append(" ");
            }
            result.Append(")");
            return result.ToString();
        }

        #region IEnumerable<LObject> Members

        public IEnumerator<LObject> GetEnumerator()
        {
            return new ListEnumerator(this);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        public static bool EqualLists(LObject oList1, LObject oList2)
        {
            LCell list1 = oList1 as LCell;
            LCell list2 = oList2 as LCell;
            if (list1 == null || list2 == null) return false;

            if (list1.Count != list2.Count) return false;
            IEnumerator<LObject> e1 = list1.GetEnumerator();
            IEnumerator<LObject> e2 = list2.GetEnumerator();

            bool status = e1.MoveNext();
            if (status != e2.MoveNext()) return false;

            while (status)
            {
                LObject o1 = e1.Current;
                LObject o2 = e2.Current;

                bool eq;
                if (o1 is LCell)
                    eq = EqualLists(o1, o2);
                else
                    eq = object.Equals(o1, o2);

                if (!eq) return false;

                status = e1.MoveNext();
                if (status != e2.MoveNext()) return false;
            }

            return true;
        }

    }
}
