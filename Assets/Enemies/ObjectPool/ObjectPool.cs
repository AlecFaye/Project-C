using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    private readonly PoolableObject prefab;
    private readonly int size;
    private readonly List<PoolableObject> availableObjectsPool;

    private ObjectPool(PoolableObject prefab, int size)
    {
        this.prefab = prefab;
        this.size = size;
        availableObjectsPool = new List<PoolableObject>(size);
    }

    public static ObjectPool CreateInstance(PoolableObject Prefab, int Size)
    {
        ObjectPool pool = new(Prefab, Size);

        GameObject poolGameObject = new(Prefab + " Pool");
        pool.CreateObjects(poolGameObject);

        return pool;
    }

    private void CreateObjects(GameObject parent)
    {
        for (int i = 0; i < size; i++)
        {
            PoolableObject poolableObject = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity, parent.transform);
            poolableObject.Parent = this;
            poolableObject.gameObject.SetActive(false); 
        }
    }

    public PoolableObject GetObject()
    {
        if (availableObjectsPool.Count <= 0) 
            return null;

        PoolableObject instance = availableObjectsPool[0];

        availableObjectsPool.RemoveAt(0);

        instance.gameObject.SetActive(true);

        return instance;
    }

    public void ReturnObjectToPool(PoolableObject Object)
    {
        availableObjectsPool.Add(Object);
    }
}
