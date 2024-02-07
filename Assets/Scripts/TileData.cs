using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]

public class TileData : ScriptableObject
{

    // The closer to 0, the more likely the tile is spawned
    [Range(0f, 6f)]
    public double spawnChance = 3d;

    public TileBase[] tiles;

    // These lists define compatible tiles on each edge (north can only be paired with tiles x and y, while east can only be paired with tile z)
    public TileData[] north, east, south, west, northwest, northeast, southeast, southwest;

    // Required adjacent tiles (multi-tile structures), <tile, relevant coords>
    public TileData[] reqTiles;
    public Vector3Int[] reqTilesCoords;
}
