using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DarkRift;

namespace CatDarkRift
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class MessageHandlerAttribute : Attribute
    {
        public ushort Tag;
        public Type Type;
        public MessageHandlerAttribute(ushort tag, Type type)
        {
            Tag = tag;
            Type = type;
        }
    }
}
