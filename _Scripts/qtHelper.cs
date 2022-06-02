using System.Collections;
using System.Collections.Generic;
using _Scripts.Helper;
using UnityEngine;
using UnityEngine.SceneManagement;

public class qtHelper
{
    private static string[] _paths;
    public static GameObject FindObjectWithPath(GameObject root, string path)
    {
        _paths = path.Split('/');
        GameObject result = root;
        for (int i = 0; i < _paths.Length; i++)
        {
            result = FindObjectInChildren(result, _paths[i]);
            if (!result)
            {
                qtLogging.LogError($"Cant find {_paths[i]} in {result.name}");
                return null;
            }
        }

        return result;
    }

    public static GameObject FindObjectInChildren(GameObject root, string name)
    {
        for (int i = 0; i < root.transform.childCount; i++)
        {
            if (root.transform.GetChild(i).name == name)
            {
                return root.transform.GetChild(i).gameObject;
            }
        }
        qtLogging.LogError($"Cant find {name} in {root.name}");
        return null;
    } 
    
    public static GameObject FindObjectIncludingInactive(GameObject root, string name)
    {
        if (root.transform.childCount <= 0)
        {
            qtLogging.LogError($"{root} have no child!");
            return null;
        }
        GameObject result = null;

        if (root.name == name)
        {
            return root;
        }

        for (int i = 0; i < root.transform.childCount; i++)
        {
            result = FindObjectIncludingInactive(root.transform.GetChild(i).gameObject, name);
            if (result)
            {
                return result;
            }
        }
        
        qtLogging.LogError($"Cant find {name} in {root.name}");
        return null;
    }
    
    public static GameObject FindObjectInRoot(string name)
    {
        return GameObject.Find(name);
    }
    
    public static GameObject FindObjectInRootIncludingInactive(string name)
    {
        Scene scene = SceneManager.GetActiveScene();
        if (!scene.isLoaded)
        {
            qtLogging.LogError("No scene loaded");
            return null;
        }

        var roots = new List<GameObject>();
        scene.GetRootGameObjects(roots);

        foreach (GameObject obj in roots)
        {
            if (obj.transform.name == name) return obj;
        }
        
        qtLogging.LogError($"Cant find {name} in root");
        return null;
    }
}
