using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.Linq;
using Random = UnityEngine.Random;
using Debug = UnityEngine.Debug;
using System.Diagnostics;

public class MapGenerator : MonoBehaviour
{
    private GameObject _cam;
    public Vector2Int _radius; //radius of tiles to generate
    private GameObject _mapManagerObj; //mapmng object
    private MapManager _mapManager; //mapmng script component

    public Tilemap _map;
    public Tilemap _fog; //tilemap for fog of war
    public TileBase _fogTile; //tile for fog
    private List<TileData> _tileDatas; //all tiles for generating (if its not in here, it wont be generated, even if its in comp lists)
    private Dictionary<TileBase, TileData> _dataFromTiles; //linking tiles to their tiledatas
    private Vector2Int _origin; //wherever the camera starts off in the scene for initial generation

    // Awake is called before Start, best for assigning components
    void Awake()
    {
        _cam = GameObject.Find("Main Camera");
        _mapManagerObj = GameObject.Find("MapManager");
        _mapManager = _mapManagerObj.GetComponent<MapManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _origin.x = RoundValue(_cam.transform.position.x);
        _origin.y = RoundValue(_cam.transform.position.y);
        //Debug.Log(_map.GetTile(new Vector3Int(0, 0, 0)));
//        HideMap(_origin); //apply fog of war to start area
        SpiralGen(_origin); //generate tiles within set range
    }

    // Update is called once per frame
    //same as start, only checking to see if more tiles should be generated
/*    void Update()
    {
        Vector3Int currentCell3D = _map.WorldToCell(_cam.transform.position);
        Vector2Int currentCell = new Vector2Int(currentCell3D.x, currentCell3D.y);
        HideMap(currentCell);
        SpiralGen(currentCell);
    }*/
    
    //cover range in fog of war top left to bottom right
//could maybe use a smaller function to simplify code? lots of repetition in HideMap()
    void HideMap(Vector2Int input)
    {
        for (int y = _radius.y + 2; y >= -_radius.y - 2; y--)
        {
            for (int x = -_radius.x - 2; x <= _radius.x + 2; x++)
            {
                if (_map.GetTile(new Vector3Int(x + input.x, y + input.y, 0)) == null)
                {
                    if (_fog.GetTile(new Vector3Int(x + input.x - 1, y + input.y, 0)) == null)
                    {
                        _fog.SetTile(new Vector3Int(x + input.x - 1, y + input.y, 0), _fogTile);
                    }
                    if (_fog.GetTile(new Vector3Int(x + input.x, y + input.y, 0)) == null)
                    {
                        _fog.SetTile(new Vector3Int(x + input.x, y + input.y, 0), _fogTile);
                    }
                    if (_fog.GetTile(new Vector3Int(x + input.x - 1, y + input.y - 1, 0)) == null)
                    {
                        _fog.SetTile(new Vector3Int(x + input.x - 1, y + input.y - 1, 0), _fogTile);
                    }
                    if (_fog.GetTile(new Vector3Int(x + input.x, y + input.y - 1, 0)) == null)
                    {
                        _fog.SetTile(new Vector3Int(x + input.x, y + input.y - 1, 0), _fogTile);
                    }
                }
            }
        }
    }

    //generate tiles spiral-form
    void SpiralGen(Vector2Int input)
    {
        
        //https://stackoverflow.com/questions/3706219/algorithm-for-iterating-over-an-outward-spiral-on-a-discrete-2d-grid-from-the-or

        //current position
        int x = 0;
        int y = 0;

        //direction of movement vector
        int dx = 1;
        int dy = 0;

        //current "segment" length and number of segments iterated thru
        int segment_length = 1;
        int segment_passed = 0;

        //total number of tiles to generate based on radius
        int area = (2 * _radius.x + 1) * (2 * _radius.y + 1);
        //Debug.Log(area);

        //whether valid generation has been found
//        bool genSuccess = false;

        for (int k = 0; k < area; k++) //iterate thru entire area
        {
            if (_map.GetTile(new Vector3Int(x + input.x, y + input.y, 0)) == null) //if empty tile
            {
                TileBase[,] grid = new TileBase[7, 7]; //create temp grid for simulation around empty tile
                for (int gx = 0; gx < 7; gx++)
                {
                    for (int gy = 6; gy > -1; gy--)
                    {
                        //Debug.Log(gx + ", " + gy);
                        grid[gx, gy] = _map.GetTile(new Vector3Int(x + input.x + gx - 3, y + input.y + gy - 3, 0)); //fill temp grid with existing tiles
                    }
                }

                BeforeLoop:
                //try until succeeded
 //               while (!genSuccess)
 //               {
 //                   genSuccess = true;

                    //for each tile in 5x5 grid to be simulated (top left to bottom right)
                    for (int gx = 1; gx < 6; gx++)
                    {
                        for (int gy = 5; gy > 0; gy--)
                        {
//                            if (genSuccess)
//                            {
                                //Debug.Log(gx + ", " + gy);
                                //create mini grid of 3x3 section to input into selectTile()
                                TileBase[,] tempGrid = new TileBase[3, 3];
                                for (int tx = 0; tx < 3; tx++)
                                {
                                    for (int ty = 2; ty > -1; ty--)
                                    {
                                        //Debug.Log(tx + ", " + ty);
                                        tempGrid[tx, ty] = grid[tx + gx - 1, ty + gy - 1];
                                    }
                                }
                                //EVERYTHING UP TO HERE SHOULD WORK, GOING TO REWORK selectTile() NEXT
                                TileBase tempTile = selectTile(tempGrid);
                                if (tempTile != null)
                                {
                                    grid[gx, gy] = tempTile;
                                } else {
                                //genSuccess = false;
                                    goto BeforeLoop;
                                }
//                            }
                        }
                    }
                //}
                TileBase changeTile = grid[3, 3];
                _map.SetTile(new Vector3Int(x + input.x, y + input.y, 0), changeTile); //SHOULD PROBABLY ALLOW FOR Z MANIPULATION
                /*int reqTilesNum = _mapManager.__dataFromTiles[changeTile].reqTiles.Count();
                if (reqTilesNum > 0)
                {
                    for (int j = 0; j < reqTilesNum; j++)
                    {
                        int index = Convert.ToInt32(Math.Floor(Random.Range(0.0f, (float)_mapManager.__dataFromTiles[changeTile].reqTiles[j].tiles.Count())));
                        _map.SetTile(new Vector3Int(x + input.x + _mapManager.__dataFromTiles[changeTile].reqTilesCoords[j].x, y + input.y + _mapManager.__dataFromTiles[changeTile].reqTilesCoords[j].y, 0), _mapManager._dataFromTiles[changeTile].reqTiles[j].tiles[index]);
                    }
                }*/

                /*try
                    {
                        TileBase changeTile = selectTile(x + input.x, y + input.y);
                        _map.SetTile(new Vector3Int(x + input.x, y + input.y, 0), changeTile); //SHOLD PROBABLY ALLOW FOR Z MANIPULATION
                        int reqTilesNum = _mapManager._dataFromTiles[changeTile].reqTiles.Count();
                        if (reqTilesNum > 0)
                        {
                            for (int j = 0; j < reqTilesNum; j++)
                            {
                                int index = Convert.ToInt32(Math.Floor(Random.Range(0.0f, (float)_mapManager._dataFromTiles[changeTile].reqTiles[j].tiles.Count())));
                                _map.SetTile(new Vector3Int(x + input.x + _mapManager._dataFromTiles[changeTile].reqTilesCoords[j].x, y + input.y + _mapManager._dataFromTiles[changeTile].reqTilesCoords[j].y, 0), _mapManager._dataFromTiles[changeTile].reqTiles[j].tiles[index]);
                            }
                        }
                    }
                    catch
                    {
                        Debug.Log("Impossible combination at " + new Vector2Int(x, y));
                        genSuccess = false;
                        //break; //freezes game

                    }*/

                //Continue thru spiral
                x += dx;
                y += dy;
                segment_passed++;
                //Debug.Log(x + ", " + y);
                if (segment_passed == segment_length)
                {
                    segment_passed = 0;
                    int buffer = dx;
                    dx = -dy;
                    dy = buffer;

                    if (dy == 0)
                    {
                        segment_length++;
                    }
                }
            }
        }
        

        /*
        for (int y = _radius.y; y >= -_radius.y; y--)
        {
            for (int x = -_radius.x; x <= _radius.x; x++)
            {
                //from top left to bottom right, generate any missing tiles (currently at (0, 0), focused on camera
                if (_map.GetTile(new Vector3Int(x + input.x, y + input.y, 0)) == null)
                {
                    try
                    {
                        TileBase changeTile = selectTile(x + input.x, y + input.y);
                        _map.SetTile(new Vector3Int(x + input.x, y + input.y, 0), changeTile); //SHOLD PROBABLY ALLOW FOR Z MANIPULATION
                        int reqTilesNum = _mapManager._dataFromTiles[changeTile].reqTiles.Count();
                        if (reqTilesNum > 0)
                        {
                            for (int i = 0; i < reqTilesNum; i++)
                            {
                                int index = Convert.ToInt32(Math.Floor(Random.Range(0.0f, (float)_mapManager._dataFromTiles[changeTile].reqTiles[i].tiles.Count())));
                                _map.SetTile(new Vector3Int(x + input.x + _mapManager._dataFromTiles[changeTile].reqTilesCoords[i].x, y + input.y + _mapManager._dataFromTiles[changeTile].reqTilesCoords[i].y, 0), _mapManager._dataFromTiles[changeTile].reqTiles[i].tiles[index]);
                            }
                        }

                        //remove fog
                        if (_fog.GetTile(new Vector3Int(x + input.x - 1, y + input.y, 0)) != null)
                        {
                            _fog.SetTile(new Vector3Int(x + input.x - 1, y + input.y, 0), null);
                        }
                        if (_fog.GetTile(new Vector3Int(x + input.x, y + input.y, 0)) != null)
                        {
                            _fog.SetTile(new Vector3Int(x + input.x, y + input.y, 0), null);
                        }
                        if (_fog.GetTile(new Vector3Int(x + input.x - 1, y + input.y - 1, 0)) != null)
                        {
                            _fog.SetTile(new Vector3Int(x + input.x - 1, y + input.y - 1, 0), null);
                        }
                        if (_fog.GetTile(new Vector3Int(x + input.x, y + input.y - 1, 0)) != null)
                        {
                            _fog.SetTile(new Vector3Int(x + input.x, y + input.y - 1, 0), null);
                        }
                    }
                    catch
                    {
                        Debug.Log("Impossible combination at " + new Vector2Int(x, y));
                    }
                }
            }
        }*/
    }

    //Used to round out the values inputted (prob a built-in way to do this but it makes me feel smart)
    int RoundValue(float val)
    {
        float offset = val % 1; //how far offcenter _cam is from tiles
        double valDouble;

        if ((offset >= 0.5) || ((offset >= -0.5) && (offset < 0)))
        {
            valDouble = Math.Ceiling(val);
        } else if ((offset < 0.5) && ((offset < 0.5) && (offset > 0))) {
            valDouble = Math.Floor(val);
        }

        int valNew = Convert.ToInt32(val);

        return valNew;
    }

    //REWORK TO USE GRID INPUT INSTEAD OF MAP
    //returns tile to be generated, takes (x, y) coords to look around at nearby tiles and their properties
    TileBase selectTile(TileBase[,] grid)
    {
        //List of lists of TileBases, will be used to find intersect between all sets of compatible tiles for edges of adjacent tiles
        List<List<TileBase>> tileLists = new List<List<TileBase>>();

        //if there's a tile on a given side, add the list of edge-compatible tiles to tileLists
        //catch, empty list, don't add to tileLists
        TileData[] n;
        if (_map.GetTile(new Vector3Int(x, y + 1, 0)) != null)
        {
            Debug.Log("Non-empty tile North of " + new Vector2Int(x, y));
            n = _mapManager._dataFromTiles[_map.GetTile(new Vector3Int(x, y + 1, 0))].south; //find compatible tiles for south edge of north tile
        } else if (grid[1, 2] != null) {
            Debug.Log("Non-empty tile simulated North of " + new Vector2Int(x, y));
            n = _mapManager._dataFromTiles[grid[1, 2]].south; //find compatible tiles for south edge of north tile
        } else {
            n = new TileData[0];
            Debug.Log("Empty tile north of " + new Vector2Int(x, y));
        }

        if (n.Length > 0)
        {
            List<TileBase> tempList = new List<TileBase>();
            for (int i = 0; i < n.Length; i++)
            {
                for (int j = 0; j < n[i].tiles.Length; j++)
                {
                    tempList.Add(n[i].tiles[j]);
                }
            }
            //Debug.Log(n.Length + " compatible tiles north of " + new Vector2Int(x, y));
            tileLists.Add(tempList);
        }
        
        try
        {
            TileData[] e = _mapManager._dataFromTiles[_map.GetTile(new Vector3Int(x + 1, y, 0))].west;
            //tileLists[1] = new List<TileBase>();
            List<TileBase> tempList = new List<TileBase>();
            for (int i = 0; i < e.Length; i++)
            {
                for (int j = 0; j < e[i].tiles.Length; j++)
                {
                    tempList.Add(e[i].tiles[j]);
                }
            }
            //Debug.Log(e.Length + " compatible tiles east of " + new Vector2Int(x, y));
            tileLists.Add(tempList);
        }
        catch
        {
            TileData[] e = new TileData[0];
            //Debug.Log("Empty tile east of " + new Vector2Int(x, y));
        }

        try
        {
            TileData[] s = _mapManager._dataFromTiles[_map.GetTile(new Vector3Int(x, y - 1, 0))].north;
            //tileLists[2] = new List<TileBase>();
            List<TileBase> tempList = new List<TileBase>();
            for (int i = 0; i < s.Length; i++)
            {
                for (int j = 0; j < s[i].tiles.Length; j++)
                {
                    tempList.Add(s[i].tiles[j]);
                }
            }
            //Debug.Log(s.Length + " compatible tiles south of " + new Vector2Int(x, y));
            tileLists.Add(tempList);
        }
        catch
        {
            TileData[] s = new TileData[0];
            //Debug.Log("Empty tile south of " + new Vector2Int(x, y));
        }

        try
        {
            TileData[] w = _mapManager._dataFromTiles[_map.GetTile(new Vector3Int(x - 1, y, 0))].east;
            //Debug.Log("Length of w = " + w.Length + "type = " + w[0].GetType());
            //tileLists[3] = new List<TileBase>(); //ERROR HERE
            List<TileBase> tempList = new List<TileBase>();
            //Debug.Log("Length of tileLists = " + tileLists.Count);
            for (int i = 0; i < w.Length; i++)
            {
                for (int j = 0; j < w[i].tiles.Length; j++)
                {
                    tempList.Add(w[i].tiles[j]);
                }
                //Debug.Log("there are " + w[i].tiles.Length + "tiles west"); //PROBLEM AREA
            }
            //Debug.Log(w.Length + " compatible tiles west of " + new Vector2Int(x, y));
            //Debug.Log("Tile list length = " + tileLists.Count);
            tileLists.Add(tempList);
        }
        catch
        {
            TileData[] w = new TileData[0];
            //Debug.Log("Empty tile west of " + new Vector2Int(x, y));
        }

        //ADD CORNER CHECKS, THIS IS GONNA SUCK
        try
        {
            TileData[] nw = _mapManager._dataFromTiles[_map.GetTile(new Vector3Int(x - 1, y + 1, 0))].southeast;
            List<TileBase> tempList = new List<TileBase>();
            for (int i = 0; i < nw.Length; i++)
            {
                for (int j = 0; j < nw[i].tiles.Length; j++)
                {
                    tempList.Add(nw[i].tiles[j]);
                }
            }
            //Debug.Log(nw.Length + " compatible tiles northwest of " + new Vector2Int(x, y));
            tileLists.Add(tempList);
        }
        catch
        {
            TileData[] nw = new TileData[0];
            //Debug.Log("Empty tile northwest of " + new Vector2Int(x, y));
        }

        try
        {
            TileData[] ne = _mapManager._dataFromTiles[_map.GetTile(new Vector3Int(x + 1, y + 1, 0))].southwest;
            List<TileBase> tempList = new List<TileBase>();
            for (int i = 0; i < ne.Length; i++)
            {
                for (int j = 0; j < ne[i].tiles.Length; j++)
                {
                    tempList.Add(ne[i].tiles[j]);
                }
            }
            //Debug.Log(nw.Length + " compatible tiles northwest of " + new Vector2Int(x, y));
            tileLists.Add(tempList);
        }
        catch
        {
            TileData[] ne = new TileData[0];
            //Debug.Log("Empty tile northeast of " + new Vector2Int(x, y));
        }

        try
        {
            TileData[] se = _mapManager._dataFromTiles[_map.GetTile(new Vector3Int(x + 1, y - 1, 0))].northwest;
            List<TileBase> tempList = new List<TileBase>();
            for (int i = 0; i < se.Length; i++)
            {
                for (int j = 0; j < se[i].tiles.Length; j++)
                {
                    tempList.Add(se[i].tiles[j]);
                }
            }
            //Debug.Log(nw.Length + " compatible tiles northwest of " + new Vector2Int(x, y));
            tileLists.Add(tempList);
        }
        catch
        {
            TileData[] se = new TileData[0];
            //Debug.Log("Empty tile southeast of " + new Vector2Int(x, y));
        }

        try
        {
            TileData[] sw = _mapManager._dataFromTiles[_map.GetTile(new Vector3Int(x - 1, y - 1, 0))].northeast;
            List<TileBase> tempList = new List<TileBase>();
            for (int i = 0; i < sw.Length; i++)
            {
                for (int j = 0; j < sw[i].tiles.Length; j++)
                {
                    tempList.Add(sw[i].tiles[j]);
                }
            }
            //Debug.Log(nw.Length + " compatible tiles northwest of " + new Vector2Int(x, y));
            tileLists.Add(tempList);
        }
        catch
        {
            TileData[] sw = new TileData[0];
            //Debug.Log("Empty tile southwest of " + new Vector2Int(x, y));
        }

        //List of compatible tiles with adjacent tiles
        IEnumerable<TileBase> compTiles;
        //Debug.Log("There are " + tileLists.Count().ToString() + " TileBase lists");
        //Debug.Log(tileLists[0]);

        //if there is at least one adjacent tile, use that tile's edge-compatible tile list
        if (tileLists.Count() > 0)
        {
            Debug.Log("There are " + tileLists.Count() + " tiles surrounding " + new Vector2Int(x, y));
            compTiles = tileLists[0];
            Debug.Log("There are " + compTiles.Count() + " compTiles in one cardinal direction");
            //Debug.Log(compTiles.ElementAt(0));

            //if there is more than one adjacent tile, continuously find intersect of compTiles and each of the rest of the adjacent tiles' edge-compatible tile lists
            if (tileLists.Count() > 1)
            {
                for (int i = 1; i < tileLists.Count(); i++)
                {
                    //FINDING INTERSECT BETWEEN EACH INDIVIDUAL TILE?
                    IEnumerable<TileBase> temp = tileLists.ElementAt(i);
                    //Debug.Log("temp length = " + temp.Count() + ", tilelist length = " + tileLists[i].Count());
                    compTiles = compTiles.Intersect(temp);
                }
            }

            //Debug.Log(compTiles.Count());

            //select a random tile from compTiles to return
            //THIS WILL LATER BE REPLACED WITH GAUSSIAN DISTRIBUTION
            //int index = Convert.ToInt32(Math.Floor(Random.Range(0.0f, (float)compTiles.Count())));

            //Gaussian Selection of tile

            //list of the chances elected by the dev
            List<double> chances = new List<double>();
            for (int i = 0; i < compTiles.Count(); i++)
            {
                chances.Add((double)_mapManager._dataFromTiles[compTiles.ElementAt(i)].spawnChance);
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
                } else if (tempDif == dif) {//if equal, randomly select between the two (if there are multiple tiles with the same chances)
                    if (Random.Range(0f, 1f) >= 0.5f)
                    {
                        index = i; //no need to reassign dif since its the same as tempDif
                    }
                }
            }

            //try
            //{
            //Debug.Log(index);
            //Debug.Log(compTiles.Count());
            Debug.Log(compTiles.Count().ToString() + ", " + index.ToString());
            return compTiles.ElementAt(index);
            /*}
            catch
            {
                Debug.Log("Impossible combination at " + new Vector2Int(x, y));
                //regenerateTiles(x, y);
            }*/
        } else {
        
            //if there are no adjacent tiles, select a random tile to begin with and return it
            //in my current implementation, this is only ever called once: generation and movement never skips a line so there will always be at least one adjacent tile after the beginning
            int place = Convert.ToInt32(Math.Floor(Random.Range(0.0f, (float)_mapManager._dataFromTiles.Count)));
            
            
            KeyValuePair<TileBase, TileData> pair = _mapManager._dataFromTiles.ElementAt(place);
            return pair.Key;
        }
    }

    //looks at adjacent tiles and regenerates them if necessary
        //might be difficult to implement since it wont have context of which replacement will result in the least subsequent replacements
    /*
    //might be unnecessary once simulation method is completed
    void regenerateTiles(int x, int y)
    {
        Dictionary<string, int[]> adjacentTiles = new Dictionary<string, int[]>();
        if (_map.GetTile(new Vector3Int(x, y + 1, 0)) != null)
        {
            //adjacentTiles['north'] = _map.getTile(new Vector3Int(x, y + 1, 0))
            adjacentTiles["north"] = new int[] { x, y + 1 };
        }
        if (_map.GetTile(new Vector3Int(x + 1, y, 0)) != null)
        {
            //adjacentTiles['east'] = _map.getTile(new Vector3Int(x + 1, y, 0))
            adjacentTiles["east"] = new int[] { x + 1, y };
        }
        if (_map.GetTile(new Vector3Int(x, y - 1, 0)) != null)
        {
            //adjacentTiles['south'] = _map.getTile(new Vector3Int(x, y - 1, 0))
            adjacentTiles["south"] = new int[] { x, y - 1 };
        }
        if (_map.GetTile(new Vector3Int(x - 1, y, 0)) != null)
        {
            //adjacentTiles['west'] = _map.getTile(new Vector3Int(x - 1, y, 0))
            adjacentTiles["west"] = new int[] { x - 1, y };
        }

        int index = Convert.ToInt32(Math.Floor(Random.Range(0.0f, (float)adjacentTiles.Count)));
        KeyValuePair<string, int[]> pair = adjacentTiles.ElementAt(index);
        
        selectTile(pair.Key[0], pair.Key[1]);
    }*/

    //gaussian double selection, used instead of randomized tile selection
    public static double RandomGaussian() //figure out how the dist actually works, make it so 0 is never spawn, and up to some number is more likely
    {
        double u1 = Random.Range(0f, 1f);
        double u2 = Random.Range(0f, 1f);
        double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
        double randNormal = 3 * randStdNormal; //random normal(mean,stdDev^2)

        return randNormal;
    }
}
