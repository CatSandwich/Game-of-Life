using DarkRift;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CatDarkRift.Extensions
{
    public static class TypeExtension
    {
        public static IEnumerable<MethodInfo> GetExtensionMethods(this Type type)
        {
            var query = from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from t in assembly.GetTypes()
                where !t.IsGenericType && !t.IsNested
                from m in t.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                where m.IsDefined(typeof(System.Runtime.CompilerServices.ExtensionAttribute), false)
                where m.GetParameters()[0].ParameterType == type
                select m;

            return query;
        }

        public static bool Implements(this Type type, Type inter) => type.GetInterfaces().Contains(inter);
    }
}
