using DarkRift;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CatDarkRift.Extensions;

namespace CatDarkRift.Serialization
{
    internal static class WriterMethods
    {
        public static IEnumerable<MethodInfo> Extensions => _extensions ?? (_extensions = typeof(DarkRiftWriter).GetExtensionMethods().Where(m => m.GetParameters().Length == 2 && m.ReturnType == typeof(void) && m.Name == "Write"));
        private static IEnumerable<MethodInfo> _extensions;
        public static IEnumerable<MethodInfo> Methods => _methods ?? (_methods = typeof(DarkRiftWriter).GetMethods(BindingFlags.Instance | BindingFlags.Public).Where(m => m.GetParameters().Length == 1 && m.ReturnType == typeof(void) && m.Name == "Write"));
        private static IEnumerable<MethodInfo> _methods;

        //private static Serializer CreateDel(MethodInfo method) => (Serializer) method.CreateDelegate(typeof(Serializer), null);
        
        public static Action<DarkRiftWriter, object> GetWriter(Type type)
        {
            // Built-in method matches type
            var method = Methods.FirstOrDefault(m => m.GetParameters()[0].ParameterType == type);
            if (method != null) return (writer, data) => method.Invoke(writer, new []{data});

            // Extension method matches type
            method = Extensions.FirstOrDefault(m => m.GetParameters()[1].ParameterType == type);
            if (method != null) return (writer, data) => method.Invoke(null, new[] { writer, data });

            // Serializable
            if (type.Implements(typeof(IDarkRiftSerializable)))
            {
                // Built-in method with generic parameter
                method = Methods.FirstOrDefault(m => m.GetParameters()[0].ParameterType.IsGenericParameter);
                if (method != null)
                {
                    var m = method.MakeGenericMethod(type);
                    return (writer, data) => m.Invoke(writer, new[] { data });
                }

                // Extension method with generic parameter
                method = Extensions.FirstOrDefault(m => m.GetParameters()[1].ParameterType.IsGenericParameter);
                if (method != null)
                {
                    var m = method.MakeGenericMethod(type);
                    return (writer, data) => m.Invoke(null, new[] {writer, data});
                };
            }

            // Array
            if (type.IsArray)
            {
                method = Methods.FirstOrDefault(m =>
                {
                    var param = m.GetParameters()[0].ParameterType;
                    // Require array parameter of same rank and type
                    return param.IsArray && param.GetArrayRank() == type.GetArrayRank() && param.GetElementType() == type.GetElementType();
                });
                if (method != null) return (writer, data) => method.Invoke(writer, new[] { data });

                method = Extensions.FirstOrDefault(m =>
                {
                    var param = m.GetParameters()[1].ParameterType;
                    // Require array parameter of same rank and type
                    return param.IsArray && param.GetArrayRank() == type.GetArrayRank() && param.GetElementType() == type.GetElementType();
                });
                if (method != null) return (writer, data) => method.Invoke(null, new[] { writer, data });
            }
            
            // Serializable Array
            if (type.IsArray && type.GetElementType().Implements(typeof(IDarkRiftSerializable)))
            {
                method = Methods.FirstOrDefault(m =>
                {
                    var param = m.GetParameters()[0].ParameterType;
                    // Require generic array parameter of same rank
                    return param.IsArray && param.GetArrayRank() == type.GetArrayRank() && param.GetElementType().IsGenericParameter;
                });
                if (method != null)
                {
                    var m = method.MakeGenericMethod(type.GetElementType());
                    return (writer, data) => m.Invoke(writer, new[] { data });
                }
                
                method = Extensions.FirstOrDefault(m =>
                {
                    var param = m.GetParameters()[1].ParameterType;
                    // Require generic array parameter of same rank
                    return param.IsArray && param.GetArrayRank() == type.GetArrayRank() && param.GetElementType().IsGenericParameter;
                });
                if (method != null)
                {
                    var m = method.MakeGenericMethod(type.GetElementType());
                    return (writer, data) => m.Invoke(null, new[] { writer,  data });
                }
            }

            throw new ArgumentException($"No suitable writer for type {type.Name}");
        }
    }
}
