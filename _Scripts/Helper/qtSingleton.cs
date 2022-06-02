using UnityEngine;

public abstract class qtSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    public static T Instance => _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
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