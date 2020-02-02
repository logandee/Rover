using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager instance; // singleton

    public Dictionary<string, GameObject> itemTypesDict; // keyed by item type

    private GameObject groundItemsGO;

    // possible property: list of all existing items

    private void Awake()
    {
        InitItemManager();
    }

    private void Start()
    {
        // TESTING
        for (int i = 0; i < 7; i++)
        {
            GridSquare randGS = Utility.FindRandomUnoccupiedWalkableGridSquare();
            if (randGS.GroundItem == null)
            {
                SpawnItemOnGround("Minor Health Potion", randGS.GridCoords);
                Debug.Log($"Spawned a minor health potion at {randGS.GridCoords}.");
            }
            else
            {
                Debug.Log($"Failed b/c GS at {randGS} had an item already!");
            }
        }
    }

    private void SpawnItemOnGround(string itemName, Vector2Int coords)
    {
        if (itemTypesDict[itemName] != null)
        {
            bool coordsAreValid = Utility.CheckIfCoordsAreValid(coords);
            if (coordsAreValid)
            {
                float x = (float)coords.x + Item.Offset.x;
                float y = (float)coords.y + Item.Offset.y;

                GameObject item = Instantiate(itemTypesDict[itemName], groundItemsGO.transform);
                item.transform.position = new Vector3(x, y, 0);

                Game.MapGrid[coords.x, coords.y].GroundItem = item.GetComponent<Item>();
            }
        }
    }

    public void PurgeItem(Item item)
    {
        Destroy(item.gameObject);
    }

    private void LoadItemTypes()
    {
        string[] itemTypePaths = System.IO.Directory.GetFiles(Application.dataPath + "/Resources/Prefabs/Items", "*.prefab");
        foreach (string path in itemTypePaths)
        {
            Debug.Log($"path = {path}");
            string fn = System.IO.Path.GetFileNameWithoutExtension(path);
            GameObject curItemType = Resources.Load<GameObject>("Prefabs/Items/" + fn);
            itemTypesDict.Add(fn, curItemType);
        }

        Debug.Log($"itemTypes.Count = {itemTypesDict.Count}");
        foreach (var key in itemTypesDict.Keys)
        {
            Debug.Log($"key: {key}");
        }
        
    }

    private void InitItemManager()
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

        itemTypesDict = new Dictionary<string, GameObject>();

        groundItemsGO = new GameObject("Ground Items");

        LoadItemTypes();
    }
}
