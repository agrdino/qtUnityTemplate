using System.Collections;
using System.Collections.Generic;
using _Scripts.qtLib.Extension;
using UnityEngine;

public class qtPooling : qtSingleton<qtPooling>
{
    private Dictionary<string, qtObjectPool> _pools;
    private Dictionary<string, qtScriptPool> _poolsComponent;
    
    protected override void Init()
    {
        base.Init();
        DontDestroyOnLoad(this);
    }

    public GameObject Spawn(string name, GameObject prefab, bool isForceCreateNew = false)
    {
        _pools ??= new Dictionary<string, qtObjectPool>();
        if (!_pools.ContainsKey(name))
        {
            _pools.Add(name, new qtObjectPool());
        }

        var pool = _pools[name];
        return pool.Spawn(prefab, isForceCreateNew);
    }
    
    public GameObject Spawn(string name, GameObject prefab, Transform parent, bool isForceCreateNew = false)
    {
        _pools ??= new Dictionary<string, qtObjectPool>();
        if (!_pools.ContainsKey(name))
        {
            _pools.Add(name, new qtObjectPool());
        }

        var pool = _pools[name];
        return pool.Spawn(prefab, parent, isForceCreateNew);
    }
    
    public GameObject Spawn(string name, GameObject prefab, Vector3 position, Quaternion rotation, Transform parent, bool isForceCreateNew = false)
    {
        _pools ??= new Dictionary<string, qtObjectPool>();
        if (!_pools.ContainsKey(name))
        {
            _pools.Add(name, new qtObjectPool());
        }

        var pool = _pools[name];
        return pool.Spawn(prefab, position, rotation, parent, isForceCreateNew);
    }
    
    public T Spawn<T>(string name, GameObject prefab, bool isForceCreateNew = false) where T : qtPoolingObject
    {
        _poolsComponent ??= new Dictionary<string, qtScriptPool>();
        if (!_poolsComponent.ContainsKey(name))
        {
            _poolsComponent.Add(name, new qtScriptPool());
        }

        var pool = _poolsComponent[name];
        return pool.Spawn<T>(prefab, isForceCreateNew);
    }
    
    public T Spawn<T>(string name, GameObject prefab, Transform parent, bool isForceCreateNew = false) where T : qtPoolingObject
    {
        _poolsComponent ??= new Dictionary<string, qtScriptPool>();
        if (!_poolsComponent.ContainsKey(name))
        {
            _poolsComponent.Add(name, new qtScriptPool());
        }

        var pool = _poolsComponent[name];
        return pool.Spawn<T>(prefab, parent, isForceCreateNew);
    }
    
    public T Spawn<T>(string name, GameObject prefab, Vector3 position, Quaternion rotation, Transform parent, bool isForceCreateNew = false) where T : qtPoolingObject
    {
        _poolsComponent ??= new Dictionary<string, qtScriptPool>();
        if (!_poolsComponent.ContainsKey(name))
        {
            _poolsComponent.Add(name, new qtScriptPool());
        }

        var pool = _poolsComponent[name];
        return pool.Spawn<T>(prefab, position, rotation, parent, isForceCreateNew).GetComponent<T>();
    }
}

public class qtObjectPool
{
    private List<GameObject> _pool;

    public GameObject Spawn(GameObject prefab, bool isForceCreateNew)
    {
        _pool ??= new List<GameObject>();

        if (isForceCreateNew)
        {
            var forceNewItem = Object.Instantiate(prefab);
            _pool.Add(forceNewItem);
            return forceNewItem;
        }
        
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
    
    public GameObject Spawn(GameObject prefab, Transform parent, bool isForceCreateNew)
    {
        _pool ??= new List<GameObject>();
        
        if (isForceCreateNew)
        {
            var forceNewItem = Object.Instantiate(prefab, parent);
            _pool.Add(forceNewItem);
            return forceNewItem;
        }
        
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
    
    public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent, bool isForceCreateNew)
    {
        _pool ??= new List<GameObject>();
        
        if (isForceCreateNew)
        {
            var forceNewItem = Object.Instantiate(prefab, position, rotation, parent);
            _pool.Add(forceNewItem);
            return forceNewItem;
        }

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

public class qtScriptPool
{
    private List<qtPoolingObject> _pool;
    public T Spawn<T>(GameObject prefab, bool isForceCreateNew) where T : qtPoolingObject
    {
        _pool ??= new List<qtPoolingObject>();

        if (isForceCreateNew)
        {
            var forceNewItem = Object.Instantiate(prefab).transform.TryGetComponent<T>();
            _pool.Add(forceNewItem);
            return forceNewItem;
        }
        
        foreach (var item in _pool)
        {
            if (!item.gameObject.activeSelf)
            {
                item.gameObject.SetActive(true);
                item.FactoryReset();
                return (T)item;
            }
        }

        var newItem = Object.Instantiate(prefab).transform.TryGetComponent<T>();
        _pool.Add(newItem);
        return newItem;
    }  
    
    public T Spawn<T>(GameObject prefab, Transform parent, bool isForceCreateNew) where T : qtPoolingObject
    {
        _pool ??= new List<qtPoolingObject>();
        
        if (isForceCreateNew)
        {
            var forceNewItem = Object.Instantiate(prefab, parent).transform.TryGetComponent<T>();
            _pool.Add(forceNewItem);
            return forceNewItem;
        }
        
        foreach (var item in _pool)
        {
            if (!item.gameObject.activeSelf)
            {
                item.gameObject.SetActive(true);
                item.transform.SetParent(parent);
                item.transform.localScale = Vector3.one;
                item.FactoryReset();
                return (T)item;
            }
        }

        var newItem = Object.Instantiate(prefab, parent).transform.TryGetComponent<T>();
        _pool.Add(newItem);
        return newItem;
    }
    
    public T Spawn<T>(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent, bool isForceCreateNew) where T : qtPoolingObject
    {
        _pool ??= new List<qtPoolingObject>();
        
        if (isForceCreateNew)
        {
            var forceNewItem = Object.Instantiate(prefab, position, rotation, parent).transform.TryGetComponent<T>();
            _pool.Add(forceNewItem);
            return forceNewItem;
        }

        foreach (var item in _pool)
        {
            if (!item.gameObject.activeSelf)
            {
                item.gameObject.SetActive(true);
                item.transform.SetParent(parent);
                item.transform.position = position;
                item.transform.rotation = rotation;
                item.transform.localScale = Vector3.one;
                item.FactoryReset();
                return (T)item;
            }
        }

        var newItem = Object.Instantiate(prefab, position, rotation, parent).transform.TryGetComponent<T>();
        _pool.Add(newItem);
        return newItem;
    }
}

[DisallowMultipleComponent]
public abstract class qtPoolingObject : MonoBehaviour
{
    public virtual void FactoryReset(){}
}