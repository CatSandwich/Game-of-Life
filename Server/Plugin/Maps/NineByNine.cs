using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Data;
using Common.Util;

namespace Plugin.Maps
{
    class NineByNine : Map
    {
        public override string Name => "Nine by Nine";
        protected override PregameGrid _generateGrid()
        {
            var grid = new PregameGrid(9, 9);
            
            // Left 3 columns
            grid.GetArea(0, 0, 3, 9).For((cell, x, y) =>
            {
                cell.Owner = 1;
                cell.Value = 0;
            });
            // Middle 3 columns
            grid.GetArea(3, 0, 3, 9).For((cell, x, y) =>
            {
                cell.Owner = 0;
                cell.Value = 0;
            });
            // Right 3 columns
            grid.GetArea(6, 0, 3, 9).For((cell, x, y) =>
            {
                cell.Owner = 2;
                cell.Value = 0;
            });
            
            return grid;
        }
    }
}
