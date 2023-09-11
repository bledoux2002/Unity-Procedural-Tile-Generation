using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class MapGenerator : MonoBehaviour
{
    public GameObject cam;
    public int radiusX; //x range of tiles to check for generation
    public int radiusY; //same but for y
    public Tilemap map;
    public TileBase changeTile;
    private Vector2Int origin;

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log(transform.position);
        //Vector2 pCoords = new Vector2(transform.position.x, transform.position.y); //curent player coords in tilemap (only int for now, player moves entire int every movement)
        //Debug.Log(pCoords);
        //private tileTable = ;
        //private 
        //Debug.Log(changeTile);
        //Debug.Log(map.GetTile(new Vector3Int (0, 1, 0))); //prints tilename
        //Debug.Log(map.GetTile(new Vector3Int (0, 2, 0))); //prints Null



        //Debug.Log(-116.2345 % 1); //-0.2345
        //Debug.Log(Math.Floor(-12.3)); //-13
        origin.x = RoundValue(cam.transform.position.x);
        origin.y = RoundValue(cam.transform.position.y);
        //Debug.Log(cam.transform.position);
        //Debug.Log(origin);

        for (int y = radiusY; y >= -radiusY; y--)
        {
            for (int x = -radiusX; x <= radiusX; x++)
            {
                //from top left to bottom right, generate any missing tiles (currently at (0, 0), focused on camera
                if (map.GetTile(new Vector3Int (x + origin.x, y + origin.y, 0)) == null)
                {
                    map.SetTile(new Vector3Int(x + origin.x, y + origin.y, 0), changeTile);
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
                    map.SetTile(new Vector3Int(x + currentCell.x, y + currentCell.y, 0), changeTile);
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


    //hash table constantly updating with loaded tiles in rendered area, values shift over as needed
    //tile class with tile component, hashtable/variables/array of corner values
}
