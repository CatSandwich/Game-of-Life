using DarkRift;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Simulation;

namespace Common.Data
{
    public class SimulationResults : IDarkRiftSerializable
    {
        public CompletionCause Cause;
        public int Iteration;
        public TimeSpan Elapsed;

        public void Deserialize(DeserializeEvent e)
        {
            Cause = (CompletionCause)e.Reader.ReadUInt16();
            Iteration = e.Reader.ReadInt32();
            Elapsed = e.Reader.ReadTimeSpan();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write((ushort)Cause);
            e.Writer.Write(Iteration);
            e.Writer.WriteTimeSpan(Elapsed);
        }
    }
}
