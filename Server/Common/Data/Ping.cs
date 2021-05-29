using CatDarkRift;
using CatDarkRift.Serialization;
using DarkRift;

namespace Common.Data
{
    public class Ping : EasySerializable<Ping>
    {
        public string Message;

        public Ping() : base()
        {
            
        }

        public Ping(string message)
        {
            Message = message;
        }
    }
}
