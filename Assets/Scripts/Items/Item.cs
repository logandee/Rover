using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public string itemName;

    public Unit Owner { get; set; }
    public Vector2Int Coords { get; set; } // coords of the item (when it's on the ground)

    // offset so that item sprites are centered in grid squares
    public static Vector2 Offset
    {
        get
        {
            return new Vector2(0.5f, 0.5f);
        }
    }

    public SpriteRenderer Rend { get; set; }

    // poss property: value

    protected virtual void Awake()
    {
        Rend = GetComponent<SpriteRenderer>();
    }
}
