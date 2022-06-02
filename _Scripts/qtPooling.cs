using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class qtPooling : qtSingleton<qtPooling>
{
    private Dictionary<string, ObjectPool> _pools;
    public GameObject Spawn(string name, GameObject prefab)
    {
        _pools ??= new Dictionary<string, ObjectPool>();
        if (!_pools.ContainsKey(name))
        {
            _pools.Add(name, new ObjectPool());
        }

        var pool = _pools[name];
        return pool.Spawn(prefab);
    }
    
    public GameObject Spawn(string name, GameObject prefab, Transform parent)
    {
        _pools ??= new Dictionary<string, ObjectPool>();
        if (!_pools.ContainsKey(name))
        {
            _pools.Add(name, new ObjectPool());
        }

        var pool = _pools[name];
        return pool.Spawn(prefab, parent);
    }
    
    public GameObject Spawn(string name, GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
    {
        _pools ??= new Dictionary<string, ObjectPool>();
        if (!_pools.ContainsKey(name))
        {
            _pools.Add(name, new ObjectPool());
        }

        var pool = _pools[name];
        return pool.Spawn(prefab, position, rotation, parent);
    }
    
    public T Spawn<T>(string name, GameObject prefab) where T : Component
    {
        _pools ??= new Dictionary<string, ObjectPool>();
        if (!_pools.ContainsKey(name))
        {
            _pools.Add(name, new ObjectPool());
        }

        var pool = _pools[name];
        return pool.Spawn(prefab).GetComponent<T>();
    }
    
    public T Spawn<T>(string name, GameObject prefab, Transform parent) where T : Component
    {
        _pools ??= new Dictionary<string, ObjectPool>();
        if (!_pools.ContainsKey(name))
        {
            _pools.Add(name, new ObjectPool());
        }

        var pool = _pools[name];
        return pool.Spawn(prefab, parent).GetComponent<T>();
    }
    
    public T Spawn<T>(string name, GameObject prefab, Vector3 position, Quaternion rotation, Transform parent) where T : Component
    {
        _pools ??= new Dictionary<string, ObjectPool>();
        if (!_pools.ContainsKey(name))
        {
            _pools.Add(name, new ObjectPool());
        }

        var pool = _pools[name];
        return pool.Spawn(prefab, position, rotation, parent).GetComponent<T>();
    }
}

public class ObjectPool
{
    private List<GameObject> _pool;

    public GameObject Spawn(GameObject prefab)
    {
        _pool ??= new List<GameObject>();
        foreach (var item in _pool)
        {
            if (!item.activeSelf)
            {
                item.SetActive(true);
                return item;
            }
        }

        var newItem = Object.Instantiate(prefab);
        _pool.Add(newItem);
        return newItem;
    }  
    
    public GameObject Spawn(GameObject prefab, Transform parent)
    {
        _pool ??= new List<GameObject>();
        foreach (var item in _pool)
        {
            if (!item.activeSelf)
            {
                item.SetActive(true);
                item.transform.SetParent(parent);
                item.transform.localScale = Vector3.one;
                return item;
            }
        }

        var newItem = Object.Instantiate(prefab, parent);
        _pool.Add(newItem);
        return newItem;
    }
    
    public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
    {
        _pool ??= new List<GameObject>();
        foreach (var item in _pool)
        {
            if (!item.activeSelf)
            {
                item.SetActive(true);
                item.transform.SetParent(parent);
                item.transform.position = position;
                item.transform.rotation = rotation;
                item.transform.localScale = Vector3.one;
                return item;
            }
        }

        var newItem = Object.Instantiate(prefab, position, rotation, parent);
        _pool.Add(newItem);
        return newItem;
    }
}