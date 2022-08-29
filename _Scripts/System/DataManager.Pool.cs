using System;
using _Scripts.qtLib.Extension;
using UnityEngine;

namespace _Scripts.System
{
    public partial class DataManager
    {
        public static GameObject Instantiate(string path, Transform parent = null, bool isForceCreateNew = false)
        {
            var prefab = Resources.Load<GameObject>(path);
            var name = path.Substring(path.LastIndexOf("/") + 1);
            if (parent == null)
            {
                return qtPooling.Instance.Spawn(name, prefab, isForceCreateNew);
            }
            else
            {
                return qtPooling.Instance.Spawn(name, prefab, parent, isForceCreateNew);
            }
        }

        public static T Instantiate<T>(string path, Transform parent = null, bool isForceCreateNew = false) where T : Component
        {
            var prefab = Resources.Load<GameObject>(path);
            var name = path.Substring(path.LastIndexOf("/") + 1);
            if (parent == null)
            {
                return qtPooling.Instance.Spawn(name, prefab, isForceCreateNew).TryGetComponent<T>();
            }
            else
            {
                return qtPooling.Instance.Spawn(name, prefab, parent, isForceCreateNew).TryGetComponent<T>();;
            }

        }
    }
}
