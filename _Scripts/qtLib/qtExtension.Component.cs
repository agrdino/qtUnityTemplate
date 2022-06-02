using Unity.VisualScripting;
using UnityEngine;

public static partial class qtExtension{
    public static T TryGetComponent<T>(this Transform target) where T : Component
    {
        var t = target.GetComponent<T>();
        if (t == null)
        {
            t = target.AddComponent<T>();
        }

        return t;
    }
    
    public static T TryGetComponent<T>(this GameObject target) where T : Component
    {
        var t = target.GetComponent<T>();
        if (t == null)
        {
            t = target.AddComponent<T>();
        }

        return t;
    }
}