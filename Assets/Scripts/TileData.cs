using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]

public class TileData: ScriptableObject
{
    public TileBase[] tiles;

    // These lists define compatible tiles on each edge (north can only be paired with tiles 1, 4, 17, while east can only be paired with tiles 3, 18, 35)
    public TileBase[] north, east, south, west, northwest, northeast, southeast, southwest;



}
