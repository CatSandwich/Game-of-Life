using Common.Data;
using Common.Tags;
using Common.Util;
using DarkRift;
using Extensions;
using InfoObjects;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

namespace CellPlacement
{
    public class PlacementController : MonoBehaviour
    {
        [Header("Colors")]
        public Tile EnemyArea;
        public Tile NonPlaying;
        public Tile Empty;
        public Tile Alive;
        public Tile Dead;
        
        [Header("References")]
        public Tilemap Tilemap;
        public Camera Camera;
        
        private RequestCellPlacement _data;
        // Start is called before the first frame update
        void Start()
        {            
            _data = FindObjectOfType<InfoRequestCellPlacement>().Consume();
            if (_data is null)
            {
                Debug.LogError("Could not find cell placement info object or its data.");
                return;
            }
            
            Tilemap.LoadPregame(_data.Player, _data.Grid);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0)) _tryToggleCell();
        }

        public void Submit()
        {
            Network.Network.Inst.SendMessage(ToServer.CellPlacement, _data.Grid);
        }

        private void _tryToggleCell()
        {
            var pos = Tilemap.WorldToCell(Camera.ScreenToWorldPoint(Input.mousePosition));
            if (pos.x < 0 || pos.x >= _data.Grid.Grid.Width()) return;
            if (pos.y < 0 || pos.y >= _data.Grid.Grid.Height()) return;
            if (_data.Grid[pos.x, pos.y].Owner != _data.Player) return;

            _toggleCell(pos);
        }
        
        private void _toggleCell(Vector3Int pos)
        {
            if (_data.Grid[pos.x, pos.y].Value == 0)
            {
                _data.Grid[pos.x, pos.y].Value = _data.Player;
                Tilemap.SetTile(pos, Alive);
            }
            else
            {
                _data.Grid[pos.x, pos.y].Value = 0;
                Tilemap.SetTile(pos, Dead);
            }
        }
    }
}
