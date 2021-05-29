using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Data;

namespace Plugin.Maps
{
    abstract class Map
    {
        public abstract string Name { get; }
        public PregameGrid Grid => _generateGrid();

        protected abstract PregameGrid _generateGrid();
    }
}
