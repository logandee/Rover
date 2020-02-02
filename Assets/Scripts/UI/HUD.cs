using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour
{
    public static HUD instance; // singleton

    public Dictionary<string, GameObject> uiPrefabsDict; // keyed by prefab name

    public GameObject healthPanel; // AVI

    private List<GameObject> fullHearts;

    private void Awake()
    {
        InitHUD();
    }

    private void Start()
    {
        for (int i = 0; i < 3; i++)
        {
            AddFullHeart();
        }
    }

    public void AddFullHeart()
    {
        GameObject heart = Instantiate(uiPrefabsDict["Heart"], healthPanel.transform);
        fullHearts.Add(heart);
    }

    public void RemoveFullHeart()
    {
        if (fullHearts.Count > 0)
        {
            GameObject heartToRemove = fullHearts[fullHearts.Count - 1];
            fullHearts.Remove(heartToRemove);
            Destroy(heartToRemove);
        }
        else
        {
            Debug.Log("There are no more hearts to remove from fullHearts!");
        }
        
    }

    private void LoadUIPrefabs()
    {
        string[] uiPrefabPaths = System.IO.Directory.GetFiles(Application.dataPath + "/Resources/Prefabs/UI", "*.prefab");
        foreach (string path in uiPrefabPaths)
        {
            Debug.Log($"path = {path}");
            string fn = System.IO.Path.GetFileNameWithoutExtension(path);
            GameObject curUIPrefab = Resources.Load<GameObject>("Prefabs/UI/" + fn);
            uiPrefabsDict.Add(fn, curUIPrefab);
        }

        Debug.Log($"uiPrefabsDict.Count = {uiPrefabsDict.Count}");
        foreach (var key in uiPrefabsDict.Keys)
        {
            Debug.Log($"key: {key}");
        }

    }

    private void InitHUD()
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

        uiPrefabsDict = new Dictionary<string, GameObject>();

        LoadUIPrefabs();

        fullHearts = new List<GameObject>();
    }
}
