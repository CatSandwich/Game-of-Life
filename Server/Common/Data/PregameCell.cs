using CatDarkRift.Serialization;

namespace Common.Data
{
    public class PregameCell : EasySerializable<PregameCell>
    {
        public static PregameCell NonPlaying => new PregameCell {Owner = -1, Value = -1};
        public static PregameCell Empty => new PregameCell {Owner = 0, Value = 0};
        
        public sbyte Owner;
        public sbyte Value;
    }
}
