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
        //List of lists of TileBases, will be used to find intersect between all sets of compatible tiles for edges of adjacent tiles
        List<TileBase[]> tileLists = new List<TileBase[]>();

        //if there's a tile on a given side, add the list of edge-compatible tiles to tileLists
        //catch, empty list, don't add to tileLists
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

        try
        {
            TileBase[] nw = mapManager.dataFromTiles[map.GetTile(new Vector3Int(x - 1, y + 1, 0))].southeast;
            tileLists.Add(nw);
            //Debug.Log(nw.Length + " compatible tiles northwest of " + new Vector2Int(x, y));
        }
        catch
        {
            TileBase[] nw = new TileBase[0];
            //Debug.Log("Empty tile northwest of " + new Vector2Int(x, y));
        }

        //CORNER CHECKS ARE NOT NECESSARY, NEXT 4 TRY-CATCHES ARE REDUNDANT

        try
        {
            TileBase[] ne = mapManager.dataFromTiles[map.GetTile(new Vector3Int(x + 1, y + 1, 0))].southwest;
            tileLists.Add(ne);
            //Debug.Log(ne.Length + " compatible tiles northeast of " + new Vector2Int(x, y));
        }
        catch
        {
            TileBase[] nw = new TileBase[0];
            //Debug.Log("Empty tile northeast of " + new Vector2Int(x, y));
        }

        try
        {
            TileBase[] se = mapManager.dataFromTiles[map.GetTile(new Vector3Int(x + 1, y - 1, 0))].northwest;
            tileLists.Add(se);
            //Debug.Log(se.Length + " compatible tiles southeast of " + new Vector2Int(x, y));
        }
        catch
        {
            TileBase[] se = new TileBase[0];
            //Debug.Log("Empty tile southeast of " + new Vector2Int(x, y));
        }

        try
        {
            TileBase[] sw = mapManager.dataFromTiles[map.GetTile(new Vector3Int(x - 1, y - 1, 0))].northeast;
            tileLists.Add(sw);
            //Debug.Log(sw.Length + " compatible tiles southwest of " + new Vector2Int(x, y));
        }
        catch
        {
            TileBase[] sw = new TileBase[0];
            //Debug.Log("Empty tile southwest of " + new Vector2Int(x, y));
        }

        //List of compatible tiles with adjacent tiles
        IEnumerable<TileBase> compTiles;

        //if there is at least one adjacent tile, use that tile's edge-compatible tile list
        if (tileLists.Count() > 0)
        {
            //Debug.Log("There are " + tileLists.Count() + " tiles surrounding " + new Vector2Int(x, y));
            compTiles = tileLists[0];

            //if there is more than one adjacent tile, continuously find intersect of compTiles and each of the rest of the adjacent tiles' edge-compatible tile lists
            if (tileLists.Count() > 1)
            {
                for (int i = 1; i < tileLists.Count; i++)
                {
                    compTiles = compTiles.Intersect(tileLists[i]);
                }
            }

            //select a random tile from compTiles to return
            //THIS WILL LATER BE REPLACED WITH GAUSSIAN DISTRIBUTION
            //int index = Convert.ToInt32(Math.Floor(Random.Range(0.0f, (float)compTiles.Count())));

            //Gaussian Selection of tile

            //list of the chances elected by the dev
            List<double> chances = new List<double>();
            for (int i = 0; i < compTiles.Count(); i++)
            {
                chances.Add((double)mapManager.dataFromTiles[compTiles.ElementAt(i)].spawnChance);
            }

            //random gaussian number, will find the closest tile "chance" to the selection and save its index in the list
            double selection = RandomGaussian();
            double dif = 6d;
            int index = 0;

            for (int i = 0; i < chances.Count(); i++)
            {
                double tempDif = Math.Abs(chances[i] - selection); //find the difference

                //if smaller than previous smallest, overwrite
                if (tempDif < dif)
                {
                    dif = tempDif;
                    index = i;
                }
                else if (tempDif == dif) //if equal, randomly select between the two (if there are multiple tiles with the same chances)
                {
                    if (Random.Range(0f, 1f) >= 0.5f)
                    {
                        index = i; //no need to reassign dif since its the same as tempDif
                    }
                }
            }

            //try
            //{
                return compTiles.ElementAt(index);
            /*}
            catch
            {
                Debug.Log("Impossible combination at " + new Vector2Int(x, y));
                //regenerateTiles(x, y);
            }*/
        }
        
        //if there are no adjacent tiles, select a random tile to begin with and return it
            //in my current implementation, this is only ever called once: generation and movement never skips a line so there will always be at least one adjacent tile after the beginning
        int place = Convert.ToInt32(Math.Floor(Random.Range(0.0f, (float)mapManager.dataFromTiles.Count)));
        
        
        KeyValuePair<TileBase, TileData> pair = mapManager.dataFromTiles.ElementAt(place);
        return pair.Key;
    }

    //looks at adjacent tiles and regenerates them if necessary
        //might be difficult to implement since it wont have context of which replacement will result in the least subsequent replacements
    void regenerateTiles(int x, int y)
    {
        Dictionary<string, int[]> adjacentTiles = new Dictionary<string, int[]>();
        if (map.GetTile(new Vector3Int(x, y + 1, 0)) != null)
        {
            //adjacentTiles['north'] = map.getTile(new Vector3Int(x, y + 1, 0))
            adjacentTiles["north"] = new int[] { x, y + 1 };
        }
        if (map.GetTile(new Vector3Int(x + 1, y, 0)) != null)
        {
            //adjacentTiles['east'] = map.getTile(new Vector3Int(x + 1, y, 0))
            adjacentTiles["east"] = new int[] { x + 1, y };
        }
        if (map.GetTile(new Vector3Int(x, y - 1, 0)) != null)
        {
            //adjacentTiles['south'] = map.getTile(new Vector3Int(x, y - 1, 0))
            adjacentTiles["south"] = new int[] { x, y - 1 };
        }
        if (map.GetTile(new Vector3Int(x - 1, y, 0)) != null)
        {
            //adjacentTiles['west'] = map.getTile(new Vector3Int(x - 1, y, 0))
            adjacentTiles["west"] = new int[] { x - 1, y };
        }

        int index = Convert.ToInt32(Math.Floor(Random.Range(0.0f, (float)adjacentTiles.Count)));
        KeyValuePair<string, int[]> pair = adjacentTiles.ElementAt(index);
        
        generateTile(pair.Key[0], pair.Key[1]);
    }

    public static double RandomGaussian()
    {
        double u1 = Random.Range(0f, 1f);
        double u2 = Random.Range(0f, 1f);
        double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
        double randNormal = 3 * randStdNormal; //random normal(mean,stdDev^2)

        return randNormal;
    }

    //hash table constantly updating with loaded tiles in rendered area, values shift over as needed
    //tile class with tile component, hashtable/variables/array of corner values
}
