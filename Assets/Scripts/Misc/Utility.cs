using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// utility class for useful functionality
public static class Utility
{
    public static int GetManhattanDistance(Vector2Int firstPoint, Vector2Int secondPoint)
    {
        int mDistance = Mathf.Abs(firstPoint.x - secondPoint.x) + Mathf.Abs(firstPoint.y - secondPoint.y);
        return mDistance;
    }

    public static bool CheckIfCoordsAreValid(Vector2Int coords, bool checkForWalkable = true, bool checkForOccupiers = true, bool checkForItems = false)
    {
        bool coordsValidity = false; // defaults to false

        if (coords.x >= 0 && coords.x < Game.MapGrid.GetLength(0)
            && coords.y >= 0 && coords.y < Game.MapGrid.GetLength(1))
        {
            coordsValidity = true;

            GridSquare gs = Game.MapGrid[coords.x, coords.y];

            if (checkForWalkable)
            {
                if (!gs.Walkable)
                {
                    coordsValidity = false;
                }
            }

            if (checkForOccupiers)
            {
                if (gs.Occupiers.Count > 0)
                {
                    coordsValidity = false;
                }
            }

            if (checkForItems)
            {
                if (gs.GroundItem != null)
                {
                    coordsValidity = false;
                }
            }
        }

        return coordsValidity;
    }

    public static GridSquare FindRandomUnoccupiedWalkableGridSquare()
    {
        //System.Random rand = new System.Random(); // SCHEDULED FOR DELETION -- REPLACED BY GLOBAL RAND
        int infiniteLoopPreventer = 0;

        while (infiniteLoopPreventer < 25000)
        {
            int xCoord = Game.Rand.Next(Game.MapWidth);
            int yCoord = Game.Rand.Next(Game.MapHeight);

            GridSquare gs = Game.MapGrid[xCoord, yCoord];
            if (gs.Occupiers.Count == 0 && gs.Walkable)
            {
                return gs;
            }

            infiniteLoopPreventer++;
        }

        Debug.LogError($"Failed to find a suitable grid square after {infiniteLoopPreventer} attempts!");
        return null;
    }

}
