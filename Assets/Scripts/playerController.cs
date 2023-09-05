using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class playerController : MonoBehaviour
{
    public GameObject cam;
    public int radiusX; //x range of tiles to check for generation
    public int radiusY; //same but for y
    public Tilemap map;
    public TileBase changeTile;
    private Vector2 input;
    private Vector2 origin;
    private bool isXAxisInUse = false;
    private bool isYAxisInUse = false;

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

        float camX = cam.transform.position.x % 1; //how far offcenter cam is from tiles
        float camY = cam.transform.position.y % 1;
        if (((camX % 1) > 0.5) && ((camX % 1))


        //Debug.Log(-116.2345 % 1); //-0.2345
        //Debug.Log(Math.Floor(-12.3)); //-13
        origin.x = Convert.ToInt32(Math.Floor(cam.transform.position.x));
        origin.y = Convert.ToInt32(Math.Floor(cam.transform.position.y));
        //Debug.Log(cam.transform.position);
        //Debug.Log(origin);

        for (int y = radiusY; y >= -radiusY; y--)
        {
            for (int x = -radiusX; x <= radiusX; x++)
            {
                //from top left to bottom right, generate any missing tiles (currently at (0, 0), not on player/cam
                if (map.GetTile(new Vector3Int (x, y, 0)) == null)
                {
                    map.SetTile(new Vector3Int(x, y, 0), changeTile);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
        if (input.x != 0) { input.y = 0; } //remove diagonal movement
        if (input.y != 0) { input.x = 0; }
        if (input.x != 0)
        {
            if (isXAxisInUse == false)
            {
                transform.position = transform.position + new Vector3(input.x, input.y, 0);
                isXAxisInUse = true;
            }
        }
        if (input.x == 0)
        {
            isXAxisInUse = false;
        }
        if (input.y != 0)
        {
            if (isYAxisInUse == false)
            {
                transform.position = transform.position + new Vector3(input.x, input.y, 0);
                isYAxisInUse = true;
            }
        }
        if (input.y == 0)
        {
            isYAxisInUse = false;
        }

        Vector3Int currentCell = map.WorldToCell(transform.position);
        

//        for (y = )

        map.SetTile(currentCell, changeTile);
    }


    //hash table constantly updating with loaded tiles in rendered area, values shift over as needed
    //tile class with tile component, hashtable/variables/array of corner values
}
