
namespace Common.Simulation
{
    public class SimulationCompleteEventArgs
    {
        public State State;
        public CompletionCause Cause;
    }

    public enum CompletionCause : ushort
    {
        Manual,
        Time,
        Iteration,
        Repeat
    }
}
