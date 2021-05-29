using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DarkRift;

namespace Common.Data
{
    public static class SerializeExtension
    {
        public static void WriteTimeSpan(this DarkRiftWriter writer, TimeSpan span)
        {
            writer.Write(span.Days);
            writer.Write(span.Hours);
            writer.Write(span.Minutes);
            writer.Write(span.Seconds);
            writer.Write(span.Milliseconds);
        }
        public static TimeSpan ReadTimeSpan(this DarkRiftReader reader)
        {
            var d = reader.ReadInt32();
            var h = reader.ReadInt32();
            var min = reader.ReadInt32();
            var s = reader.ReadInt32();
            var mil = reader.ReadInt32();
            return new TimeSpan(d, h, min, s, mil);
        }
    }
}
