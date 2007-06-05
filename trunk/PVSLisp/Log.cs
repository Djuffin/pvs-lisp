using System;
using System.Collections.Generic;
using System.Text;
using PVSLisp.Common;
using System.Diagnostics;

namespace PVSLisp
{
    public class Log
    {
        public static int EnvMaxDepth = 0;
        public static int EnvCreated = 0;

        public static void EnvironmentCreate(LispEnvironment env)
        {
            EnvCreated++;
            int count = 0;
            while (env != null)
            {
                env = env.Parent;
                count++;
            }

            if (count > EnvMaxDepth)
                EnvMaxDepth = count;
        }


        public static int MaxCallDepth = 0;
        public static StackTrace DeepestStackTrace;
        public static bool WriteTrace = false;

        public static void LCellEvaluate(LCell cell, LispEnvironment env)
        {
            if (WriteTrace)
            {
                StackTrace st = new StackTrace();
                if (MaxCallDepth < st.FrameCount)
                {
                    MaxCallDepth = st.FrameCount;
                    DeepestStackTrace = st;
                }
            }
        }

        public static int LateEvaluatorCount = 0;
        public static void LateEvaluatorCreate(LateEvaluator evaluator)
        {
            LateEvaluatorCount++;
        }


        public static void PrintStatistics()
        {
            Console.WriteLine("\nMax Env Depth: {0}", EnvMaxDepth);
            Console.WriteLine("Env creation count: {0}", EnvCreated);
            Console.WriteLine("LateEvaluator creation count: {0}", LateEvaluatorCount);
            Console.WriteLine("Max depth of stacktrace: {0}", MaxCallDepth);

            if (DeepestStackTrace != null)
            {
                Console.WriteLine("Most deepest stacktrace.");
                PrintStackTrace(DeepestStackTrace);
            }
        }

        private static void PrintStackTrace(StackTrace st)
        {
            for (int i = st.FrameCount - 1; i >= 0 ; i--)
            {
                StackFrame frame = st.GetFrame(i);
                Console.WriteLine("FRAME {0}", st.FrameCount - i);
                Console.WriteLine("Class: {0}, Method: {1}", frame.GetMethod().DeclaringType, frame.GetMethod());
            }
        }
    }
}
