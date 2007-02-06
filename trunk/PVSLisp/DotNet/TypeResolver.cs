using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using PVSLisp.Common;

namespace PVSLisp.DotNet
{
    public class TypeResolver
    {
        public static readonly TypeResolver Instance = new TypeResolver();
        private Dictionary<string, Type> typeMap = new Dictionary<string, Type>();
        private List<string> usings = new List<string>();

        private TypeResolver()
        {
            ClearCache();
            ClearUsings();
        }

        public void ClearCache()
        {
            typeMap.Clear();
        }

        public void ClearUsings()
        {
            usings.Clear();
            usings.Add(string.Empty);
            AddUsing("System");
        }

        public void AddUsing(string name)
        {
            usings.Add(name + ".");
        }

        public Type GetTypeByName(string name)
        {
            Type result = null;
            foreach (string usingName in usings)
            {
                result = GetTypeByFullName(usingName + name);
                if (result != null) break;
            }
            return result;
        }

        public Type GetTypeByFullName(string name)
        {
            name = name.ToLower();
            Type result = null;
            typeMap.TryGetValue(name, out result);

            if (result == null)
            {
                result = SearchType(name);
                typeMap[name] = result;
            }

            return result;
        }

        private Type SearchType(string name)
        {
            Type result = null;
            foreach (Assembly assembly in GetAssemblies())
            {
                result = assembly.GetType(name, false, true);
                if (result != null) break;
            }

            return result;
        }

        private static Assembly[] GetAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }

        public Assembly LoadAssembly(string name)
        {
            try
            {
                if (System.IO.Path.IsPathRooted(name))
                    return Assembly.LoadFrom(name);
                else
                {
                    return Assembly.LoadWithPartialName(name);
                    //return Assembly.Load(new AssemblyName(name));
                }
            }
            catch
            {
                return null;
            }
        }

    }
}
