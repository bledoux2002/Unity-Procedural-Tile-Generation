using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.Linq;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour
{
    private GameObject cam;
    public int radiusX; //x range of tiles to check for generation
    public int radiusY; //same but for y
    private GameObject mapManagerObj;
    private MapManager mapManager;
    [HideInInspector] public Tilemap map;
    private List<TileData> tileDatas;
    private Dictionary<TileBase, TileData> dataFromTiles;
    private Vector2Int origin;

    // Awake is called before Start, best for assigning components
    void Awake()
    {
        cam = GameObject.Find("Main Camera");
        mapManagerObj = GameObject.Find("MapManager");
        mapManager = mapManagerObj.GetComponent<MapManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //mapManager = mapmanagerObj.GetComponent<script>("MapManager");


        origin.x = RoundValue(cam.transform.position.x);
        origin.y = RoundValue(cam.transform.position.y);


        for (int y = radiusY; y >= -radiusY; y--)
        {
            for (int x = -radiusX; x <= radiusX; x++)
            {
                //from top left to bottom right, generate any missing tiles (currently at (0, 0), focused on camera
                if (map.GetTile(new Vector3Int (x + origin.x, y + origin.y, 0)) == null)
                {
                    try
                    {
                        TileBase changeTile = generateTile(x + origin.x, y + origin.y);
                        map.SetTile(new Vector3Int(x + origin.x, y + origin.y, 0), changeTile);
                    }
                    catch
                    {
                        Debug.Log("Impossible combination at " + new Vector2Int(x, y));
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3Int currentCell = map.WorldToCell(cam.transform.position);

        for (int y = radiusY; y >= -radiusY; y--)
        {
            for (int x = -radiusX; x <= radiusX; x++)
            {
                //from top left to bottom right, generate any missing tiles (currently at (0, 0), focused on camera
                if (map.GetTile(new Vector3Int(x + currentCell.x, y + currentCell.y, 0)) == null)
                {
                    try
                    {
                        TileBase changeTile = generateTile(x + currentCell.x, y + currentCell.y);
                        map.SetTile(new Vector3Int(x + currentCell.x, y + currentCell.y, 0), changeTile);
                    }
                    catch
                    {
                        Debug.Log("Impossible combination at " + new Vector2Int(x, y));
                    }
            }
            }
        }

        //map.SetTile(currentCell, changeTile);
    }

    //Used to round out the values inputted (prob a built-in way to do this but it makes me feel smart)
    int RoundValue(float val)
    {
        float offset = val % 1; //how far offcenter cam is from tiles
        double valDouble;

        if ((offset >= 0.5) || ((offset >= -0.5) && (offset < 0)))
        {
            valDouble = Math.Ceiling(val);
        }
        else if ((offset < 0.5) && ((offset < 0.5) && (offset > 0)))
        {
            valDouble = Math.Floor(val);
        }

        int valNew = Convert.ToInt32(val);

        return valNew;
    }

    //returns tile to be generated, takes (x, y) coords to look around at nearby tiles and their properties
    TileBase generateTile(int x, int y)
    {
        List<TileBase[]> tileLists = new List<TileBase[]>();

        try
        {
            TileBase[] n = mapManager.dataFromTiles[map.GetTile(new Vector3Int(x, y + 1, 0))].south; //find compatible tiles for south edge of north tile
            tileLists.Add(n);
            //Debug.Log(n.Length + " compatible tiles north of " + new Vector2Int(x, y));
        }
        catch
        {
            TileBase[] n = new TileBase[0];
            //Debug.Log("Empty tile north of " + new Vector2Int(x, y));
        }
        
        try
        {
            TileBase[] e = mapManager.dataFromTiles[map.GetTile(new Vector3Int(x + 1, y, 0))].west;
            tileLists.Add(e);
            //Debug.Log(e.Length + " compatible tiles east of " + new Vector2Int(x, y));
        }
        catch
        {
            TileBase[] e = new TileBase[0];
            //Debug.Log("Empty tile east of " + new Vector2Int(x, y));
        }

        try
        {
            TileBase[] s = mapManager.dataFromTiles[map.GetTile(new Vector3Int(x, y - 1, 0))].north;
            tileLists.Add(s);
            //Debug.Log(s.Length + " compatible tiles south of " + new Vector2Int(x, y));
        }
        catch
        {
            TileBase[] s = new TileBase[0];
            //Debug.Log("Empty tile south of " + new Vector2Int(x, y));
        }

        try
        {
            TileBase[] w = mapManager.dataFromTiles[map.GetTile(new Vector3Int(x - 1, y, 0))].east;
            tileLists.Add(w);
            //Debug.Log(w.Length + " compatible tiles west of " + new Vector2Int(x, y));
        }
        catch
        {
            TileBase[] w = new TileBase[0];
            //Debug.Log("Empty tile west of " + new Vector2Int(x, y));
        }

        //Debug.Log(tileLists.Count());

        IEnumerable<TileBase> compTiles;
        if (tileLists.Count() > 0)
        {
            //Debug.Log("There are " + tileLists.Count() + " tiles surrounding " + new Vector2Int(x, y));
            compTiles = tileLists[0];
            if (tileLists.Count() > 1)
            {
                for (int i = 1; i < tileLists.Count; i++)
                {
                    compTiles = compTiles.Intersect(tileLists[i]);
                }
            }
            int index = Convert.ToInt32(Math.Floor(Random.Range(0.0f, (float)compTiles.Count())));
            //Debug.Log(compTiles.Count());
            return compTiles.ElementAt(index);
        }
        
        //Debug.Log(mapManager.dataFromTiles.Count);
        int place = Convert.ToInt32(Math.Floor(Random.Range(0.0f, (float)mapManager.dataFromTiles.Count)));
        //Debug.Log(place);
        KeyValuePair<TileBase, TileData> pair = mapManager.dataFromTiles.ElementAt(place);
        Debug.Log(pair.Key);
        return pair.Key;
    }


    //hash table constantly updating with loaded tiles in rendered area, values shift over as needed
    //tile class with tile component, hashtable/variables/array of corner values
}
