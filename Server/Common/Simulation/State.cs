using System;

namespace Common.Simulation
{
    public class State
    {
        public sbyte[,] Cells;
        public int Iteration;
        public TimeSpan Elapsed;
        public TimeSpan UpdateTime;
        public TimeSpan CopyTime;
    }
}
