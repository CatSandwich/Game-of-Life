using Common.Data;
using Common.Simulation;
using Common.Util;
using InfoObjects;
using UnityEngine;
using UnityEngine.Tilemaps;
using Task = System.Threading.Tasks.Task;

public class GameController : MonoBehaviour
{
    public AssetController Tiles => AssetController.Inst;
    public Tilemap Tilemap;
    public StartSimulation Data;
    public Simulation Simulation;

    public void Start()
    {
        Data = FindObjectOfType<InfoStartSimulation>().Consume();
        Debug.Log(Data.Player);

        Task.Run(async () =>
        {
            Simulation = new Simulation(Data.Grid);
            Simulation.StateUpdated += (sender, ev) =>
            {
                Debug.Log("StateUpdated");
                Data.Grid = ev.Cells;
            };
            await Simulation.ExecuteAsync();
        });
    }

    private void Update()
    {
        Data.Grid.For((val, x, y) =>
        {
            var pos = new Vector3Int(x, y, 0);
                
            Tile tile;
                
            if (val == -1) tile = Tiles.NonPlaying;
            else if (val == 0) tile = Tiles.Dead;
            else if (val == Data.Player) tile = Tiles.Alive;
            else tile = Tiles.EnemyArea;
                
            Tilemap.SetTile(pos, tile);
        });
    }
}
