using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable ForCanBeConvertedToForeach
// ReSharper disable LoopCanBeConvertedToQuery

namespace Common.Simulation
{
    public class Simulation
    {
        public sbyte[,] Grid => _beforeCalculation.Clone() as sbyte[,];
        private readonly sbyte[,] _beforeCalculation;
        private readonly sbyte[,] _calculated;
        public readonly int Width;
        public readonly int Height;

        public Stopwatch Stopwatch;
        public int Iteration;
        public CompletionCause Cause;

        #region Events
        public event EventHandler<State> StateUpdated = (sender, args) => { };
        public event EventHandler<SimulationCompleteEventArgs> SimulationComplete = (sender, args) => { };
        #endregion

        private bool _running = true;

        #region Event Subscribers
        public static EventHandler<State> EndOnRepeat()
        {
            var history = new LinkedList<byte[,]>();
            return (sender, ev) =>
            {
                foreach (var grid in history)
                {
                    for (var x = 0; x < grid.GetLength(0); x++)
                    {
                        for (var y = 0; y < grid.GetLength(1); y++)
                        {
                            if (grid[x, y] != ev.Cells[x, y]) goto continueLabel;
                        }
                    }

                    ((Simulation)sender)._end(CompletionCause.Repeat);
                    return;

                continueLabel:;
                }

                history.AddLast((byte[,])ev.Cells.Clone());
            };
        }

        public static EventHandler<State> Sleep(int ms) => (sender, ev) => Thread.Sleep(ms);

        public static EventHandler<State> EndOnIteration(int iteration) => (sender, ev) =>
        {
            if (ev.Iteration == iteration) (sender as Simulation)._end(CompletionCause.Iteration);
        };

        public static EventHandler<State> EndOnTime(int milliseconds) => (sender, ev) =>
        {
            if (ev.Elapsed.TotalMilliseconds >= milliseconds) (sender as Simulation)._end(CompletionCause.Time);
        };
        #endregion

        public Simulation(sbyte[,] start)
        {
            Width = start.GetLength(0);
            Height = start.GetLength(1);

            _beforeCalculation = (sbyte[,])start.Clone();
            _calculated = (sbyte[,])start.Clone();
        }

        public async Task<State> ExecuteAsync()
        {
            return await Task.Run(() =>
            {
                Stopwatch = Stopwatch.StartNew();
                Iteration = 0;
                while (_running)
                {
                    ++Iteration;

                    _update();
                    _copy();

                    StateUpdated.Invoke(this, new State
                    {
                        Cells = _beforeCalculation,
                        Iteration = Iteration,
                        Elapsed = Stopwatch.Elapsed
                    });
                }

                var state = new State
                {
                    Cells = _beforeCalculation,
                    Iteration = Iteration,
                    Elapsed = Stopwatch.Elapsed
                };
                SimulationComplete.Invoke(this, new SimulationCompleteEventArgs { State = state, Cause = Cause });
                return state;
            });
        }

        public void End() => _end(CompletionCause.Manual);
        private void _end(CompletionCause cause)
        {
            _running = false;
            Cause = cause;
        }

        private void _copy()
        {
            for (var x = 0; x < _calculated.GetLength(0); x++)
            {
                for (var y = 0; y < _calculated.GetLength(1); y++)
                {
                    _beforeCalculation[x, y] = _calculated[x, y];
                }
            }
        }

        private void _update()
        {
            var changed = false;
            for (var x = 0; x < _calculated.GetLength(0); x++)
            {
                for (var y = 0; y < _calculated.GetLength(1); y++)
                {
                    // Ignore negative values (non-playing cells)
                    if (_beforeCalculation[x, y] < 0) continue;
                    
                    var doUp = y != Height - 1;
                    var doDown = y != 0;
                    var doRight = x != Width - 1;
                    var doLeft = x != 0;

                    var w = 1;
                    if (doLeft) ++w;
                    if (doRight) ++w;
                    var h = 1;
                    if (doUp) ++h;
                    if (doDown) ++h;

                    var neighbors = new sbyte[w * h - 1];
                    var index = 0;

                    if (doUp) neighbors[index++] = _beforeCalculation[x, y + 1];
                    if (doUp && doRight) neighbors[index++] = _beforeCalculation[x + 1, y + 1];
                    if (doRight) neighbors[index++] = _beforeCalculation[x + 1, y];
                    if (doDown && doRight) neighbors[index++] = _beforeCalculation[x + 1, y - 1];
                    if (doDown) neighbors[index++] = _beforeCalculation[x, y - 1];
                    if (doDown && doLeft) neighbors[index++] = _beforeCalculation[x - 1, y - 1];
                    if (doLeft) neighbors[index++] = _beforeCalculation[x - 1, y];
                    if (doUp && doLeft) neighbors[index] = _beforeCalculation[x - 1, y + 1];

                    var newVal = _updateCell(x, y, neighbors);
                    if (newVal == _beforeCalculation[x, y]) continue;

                    changed = true;
                    _calculated[x, y] = newVal;
                }
            }
            if (!changed) _end(CompletionCause.Repeat);
        }

        private sbyte _updateCell(int x, int y, sbyte[] neighbors)
        {
            var alive = 0;

            for (var i = 0; i < neighbors.Length; i++)
            {
                if (neighbors[i] != 0) alive++;
            }

            // If not 2 or 3      -> Die
            if (alive < 2 || alive > 3) return 0;
            // If 2               -> Don't change states
            if (alive == 2) return _beforeCalculation[x, y];
            // If 3 and alive     -> Stay alive
            if (_beforeCalculation[x, y] != 0) return _beforeCalculation[x, y];

            // If 3 and not alive -> Become alive -> Owned by majority around

            var owners = new sbyte[3];
            var index = 0;

            foreach (var neighbor in neighbors)
            {
                if (neighbor != 0) owners[index++] = neighbor;
            }

            if (owners[0] == owners[1]) return owners[0];
            if (owners[0] == owners[2]) return owners[0];
            if (owners[1] == owners[2]) return owners[1];

            // TODO: evenly distribute ties
            return owners[0];
        }
    }
}
