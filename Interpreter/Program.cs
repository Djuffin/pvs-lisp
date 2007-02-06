using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using PVSLisp.Common;

namespace PVSLisp
{
    public class Program
    {
        const string prompt = "$";
        const string exit = "exit";
        Interpreter intprt = new Interpreter();

        public void Run()
        {
            if (!RunInitScript()) return;
            for (; ; )
            {
                Console.Write(prompt);
                string input = Console.ReadLine();
                if (input == exit) break;
                LObject result = null;
                try
                {
                    result = Execute(input);
                    if (result != null)
                        Console.WriteLine(result.ToString());
                    else
                        Console.WriteLine("undefined result");
                }
                catch (LispException e)
                {
                    Console.WriteLine("Error: " + e.Message);
                }
                catch (Exception systemException)
                {
                    Console.WriteLine("System Error: " + systemException.Message);
                }
            } 
        }

        public LObject Execute(string input)
        {
            return intprt.Execute(input);
        }

        public bool RunInitScript()
        {
            try
            {
                using (TextReader reader = new StreamReader("system.ls", Encoding.Default))
                {
                    intprt.Execute(reader.ReadToEnd());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Initialization error. (see system.ls)");
                Console.WriteLine(e.GetType());
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return false;
            }
            return true;
        }


        static void Main(string[] args)
        {
            new Program().Run();
        }
    }
}
