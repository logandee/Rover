using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    public static Testing instance;

    void Awake()
    {
        if (instance == null)
        {
            //DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.T))
        //{
        //    foreach(GridSquare gs in Game.MapGrid)
        //    {
        //        Debug.Log($"GS at {gs.GridCoords} has terrain of type {gs.TerrainType.GetType().Name} and its walkability is {gs.Walkable}");
        //    }
        //}
    }
}
