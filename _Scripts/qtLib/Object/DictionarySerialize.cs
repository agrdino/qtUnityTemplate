using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DictionarySerialize<K,V>{
    [SerializeField]
    private List<K> keys;
    [SerializeField]
    private List<V> values;

    public DictionarySerialize(){
        keys = new List<K>();
        values = new List<V>();
    }

    public DictionarySerialize<K,V> put(K key, V value){
        try {
            if (!keys.Contains(key)){
                keys.Add(key);
                values.Add(value);
            }
            else{
                throw new Exception("Key is exist!");
            }
        }
        catch (Exception e){
            Debug.LogError(e.Message);
        }
        return this;
    }

    public int Count => keys.Count;

    public K GetKey(int index){
        return keys[index];
    }

    public V GetValue(K key)
    {
        if (keys.Contains(key)){
            var tempIndex = keys.IndexOf(key);
            return values[tempIndex];
        }

        return default;
    }

    public V GetValue(int index){
        return values[index];
    }

    public bool Contains(K key){
        return keys.Contains(key);
    }

    public bool Remove(K key){
        if (Count <= 0){
            return false;
        }
        var isContain = false;
        if (keys.Contains(key)){
            isContain = true;
            var tempIndex = keys.IndexOf(key);
            keys.RemoveAt(tempIndex);
            values.RemoveAt(tempIndex);
        }
        return isContain;
    }

    public bool Remove(int index){
        if (Count <= 0){
            return false;
        }
        else{
            keys.RemoveAt(index);
            values.RemoveAt(index);
            return true;
        }
    }

    public void Clear(){
        keys.Clear();
        values.Clear();
    }

    public List<K> Keys(){
        return keys;
    }
    
    public List<V> Values(){
        return values;
    }
}