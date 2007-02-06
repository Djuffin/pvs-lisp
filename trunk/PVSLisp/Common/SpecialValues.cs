using System;
using System.Collections.Generic;
using System.Text;

namespace PVSLisp.Common
{
    public static class SpecialValues
    {
        public static Scalar T = ScalarFactory.Make(true);
        public static Scalar NIL = ScalarFactory.Make((object)null);
    }
}
