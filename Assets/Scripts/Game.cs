using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// the overall manager class for the game -- this is the main script!
public class Game : MonoBehaviour
{
    public static Game instance; // singleton

    public Tilemap tilemap; // assigned via inspector (AVI) -- actually, I don't think this is assigned by the inspector anymore
    public DunMap DungeonMap { get; set; }

    public static int MapWidth { get; set; } // might be moved to a map class
    public static int MapHeight { get; set; } // might be moved to a map class
    public static GridSquare[,] MapGrid { get; set; }
    public Node[,] NodeGrid; // for A* pathfinding

    public static System.Random Rand { get; set; } // the global Random object

    // a given tile only needs to be loaded once -- it is much like a type for itself, since the same tile can be used across the tilemap
    // keyed by the name of the tile
    public Dictionary<string, CustomTile> TilesDict { get; set; }

    public Dictionary<string, TerrainType> TerrainTypesDict { get; set; } // keyed by the name of the terrain type


    public List<Enemy> Enemies { get; set; }

    // Awake is always called before Start
    void Awake()
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

        InitGame();

        LoadTiles();

        GenerateTerrainTypes();

        // TODO: procedural generation of map should get called here
        InitializeMapGrid();

        InitializeNodeGrid(); // grid squares must be assigned their tiles before this point!

        // TODO: create & initialize player
    }

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("Good to go.");

        //UnitManager.instance.SpawnUnit();

        // FOR TESTING
        StartCoroutine(CallEnemyMovementInterval());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // TEMPORARY
    private IEnumerator CallEnemyMovementInterval()
    {
        while (true)
        {
            foreach(Enemy enemy in Enemies)
            {
                bool movementAttempt = enemy.AI.AttemptMovementByAI();
                Debug.Log($"{enemy.UnitName}'s movement attempt = {movementAttempt}");
            }

            yield return new WaitForSeconds(1f);
        }
    }

    private void LoadTiles()
    {
        string[] tilePaths = System.IO.Directory.GetFiles(Application.dataPath + "/Resources/Tiles", "*.asset");
        foreach (string path in tilePaths)
        {
            string fn = System.IO.Path.GetFileNameWithoutExtension(path);
            CustomTile curTile = Resources.Load<CustomTile>("Tiles/" + fn);
            TilesDict.Add(fn, curTile);
        }
    }

    private void GenerateTerrainTypes()
    {
        // q&d
        TerrainTypesDict = new Dictionary<string, TerrainType>();
        TerrainType currentTerrainType = null;

        currentTerrainType = new Ground();
        TerrainTypesDict.Add(currentTerrainType.GetType().Name, currentTerrainType);
        //Debug.Log(currentTerrainType.GetType().Name);

        currentTerrainType = new Wall();
        TerrainTypesDict.Add(currentTerrainType.GetType().Name, currentTerrainType);

        currentTerrainType = new Water();
        TerrainTypesDict.Add(currentTerrainType.GetType().Name, currentTerrainType);
    }

    private void InitializeMapGrid()
    {
        // TEMPORARY IMPLEMENTATION -- SCAFFOLDING FOR TESTING

        MapWidth = 50;
        MapHeight = 50;
        DungeonMap = new DunMap(MapWidth, MapHeight);
        DungeonMap.FillMap(15, 5, 10);


        GameObject gridGO = new GameObject();
        gridGO.name = "GeneratedGrid";
        //gridGO.transform.SetParent(worldMapGO.transform);
        Grid grid = gridGO.AddComponent<Grid>();
        Vector3 gsSize = new Vector3(1, 1, 0);
        grid.cellSize = new Vector3(gsSize.x, gsSize.y, 0f);

        GameObject mapTilemapGO = new GameObject();
        mapTilemapGO.name = "Map Tilemap";
        mapTilemapGO.transform.SetParent(gridGO.transform);
        Tilemap mapTilemap = mapTilemapGO.AddComponent<Tilemap>();
        mapTilemapGO.AddComponent<TilemapRenderer>();

        mapTilemap.size = new Vector3Int(MapWidth, MapHeight, 0);

        tilemap = mapTilemap; // important

        MapGrid = new GridSquare[MapWidth, MapHeight];

        //System.Random rand = new System.Random(); // for some randomization that happens within the loop // SCHEDULED FOR DELETION - REPLACED BY GLOBAL RAND

        for (int y = 0; y < MapHeight; y++)
        {
            for (int x = 0; x < MapWidth; x++)
            {
                GridSquare gs = new GridSquare(new Vector2Int(x, y));
                MapGrid[x, y] = gs;

                // for if the level is manually designed -- doesn't currently work and I don't plan to fix it since we don't need it
                //gs.Tile = (CustomTile)tilemap.GetTile(new Vector3Int(gs.GridCoords.x, gs.GridCoords.y, 0));

                //string tileName = "defaultGround"

                string tileName = DungeonMap.MapArr[x,y];

                // a bit of randomization
                //if (x > 2 || y > 2) // ensures we create a safe space
                //{
                //    int randNum = rand.Next(10);
                //    if (randNum > 7)
                //    {
                //        tileName = "defaultWall";
                //    }
                //    else if (randNum == 9)
                //    {
                //        tileName = "defaultWater";
                //    }
                //}

                // for if the level is procedurally generated
                tilemap.SetTile(new Vector3Int(gs.GridCoords.x, gs.GridCoords.y, 0), TilesDict[tileName]);
                gs.Tile = (CustomTile)tilemap.GetTile(new Vector3Int(gs.GridCoords.x, gs.GridCoords.y, 0));

                // debugging
                //if (gs.Tile != null)
                //{
                //    Debug.Log($"At {gs.GridCoords}, its tile's terrain type is {gs.Tile.terrain.ToString()}");
                //}
                //else
                //{
                //    Debug.Log($"At {gs.GridCoords}, the tile is apparently null!");
                //}
            }
        }
    }



        private void InitializeNodeGrid()
    {
        // initialize node grid
        NodeGrid = new Node[MapGrid.GetLength(0), MapGrid.GetLength(1)];
        foreach (GridSquare gs in MapGrid)
        {
            Node newNode = new Node(gs.GridCoords, gs.Walkable);
            NodeGrid[gs.GridCoords.x, gs.GridCoords.y] = newNode;
        }

        // check for and set neighbor nodes of each node
        foreach (Node currentNode in NodeGrid)
        {
            int i = 1;
            while (i < 5)
            {
                Vector2Int coordsModifier = new Vector2Int(0, 0);
                switch (i)
                {
                    case 1: // N grid square
                        coordsModifier = new Vector2Int(0, 1);
                        break;
                    case 2: // E gs
                        coordsModifier = new Vector2Int(1, 0);
                        break;
                    case 3: // S gs
                        coordsModifier = new Vector2Int(0, -1);
                        break;
                    case 4: // W gs
                        coordsModifier = new Vector2Int(-1, 0);
                        break;
                    /* for diagonal movement
                    case 1: // (0,1) grid square
                        coordsModifier = new Vector2Int(0, 1);
                        break;
                    case 2: // (1,1) gs
                        coordsModifier = new Vector2Int(1, 1);
                        break;
                    case 3: // (1,0) gs
                        coordsModifier = new Vector2Int(1, 0);
                        break;
                    case 4: // (1,-1) gs
                        coordsModifier = new Vector2Int(1, -1);
                        break;
                    case 5: // (0, -1) gs
                        coordsModifier = new Vector2Int(0, -1);
                        break;
                    case 6: // (-1, -1) gs
                        coordsModifier = new Vector2Int(-1, -1);
                        break;
                    case 7: // (-1, 0) gs
                        coordsModifier = new Vector2Int(-1, 0);
                        break;
                    case 8: // (-1, 1) gs
                        coordsModifier = new Vector2Int(-1, 1);
                        break;
                        */
                }

                Vector2Int modifiedCoords = new Vector2Int(currentNode.coords.x + coordsModifier.x, currentNode.coords.y + coordsModifier.y);
                // ensure index is in range
                if (modifiedCoords.x >= 0 && modifiedCoords.y >= 0)
                {
                    if (modifiedCoords.x < NodeGrid.GetLength(0) && modifiedCoords.y < NodeGrid.GetLength(1))
                    {
                        currentNode.neighbors.Add(NodeGrid[modifiedCoords.x, modifiedCoords.y]);
                    }
                }
                i++;
            }
        }
    }

    private void InitGame()
    {
        TerrainTypesDict = new Dictionary<string, TerrainType>();
        TilesDict = new Dictionary<string, CustomTile>();

        Enemies = new List<Enemy>();

        Rand = new System.Random();
    }
}
