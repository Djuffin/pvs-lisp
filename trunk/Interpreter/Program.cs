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

        public void Run(string [] files)
        {
            if (!RunInitScript()) return;

			if (files.Length == 0)
				ExecuteConsole();
			else
			{
				foreach (string fileName in files)
					ExecuteFile(fileName);
			}
        }

		private void ExecuteFile(string fileName)
		{
			try
			{
				using (TextReader reader = new StreamReader(fileName, Encoding.Default))
				{
					intprt.Execute(reader.ReadToEnd());
				}
			}
			catch (LispException le)
			{
				Console.WriteLine("Error '{0}' in file {1}", le.Message, fileName);
			}
			catch (Exception e)
			{
				Console.WriteLine("Error in file: " + fileName);
				Console.WriteLine(e.GetType());
				Console.WriteLine(e.Message);
				Console.WriteLine(e.StackTrace);
			}
		}

		private void ExecuteConsole()
		{
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
            new Program().Run(args);
        }
    }
}
