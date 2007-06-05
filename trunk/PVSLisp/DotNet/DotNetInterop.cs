using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using PVSLisp.Common;

namespace PVSLisp.DotNet
{
    public class DotNetInterop
    {

        public static object CallStatic(string typeName, string name, params object[] arguments)
        {
            return CallInternal(null, ResolveType(typeName), name, arguments);
        }

        public static object Call(object This, string name, params object[] arguments)
        {
            if (This == null)
                throw new LispException("First argument (this) is equal null in nonstatic call.");

            return CallInternal(This, This.GetType(), name, arguments);
        }

        public static object New(string typeName, params object[] arguments)
        {
            try
            {
                if (arguments == null) arguments = new object[0];
                return Activator.CreateInstance(ResolveType(typeName), arguments);
            }
            catch (MissingMethodException e)
            {
                string message = string.Format("There is no constructor for type '{0}' with these argumetns", typeName);
                throw new LispException(message, e);
            }
            catch (MemberAccessException e1)
            {
                string message = string.Format("Cannot create instance of type '{0}'", typeName);
                throw new LispException(message, e1);
            }
            catch (AmbiguousMatchException e2)
            {
                string message = string.Format("Ambiguous match found. Cannot create instance of type '{0}'", typeName);
                throw new LispException(message, e2);
            }
        }

		public static void AddEventHandler(object This, string name, Delegate handler)
		{
			Type targetType = This.GetType();
			BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.FlattenHierarchy;

			EventInfo targetEvent = targetType.GetEvent(name, bindingFlags);
			if (targetEvent == null)
				throw new LispException("No event with such name exests in the type: " + targetType.FullName);

			try
			{
				targetEvent.AddEventHandler(This, handler);
			}
			catch (Exception e)
			{
				throw new LispException("Cannot subscribe to the event: " + name, e);
			}
		}

        private static Type ResolveType(string typeName)
        {
            Type result = TypeResolver.Instance.GetTypeByName(typeName);
            if (result == null)
                throw new LispException("Unresolved .NET type : " + typeName);
            return result;
        }

        private static object CallInternal(object This, Type targetType, string name, object[] arguments)
        {
            string entityName = targetType.FullName + "." + name;
            if (arguments == null) arguments = new object[0];
            BindingFlags bindingFlags = BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.FlattenHierarchy;

            bindingFlags = bindingFlags | (This == null ? BindingFlags.Static : BindingFlags.Instance);

            Type[] argumentsTypes = Array.ConvertAll<object, Type>(arguments, delegate(object item)
            {
                if (item == null) return typeof(object);
                return item.GetType();
            });

			//try do call
            MethodInfo method = targetType.GetMethod(name, bindingFlags, null, argumentsTypes, null);
            if (method != null)
                return method.Invoke(This, arguments);

			//try get/set property
            PropertyInfo property = targetType.GetProperty(name, bindingFlags);
            if (property != null)
            {
                if (arguments.Length > 1)
                    throw new LispException("Too many parameters for property set");

                if (arguments.Length == 0)
                {
                    if (!property.CanRead)
                        throw new LispException("Cannot read property: " + entityName);
                    return property.GetValue(This, null);
                }
                else
                {
                    if (!property.CanWrite)
                        throw new LispException("Cannot write property: " + entityName);
                    property.SetValue(This, arguments[0], null);
                    return arguments[0];
                }
            }

			//try read/write field
            FieldInfo field = targetType.GetField(name, bindingFlags);
            if (field != null)
            {
                if (arguments.Length > 1)
                    throw new LispException("Too many parameters for field set");

                if (arguments.Length == 0)
                    return field.GetValue(This);
                else
                {
                    field.SetValue(This, arguments[0]);
                    return arguments[0];
                }
            }

            throw new LispException("Unresolved method/property/field: " + entityName);
        }
    }
}
