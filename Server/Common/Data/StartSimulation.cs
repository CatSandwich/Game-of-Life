using Common.Util;
using DarkRift;

namespace Common.Data
{
    public class StartSimulation : IDarkRiftSerializable
    {
        public sbyte[,] Grid;
        public sbyte Player;
        public void Deserialize(DeserializeEvent e)
        {
            Grid = e.Reader.Read2DSByte();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(Grid);
        }
    }
}
