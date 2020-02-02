using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager instance; // singleton

    public bool playersTurn = true; // check if it is the players turn
    public static Player Player { get; set; }

    public List<Enemy> Enemies { get; set; }

    private GameObject enemiesGO; // serves as the parent for all enemy GameObjects so that the inspector isn't cluttered with them

    private void Awake()
    {
        InitUnitManager();
    }

    private void Start()
    {
        GameObject playerGO = new GameObject("PlayerCharacter");
        playerGO.AddComponent<Player>();
        Camera mainCam = FindObjectOfType<Camera>();
        mainCam.gameObject.transform.parent = playerGO.transform;

        // ### player spawning
        //SpawnUnit(Player, Utility.FindRandomUnoccupiedWalkableGridSquare());
        Debug.Log($"Room list count: {Game.instance.DungeonMap.RoomList.Count}");
        Debug.Log($"Room One center: {Game.instance.DungeonMap.RoomList[0].Center}");
        Vector2Int centerOfRoomOne = Game.instance.DungeonMap.RoomList[0].Center;
        SpawnUnit(Player, Game.MapGrid[centerOfRoomOne.y, centerOfRoomOne.x]); // USES Y, THEN X ORDER b/c of David's procedural generation implementation
        Debug.Log($"Player coords after spawning: {Player.Coords}");

        // ### enemy spawning
        SpawnEnemies(13);
    }

    // Update is called once per frame
    void Update()
    {
        if (!playersTurn)
        {
            HaveEnemiesAct();
        }
    }

    // bool return value is for whether the spawning was successful
    public bool SpawnUnit(Unit unit, GridSquare gs)
    {
        if (gs.Occupiers.Count == 0 && gs.Walkable)
        {
            unit.MoveToGridSquare(null, gs);
            return true;
        }
        else
        {
            return false;
        }
    }

    private void SpawnEnemies(int numEnemies)
    {
        // planning to extend this to be able to account for multiple enemy types, but for now it only deals with one type of enemy

        for(int i = 0; i < numEnemies; i++)
        {
            GameObject enemyGO = new GameObject($"Enemy {i}");
            Enemy enemy = enemyGO.AddComponent<Enemy>();
            SpawnUnit(enemy, Utility.FindRandomUnoccupiedWalkableGridSquare());
            enemyGO.transform.parent = enemiesGO.transform;
        }
    }

    private void HaveEnemiesAct()
    {
        int numEnemiesWhoMovedThisRound = 0;

        foreach (Enemy enemy in Enemies)
        {
            enemy.AI.FindPath(Game.MapGrid[enemy.Coords.x, enemy.Coords.y], Game.MapGrid[UnitManager.Player.Coords.x, UnitManager.Player.Coords.y]); // very q&d!

            enemy.AI.DestinationCoords = Player.Coords;
            //Debug.Log($"{enemy.UnitName}'s coords path count: {enemy.AI.CoordsPath.Count}");
            bool movementAttempt = enemy.AI.AttemptMovementByAI();
            //Debug.Log($"{enemy.UnitName}'s movement attempt = {movementAttempt}");

            if (movementAttempt)
            {
                numEnemiesWhoMovedThisRound++;
            }
            else if (!movementAttempt)
            {
                // TODO: if enemy's movement attempt failed and it is adjacent to player, have it attempt to attack player
                GridSquare[,] grid = Game.MapGrid;
                int x = enemy.AI.CoordsPath[0].x;
                int y = enemy.AI.CoordsPath[0].y;
                GridSquare targetGS = grid[x, y];
                if (targetGS.Occupiers.Count > 0 && targetGS.Occupiers[0] is Player)
                {
                    Unit targetEnemy = targetGS.Occupiers[0];
                    int hitChance = Game.Rand.Next(3);
                    if(hitChance == 2)
                    {
                        enemy.Attack(targetEnemy);
                    }

                }
            }

        }

        //Debug.Log($"numEnemiesWhoMovedThisRound = {numEnemiesWhoMovedThisRound}");

        playersTurn = true;
    }

    private void InitUnitManager()
    {
        // establishing singleton
        if (instance == null)
        {
            //DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        Enemies = new List<Enemy>();

        enemiesGO = new GameObject("Enemies");
    }
}
