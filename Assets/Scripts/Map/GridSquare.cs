using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// a grid square of the overall map's grid
public class GridSquare
{
    public Vector2Int GridCoords { get; set; }

    // I don't think there will be a need for a TileCoords property

    public CustomTile Tile { get; set; } // the tile object which corresponds to this grid square

    public Item GroundItem { get; set; } // the item on the ground in this grid square, if any

    public TerrainType TerrainType // for convenience
    {
        get
        {
            return Game.instance.TerrainTypesDict[Tile.terrain.ToString()];
        }
    }

    public bool Walkable
    {
        get
        {
            if (TerrainType.Traversable)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public List<Unit> Occupiers { get; set; } // when this list's count is 0, it means it is unoccupied

    // poss property: the item (if any) in this GS


    private void InitGridSquare()
    {
        GridCoords = new Vector2Int(0, 0);
        Occupiers = new List<Unit>();
    }

    public GridSquare()
    {
        InitGridSquare();
    }

    public GridSquare(Vector2Int _gridCoords)
    {
        InitGridSquare();
        GridCoords = _gridCoords;
    }
}
