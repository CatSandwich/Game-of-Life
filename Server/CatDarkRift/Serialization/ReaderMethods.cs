using DarkRift;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using CatDarkRift.Extensions;

namespace CatDarkRift.Serialization
{
    internal static class ReaderMethods
    {
        public static IEnumerable<MethodInfo> Extensions => _extensions ?? (_extensions = typeof(DarkRiftReader).GetExtensionMethods().Where(m => m.Name.Contains("Read") && m.GetParameters().Length == 1));
        private static IEnumerable<MethodInfo> _extensions;
        public static IEnumerable<MethodInfo> Methods => _methods ?? (_methods = typeof(DarkRiftReader).GetMethods(BindingFlags.Instance | BindingFlags.Public));
        private static IEnumerable<MethodInfo> _methods;
        
        private static Func<DarkRiftReader, object> CreateDel(MethodInfo m) => (Func<DarkRiftReader, object>)Delegate.CreateDelegate(typeof(Func<DarkRiftReader, object>), m);

        public static Func<DarkRiftReader, object> GetReader(Type type)
        {
            // Built-in method matches type
            var method = Methods.FirstOrDefault(m => m.ReturnType == type);
            if (method != null) return CreateDel(method);
            //if (method != null) return reader => method.Invoke(reader, null);

            // Extension method matches type
            method = Extensions.FirstOrDefault(m => m.ReturnType == type);
            if (method != null) return reader => method.Invoke(null, new object[] { reader });

            // Serializable
            if (type.Implements(typeof(IDarkRiftSerializable)))
            {
                method = Methods.FirstOrDefault(m => m.ReturnType.IsGenericParameter);
                if (method != null)
                {
                    var m = method.MakeGenericMethod(type);
                    return reader => m.Invoke(reader, null);
                }

                method = Extensions.FirstOrDefault(m => m.ReturnType.IsGenericParameter);
                if (method != null)
                {
                    var m = method.MakeGenericMethod(type);
                    return reader => m.Invoke(null, new object[]{reader});
                }
            }

            // Array
            if (type.IsArray)
            {
                method = Methods.FirstOrDefault(m =>
                {
                    var returnType = m.ReturnType;
                    // Require array return type of same rank and type
                    return returnType.IsArray && returnType.GetArrayRank() == type.GetArrayRank() && returnType.GetElementType() == type.GetElementType();
                });
                if (method != null) return reader => method.Invoke(reader, null);

                method = Extensions.FirstOrDefault(m =>
                {
                    var returnType = m.ReturnType;
                    // Require array return type of same rank and type
                    return returnType.IsArray && returnType.GetArrayRank() == type.GetArrayRank() && returnType.GetElementType() == type.GetElementType();
                });
                if (method != null) return reader => method.Invoke(null, new object[] { reader });
            }

            // Serializable Array
            if (type.IsArray && type.GetElementType().Implements(typeof(IDarkRiftSerializable)))
            {
                method = Methods.FirstOrDefault(m =>
                {
                    var returnType = m.ReturnType;
                    // Require generic array return type of same rank
                    return returnType.IsArray && returnType.GetArrayRank() == type.GetArrayRank() && returnType.GetElementType().IsGenericParameter;
                });
                if (method != null)
                {
                    var m = method.MakeGenericMethod(type.GetElementType());
                    return reader => m.Invoke(reader, null);
                }

                method = Extensions.FirstOrDefault(m =>
                {
                    var returnType = m.ReturnType;
                    // Require generic array return type of same rank
                    return returnType.IsArray && returnType.GetArrayRank() == type.GetArrayRank() && returnType.GetElementType().IsGenericParameter;
                });
                if (method != null)
                {
                    var m = method.MakeGenericMethod(type.GetElementType());
                    return reader => m.Invoke(null, new object[] { reader });
                }
            }

            throw new ArgumentException($"No suitable reader for type {type}");
        }
    }
}
