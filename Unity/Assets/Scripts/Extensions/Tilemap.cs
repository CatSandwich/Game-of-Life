using Common.Data;
using Common.Util;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Extensions
{
    public static class TilemapExtensions
    {
        private static AssetController Tiles => AssetController.Inst;
        public static void LoadPregame(this Tilemap tilemap, sbyte player, PregameGrid grid)
        {
            grid.Grid.Select(c => c.Owner).For((owner, x, y) =>
            {
                var pos = new Vector3Int(x, y, 0);
                
                Tile tile;
                
                if (owner == -1) tile = Tiles.NonPlaying;
                else if (owner == 0) tile = Tiles.Empty;
                else if (owner == player) tile = Tiles.Dead;
                else tile = Tiles.EnemyArea;
                
                tilemap.SetTile(pos, tile);
            });
        }
    }
}
