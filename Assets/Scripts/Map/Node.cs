using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// a node for A* pathfinding
public class Node
{
    protected internal Vector2Int coords;
    protected internal int FCost
    {
        get { return gCost + hCost; }
    }
    protected internal int gCost;
    protected internal int hCost;
    protected internal bool traversable;
    //protected internal int movementCost;
    protected internal List<Node> neighbors;
    protected internal Node parent;

    // constructors & initialization
    private void Init()
    {
        coords = new Vector2Int(0, 0);
        gCost = 0;
        hCost = 0;
        traversable = true;
        //movementCost = 10;
        neighbors = new List<Node>();
        parent = null;
    }

    public Node()
    {
        Init();
    }

    public Node(Vector2Int _coords, bool _traversable)
    {
        Init();
        coords = _coords;
        traversable = _traversable;
    }
}
