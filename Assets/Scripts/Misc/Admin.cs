using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Admin
{
    public static void CreateMyAsset()
    {
        CustomTile asset = ScriptableObject.CreateInstance<CustomTile>();


    }
}

// ### ENUMERATIONS ###########################################################################################

public enum FaceDirection { None, Left, Right }

public enum Terrain
{
    None, Ground, Wall, Water,
}

public enum MentalState // mental state for an NPC
{
    Idle, Alert
}

