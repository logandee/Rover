using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// AI for NPCs
public class ArtInt
{

    public Enemy Self { get; set; } // currently this is of type Enemy b/c all NPCs are of type Enemy

    public MentalState MentalState { get; set; }

    // no need for a target property b/c right now the target will always be the player character

    public Vector2Int DestinationCoords { get; set; }

    public List<Vector2Int> CoordsPath { get; set; }

    public Node[,] NodeGrid // for convenience
    {
        get
        {
            return Game.instance.NodeGrid;
        }
    }


    public bool AttemptMovementByAI()
    {
        if (CoordsPath.Count > 0 && Self.Coords != DestinationCoords)
        {
            GridSquare[,] grid = Game.MapGrid; // for convenience

            FindPath(grid[Self.Coords.x, Self.Coords.y], grid[DestinationCoords.x, DestinationCoords.y]);

            int x = CoordsPath[0].x;
            int y = CoordsPath[0].y;
            GridSquare gsToMoveTo = grid[x, y];

            bool canMoveToCoords = CheckMovementValidity(gsToMoveTo);

            if (canMoveToCoords)
            {
                Self.MoveToGridSquare(grid[Self.Coords.x, Self.Coords.y], gsToMoveTo);
                CoordsPath.RemoveAt(0); // after successful movement, remove GS from path
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public void FindPath(GridSquare startingGridSquare, GridSquare targetGridSquare)
    {
        List<Node> openNodes = new List<Node>();
        List<Node> closedNodes = new List<Node>();
        Node startNode = NodeGrid[startingGridSquare.GridCoords.x, startingGridSquare.GridCoords.y];
        Node targetNode = NodeGrid[targetGridSquare.GridCoords.x, targetGridSquare.GridCoords.y];

        // I'm pretty sure that only the starting node's g & h costs need to be reset
        //the rest of the nodes can potentially start with inaccurate g & h costs, but their g & h costs are set properly before they have to be accurate
        startNode.gCost = 0;
        startNode.hCost = 0;

        openNodes.Add(startNode);

        int debugCounter = 0;
        while (openNodes.Count > 0)
        {
            debugCounter++;
            if (debugCounter > 5000)
            {
                Debug.Log("Something went wrong here! This ran over 5,000 times w/out success!");
                break;
            }

            // find node in OPEN with lowest f cost
            int lowestFCost = 1000000; // q&d (b/c semi-arbitrary)
            Node nodeWithLowestFCost = null;
            foreach (Node curNode in openNodes)
            {
                if (curNode.FCost < lowestFCost)
                {
                    nodeWithLowestFCost = curNode;
                    lowestFCost = nodeWithLowestFCost.FCost;
                }
                else if (curNode.FCost == lowestFCost && curNode.hCost < nodeWithLowestFCost.hCost) // in case of tied f costs
                {
                    nodeWithLowestFCost = curNode;
                    lowestFCost = nodeWithLowestFCost.FCost;
                }
            }
            Node currentNode = nodeWithLowestFCost;

            openNodes.Remove(currentNode);
            closedNodes.Add(currentNode);

            if (currentNode == targetNode)
            {
                List<Node> nodePath = RetracePath(startNode, targetNode);
                ConvertPathToCoords(nodePath);
                return;
            }

            foreach (Node neighbor in currentNode.neighbors)
            {
                if (!neighbor.traversable || closedNodes.Contains(neighbor))
                {
                    continue; // just skip to next neighbor
                }
                else if (!openNodes.Contains(neighbor) || (currentNode.gCost + 1) < neighbor.gCost) // q&d
                {
                    neighbor.gCost = currentNode.gCost + 1; // q&d
                    neighbor.hCost = Utility.GetManhattanDistance(neighbor.coords, targetNode.coords);
                    neighbor.parent = currentNode;
                    if (!openNodes.Contains(neighbor))
                    {
                        openNodes.Add(neighbor);
                    }
                }
            }
        }

    }

    private List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        return path;
    }

    private void ConvertPathToCoords(List<Node> path)
    {
        CoordsPath = new List<Vector2Int>();

        foreach (Node currentNode in path)
        {
            CoordsPath.Add(new Vector2Int(currentNode.coords.x, currentNode.coords.y));
        }
    }

    public bool CheckMovementValidity(GridSquare gridSquare)
    {
        if (gridSquare.Occupiers.Count == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    private void InitArtInt()
    {
        Self = null;
        MentalState = MentalState.Idle;
        CoordsPath = new List<Vector2Int>();
    }

    public ArtInt()
    {
        InitArtInt();
    }

    public ArtInt(Enemy _self)
    {
        InitArtInt();
        Self = _self;
    }
}
