using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]

public class TileData: ScriptableObject
{

    // The closer to 0, the more likely the tile is spawned
    [Range(0f, 6f)]
    public double spawnChance = 3f;

    public TileBase[] tiles;

    // These lists define compatible tiles on each edge (north can only be paired with tiles 1, 4, 17, while east can only be paired with tiles 3, 18, 35)
    public TileBase[] north, east, south, west/*, northwest, northeast, southeast, southwest*/;

    //CORNER CHECKS ARE REDUNDANT, CAN BE REMOVED BUT I'M LEAVING THEM OUT OF SPITE
}
