using System;
using System.Collections.Generic;
using System.Text;

namespace PVSLisp.Common
{
    public abstract class Function : LObject
    {
        public abstract LispEnvironment BindParameters(LispEnvironment parent, LCell values);
    }
}
