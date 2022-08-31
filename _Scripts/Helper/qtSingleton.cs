using _Scripts.Helper;
using UnityEngine;

public abstract class qtSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    public static T Instance {
        get
        {
            if (_instance != null)
            {
                return _instance;
            }

            qtLogging.LogWarning($"{typeof(T).Name} instance is NULL, going to create!");
            var gameObject = new GameObject
            {
                name = typeof(T).Name
            };
            _instance = gameObject.AddComponent<T>();

            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogError($"Destroy {name} instance in {gameObject.name}");
            Destroy(this);
        }
        else
        {
            _instance = this as T;
        }
        Init();
    }
    
    protected virtual void Init(){}
}