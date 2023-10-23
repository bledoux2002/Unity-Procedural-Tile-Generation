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
    public static MapManager instance;
    public Tilemap map;

    [SerializeField]
    private List<TileData> tileDatas;

    [HideInInspector]
    public Dictionary<TileBase, TileData> dataFromTiles;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);

        if (File.Exists(Application.dataPath + "/levelData.json"))
        {
            LoadLevel();
        }

        dataFromTiles = new Dictionary<TileBase, TileData>();

        foreach (var tileData in tileDatas)
        {
            foreach (var tile in tileData.tiles)
            {
                dataFromTiles.Add(tile, tileData);
            }
        }
    }
    /*
    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.F)) SaveLevel();
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.G)) LoadLevel();
    }*/

    public void SaveLevel()
    {
        BoundsInt bounds = map.cellBounds;
        LevelData levelData = new LevelData();
        levelData.charPos = GameObject.FindWithTag("Player").transform.position;

        for (int x = bounds.min.x; x <= bounds.max.x; x++)
        {
            for (int y = bounds.min.y; y <= bounds.max.y; y++)
            {
                TileBase temp = map.GetTile(new Vector3Int(x, y, 0));
                
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

        map.ClearAllTiles();

        GameObject.FindWithTag("Player").transform.position = data.charPos;

        for (int i = 0; i < data.pos.Count; i++)
        {
            map.SetTile(data.pos[i], data.tiles[i]);
        }
    }
}

public class LevelData
{
    public Vector3 charPos = new Vector3();
    public List<TileBase> tiles = new List<TileBase>();
    public List<Vector3Int> pos = new List<Vector3Int>();
}