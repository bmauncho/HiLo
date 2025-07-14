using System.Collections.Generic;
using UnityEngine;

public enum PoolType
{
    Cards,
    CardHistory,
}

[System.Serializable]
public class PoolConfig
{
    public PoolType objType;
    public GameObject prefab;
    public Transform parent;
    public int initialSize = 10;
    public int maxCapacity = 20;
}

public class PoolManager : MonoBehaviour
{
    [SerializeField] private PoolConfig [] poolConfigs;

    private Dictionary<PoolType , Queue<GameObject>> pools = new();
    private Dictionary<PoolType , PoolConfig> configLookup = new();
    private Dictionary<PoolType , int> totalCountLookup = new();

    private void Start ()
    {
        foreach (var config in poolConfigs)
        {
            InitializePool(config);
        }
    }

    private void InitializePool ( PoolConfig config )
    {
        Queue<GameObject> queue = new();
        for (int i = 0 ; i < config.initialSize ; i++)
        {
            GameObject obj = Instantiate(config.prefab , config.parent);
            obj.name = $"{config.objType}_{i}";
            obj.SetActive(false);
            queue.Enqueue(obj);
        }

        pools [config.objType] = queue;
        configLookup [config.objType] = config;
        totalCountLookup [config.objType] = config.initialSize;
    }

    public GameObject GetFromPool ( PoolType key , Vector3 position , Quaternion rotation , Transform parent = null )
    {
        if (!pools.ContainsKey(key))
        {
            Debug.LogWarning($"No pool with key '{key}' exists.");
            return null;
        }

        Queue<GameObject> pool = pools [key];
        PoolConfig config = configLookup [key];

        if (pool.Count == 0)
        {
            if (totalCountLookup [key] < config.maxCapacity)
            {
                ExpandPool(config , key);
            }
            else
            {
                Debug.LogWarning($"Pool for '{key}' reached max capacity.");
                return null;
            }
        }

        GameObject obj = pool.Dequeue();
        obj.transform.SetPositionAndRotation(position , rotation);
        obj.transform.localScale = Vector3.one;
        obj.SetActive(true);
        obj.transform.SetParent(parent ?? config.parent , worldPositionStays: true);
        return obj;
    }

    public void ReturnToPool ( PoolType key , GameObject obj )
    {
        if (!pools.ContainsKey(key))
        {
            Debug.LogWarning($"Trying to return to unknown pool '{key}'. Destroying object.");
            Destroy(obj);
            return;
        }

        obj.SetActive(false);
        obj.transform.SetParent(configLookup [key].parent , worldPositionStays: false);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localScale = Vector3.one;
        pools [key].Enqueue(obj);
    }

    private void ExpandPool ( PoolConfig config , PoolType key )
    {
        Queue<GameObject> pool = pools [key];
        int currentCount = totalCountLookup [key];
        int toAdd = Mathf.Min(config.initialSize , config.maxCapacity - currentCount);

        for (int i = 0 ; i < toAdd ; i++)
        {
            GameObject obj = Instantiate(config.prefab , config.parent);
            obj.name = $"{key}_{currentCount + i}";
            obj.SetActive(false);
            pool.Enqueue(obj);
        }

        totalCountLookup [key] += toAdd;
    }

    private Transform GetParent ( PoolType pooltype )
    {
        if (configLookup.TryGetValue(pooltype , out var config))
            return config.parent;

        return transform;
    }
}
