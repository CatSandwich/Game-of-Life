using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    namespace Tags
    {
        public enum ToClient
        {
            Ping = 0,
            Pong = 1,
            
            RequestCellPlacement = 100,
            StartSimulation = 101,
            SimulationResults = 102
        }
        public enum ToServer
        {
            Ping = 0,
            Pong = 1,
            
            CellPlacement = 100,
        }
    }

}
