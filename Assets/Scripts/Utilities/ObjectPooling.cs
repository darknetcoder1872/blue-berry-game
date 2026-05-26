using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Object pooling system for optimized performance with bullets, zombies, and effects.
/// </summary>
public class ObjectPool : MonoBehaviour
{
    [System.Serializable]
    private class PooledObject
    {
        public GameObject prefab;
        public int initialCount = 10;
        public List<GameObject> available = new List<GameObject>();
        public List<GameObject> inUse = new List<GameObject>();
    }

    private static ObjectPool instance;
    private Dictionary<string, PooledObject> pools = new Dictionary<string, PooledObject>();

    public static ObjectPool Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ObjectPool>();
                if (instance == null)
                {
                    GameObject go = new GameObject("ObjectPool");
                    instance = go.AddComponent<ObjectPool>();
                }
            }
            return instance;
        }
    }

    /// <summary>
    /// Register a prefab for pooling
    /// </summary>
    public void RegisterPool(GameObject prefab, int initialCount = 10)
    {
        string key = prefab.name;
        if (pools.ContainsKey(key)) return;

        PooledObject pool = new PooledObject
        {
            prefab = prefab,
            initialCount = initialCount
        };

        for (int i = 0; i < initialCount; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pool.available.Add(obj);
        }

        pools[key] = pool;
    }

    /// <summary>
    /// Get an object from the pool
    /// </summary>
    public GameObject GetObject(GameObject prefab)
    {
        string key = prefab.name;
        if (!pools.ContainsKey(key))
            RegisterPool(prefab, 10);

        PooledObject pool = pools[key];

        GameObject obj;
        if (pool.available.Count > 0)
        {
            obj = pool.available[0];
            pool.available.RemoveAt(0);
        }
        else
        {
            obj = Instantiate(prefab);
        }

        obj.SetActive(true);
        pool.inUse.Add(obj);
        return obj;
    }

    /// <summary>
    /// Return an object to the pool
    /// </summary>
    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        
        // Find which pool this belongs to
        foreach (var pool in pools.Values)
        {
            if (pool.inUse.Contains(obj))
            {
                pool.inUse.Remove(obj);
                pool.available.Add(obj);
                break;
            }
        }
    }

    /// <summary>
    /// Clear all pools
    /// </summary>
    public void ClearAllPools()
    {
        foreach (var pool in pools.Values)
        {
            foreach (var obj in pool.available)
                Destroy(obj);
            foreach (var obj in pool.inUse)
                Destroy(obj);
        }
        pools.Clear();
    }
}