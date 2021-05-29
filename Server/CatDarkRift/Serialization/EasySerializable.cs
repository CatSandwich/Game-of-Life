using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DarkRift;

// ReSharper disable StaticMemberInGenericType

namespace CatDarkRift.Serialization
{
    public abstract class EasySerializable<TChild> : IDarkRiftSerializable where TChild : EasySerializable<TChild>, new()
    {
        private static IEnumerable<SerializedField> Fields => _fields ?? (_fields = _init());
        private static IEnumerable<SerializedField> _fields;
        
        private static IEnumerable<SerializedField> _init()
        {
            var type = typeof(TChild);
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            return fields.Select(f => new SerializedField
            {
                Field = f, 
                Serializer = WriterMethods.GetWriter(f.FieldType),
                Deserializer = ReaderMethods.GetReader(f.FieldType)
            });
        }

        public void Deserialize(DeserializeEvent e)
        {
            foreach (var serializable in Fields)
            {
                serializable.Field.SetValue(this, serializable.Deserializer.Invoke(e.Reader));
            }
        }

        public void Serialize(SerializeEvent e)
        {
            foreach (var serializable in Fields)
            {
                serializable.Serializer.Invoke(e.Writer, serializable.Field.GetValue(this));
            }
        }
    }

    internal class SerializedField
    {
        internal delegate void SerializerDel(DarkRiftWriter writer, object data);
        public FieldInfo Field;
        public SerializerDel Serializer;
        public Func<DarkRiftReader, object> Deserializer;
    }
}
