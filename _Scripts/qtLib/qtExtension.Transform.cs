using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class qtExtension
{
    public static GameObject GetChild(this Transform target, string name)
    {
        for (int i = 0; i < target.childCount; i++)
        {
            if (target.GetChild(i).name.Equals(name))
            {
                return target.GetChild(i).gameObject;
            }
        }
        return null;
    }
}
