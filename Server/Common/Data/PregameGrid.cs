using System;
using CatDarkRift.Serialization;
using Common.Util;

namespace Common.Data
{
    public class PregameGrid : EasySerializable<PregameGrid>
    {
        public PregameCell[,] Grid;

        public PregameCell this[int x, int y] => Grid[x, y];

        public PregameGrid()
        {

        }
        
        public PregameGrid(int w, int h)
        {
            Grid = new PregameCell[w, h];
            Grid.Fill(() => PregameCell.NonPlaying);
        }

        #region Manipulators

        public void CopyUserInput(sbyte owner, PregameGrid input)
        {
            // Copy over all cells owned by owner
            input.Grid.For((cell, x, y) =>
            {
                if (cell.Owner == owner) Grid[x, y].Value = input[x, y].Value;
            });
        }
        
        public PregameCell[,] GetArea(int left, int bottom, int w, int h)
        {
            if (left + w - 1 >= Grid.Width()) throw new ArgumentException("Area reaches outside the width of the grid");
            if (bottom + h - 1 >= Grid.Height()) throw new ArgumentException("Area reaches outside the height of the grid");
            
            var arr = new PregameCell[w, h];
            for (var x = 0; x < w; x++)
            {
                for (var y = 0; y < h; y++)
                {
                    arr[x, y] = Grid[x + left, y + bottom];
                }
            }
            return arr;
        }

        public void CopyArea(int left, int bottom, PregameCell[,] from)
        {
            from.For((cell, x, y) =>
            {
                Grid[left + x, bottom + y] = from[x, y];
            });
        }
        #endregion

        public Simulation.Simulation ToSimulation()
        {
            return new Simulation.Simulation(Grid.Select(cell => cell.Value));
        }
    }
}
