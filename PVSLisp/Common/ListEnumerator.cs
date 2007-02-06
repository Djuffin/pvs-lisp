using System;
using System.Collections.Generic;
using System.Text;

namespace PVSLisp.Common
{
    internal class ListEnumerator : IEnumerator<LObject>
    {
        private LCell originalList;
        private LCell currentList;

        public ListEnumerator(LCell list)
        {
            originalList = new LCell(SpecialValues.NIL, list); //Fake first call of MoveNext
            Reset();
            
        }

        #region IEnumerator<LObject> Members

        public LObject Current
        {
            get
            {
                return currentList.Head;
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            //nothing to do
        }

        #endregion

        #region IEnumerator Members

        object System.Collections.IEnumerator.Current
        {
            get 
            {
                return ((IEnumerator<LObject>)this).Current;
            }
        }

        public bool MoveNext()
        {
            if (currentList.Tail == null) return false;
            currentList = currentList.Tail;
            return true;
        }

        public void Reset()
        {
            currentList = originalList;
        }

        #endregion
    }
}
