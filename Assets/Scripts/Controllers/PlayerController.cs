using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Player player; // for convenience

    private void Awake()
    {
        player = GetComponent<Player>();
        if (player == null)
        {
            Debug.LogError("Player is null for the PlayerController, but it shouldn't be!");
        }
    }

    private void Update()
    {
        if (UnitManager.instance.playersTurn || Game.instance.Enemies.Count == 0)
        {
            if (CheckForMovementInput())
            {
                bool movementSuccessful = false; // false by default
                Vector2Int destinationCoords = new Vector2Int(-1, -1);

                if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) // up
                {
                    int newYCoord = player.Coords.y + 1;
                    if (newYCoord < Game.MapGrid.GetLength(1))
                    {
                        destinationCoords = new Vector2Int(player.Coords.x, newYCoord);
                    }
                }
                else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) // left
                {
                    int newXCoord = player.Coords.x - 1;
                    if (newXCoord >= 0)
                    {
                        destinationCoords = new Vector2Int(newXCoord, player.Coords.y);
                    }
                }
                else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) // down
                {
                    int newYCoord = player.Coords.y - 1;
                    if (newYCoord >= 0)
                    {
                        destinationCoords = new Vector2Int(player.Coords.x, newYCoord);
                    }
                }
                else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) // right
                {
                    int newXCoord = player.Coords.x + 1;
                    if (newXCoord < Game.MapGrid.GetLength(0))
                    {
                        destinationCoords = new Vector2Int(newXCoord, player.Coords.y);
                    }
                }

                if (destinationCoords.x != -1 && destinationCoords.y != -1)
                {
                    movementSuccessful = player.AttemptMovementByPlayer(destinationCoords);

                    // attempt attack if an enemy was occupying the spot the PC tried to move to
                    if (!movementSuccessful)
                    {
                        GridSquare targetGS = Game.MapGrid[destinationCoords.x, destinationCoords.y];
                        if (targetGS.Occupiers.Count > 0)
                        {
                            Unit targetEnemy = targetGS.Occupiers[0];
                            player.Attack(targetEnemy);

                        }
                    }

                    UnitManager.instance.playersTurn = false;
                }
            }
        }
    }

    private bool CheckForMovementInput()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) 
            || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow) 
            || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)
            || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
