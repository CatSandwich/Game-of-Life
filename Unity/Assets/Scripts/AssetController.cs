using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AssetController : MonoBehaviour
{
    public static AssetController Inst;

    [Header("Tiles")]
    public Tile EnemyArea;
    public Tile NonPlaying;
    public Tile Empty;
    public Tile Alive;
    public Tile Dead;
    
    void Start()
    {
        if (Inst != null)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Inst = this;
    }
}
