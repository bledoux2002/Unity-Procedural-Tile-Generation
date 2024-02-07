using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using Debug = UnityEngine.Debug;
using Application = UnityEngine.Application;

public class MapManager : MonoBehaviour
{
    public static MapManager _instance;
    public Tilemap _map;

    //tiles can only be generated if they are listed here in Unity
    [SerializeField]
    private List<TileData> _tileDatas;

    //links tiles with associated tiledata objects
    [HideInInspector]
    public Dictionary<TileBase, TileData> _dataFromTiles;

    private void Awake()
    {
        if (_instance == null) _instance = this;
        else Destroy(this);

        if (File.Exists(Application.dataPath + "/levelData.json")) LoadLevel();

        _dataFromTiles = new Dictionary<TileBase, TileData>();

        foreach (var tileData in _tileDatas)
        {
            foreach (var tile in tileData.tiles)
            {
                _dataFromTiles.Add(tile, tileData);
            }
        }
    }

    public void SaveLevel()
    {
        BoundsInt bounds = _map.cellBounds;
        LevelData levelData = new LevelData();
        levelData.charPos = GameObject.FindWithTag("Player").transform.position;

        for (int x = bounds.min.x; x <= bounds.max.x; x++)
        {
            for (int y = bounds.min.y; y <= bounds.max.y; y++)
            {
                TileBase temp = _map.GetTile(new Vector3Int(x, y, 0));
                
                if (temp != null)
                {
                    //Debug.Log(x.ToString() + ", " + y.ToString());
                    levelData.tiles.Add(temp);
                    levelData.pos.Add(new Vector3Int(x, y, 0));
                }
            }
        }

        string json = JsonUtility.ToJson(levelData, true);
        File.WriteAllText(Application.dataPath + "/levelData.json", json);

        Debug.Log("Tilemap saved.");
    }

    public void LoadLevel()
    {
        string json = File.ReadAllText(Application.dataPath + "/levelData.json");
        LevelData data = JsonUtility.FromJson<LevelData>(json);

        _map.ClearAllTiles();

        GameObject.FindWithTag("Player").transform.position = data.charPos;

        for (int i = 0; i < data.pos.Count; i++)
        {
            _map.SetTile(data.pos[i], data.tiles[i]);
        }
    }

    public void ClearLevel()
    {
        _map.ClearAllTiles();
        if (File.Exists(Application.dataPath + "/levelData.json"))
        {
            _map.ClearAllTiles();
            File.Delete(Application.dataPath + "/levelData.json");
        }
    }
}

public class LevelData
{
    public Vector3 charPos = new Vector3();
    public List<TileBase> tiles = new List<TileBase>();
    public List<Vector3Int> pos = new List<Vector3Int>();
}