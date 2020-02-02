using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// the base class for terrain types
// each class which derives from this is a specific terrain type
public class TerrainType
{
    public int MovementCost { get; set; }
    public bool ObstructsLOS { get; set; }
    public bool Traversable { get; set; }
}


public class Ground : TerrainType
{
    public Ground()
    {
        MovementCost = 1;
        ObstructsLOS = false;
        Traversable = true;
    }
}

public class Wall : TerrainType
{
    public Wall()
    {
        MovementCost = 1;
        ObstructsLOS = true;
        Traversable = false;
    }
}

public class Water : TerrainType
{
    public Water()
    {
        MovementCost = 1;
        ObstructsLOS = false;
        Traversable = true;
    }
}
