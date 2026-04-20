using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using ModularFW.Core.Locator;

namespace ModularFW.Core.PoolSystem {
public class PoolingService : IService, ModularFW.Core.IPoolingService
{
    
    public static PoolingService Instance => SystemLocator.Instance.PoolingService;
    PoolCollection PoolCollection;

    Dictionary<PoolEnum, Queue<GameObject>> WaitingObjects = new Dictionary<PoolEnum, Queue<GameObject>>();

    public bool IsReady { get; private set; }

    public async Task Initialize(PoolCollection poolCollection)
    {
        PoolCollection = poolCollection;
        await Task.Delay(1);
        IsReady = true;
    }

    public T Create<T>(PoolEnum poolEnum, Transform parent) where T : MonoBehaviour
        => CreateInternal<T>(poolEnum, parent, 0);

    private T CreateInternal<T>(PoolEnum poolEnum, Transform parent, int attempt) where T : MonoBehaviour
    {
        const int maxAttempts = 3;
        if (!WaitingObjects.ContainsKey(poolEnum))
            WaitingObjects.Add(poolEnum, new Queue<GameObject>());

        if (WaitingObjects[poolEnum].Count < 1)
        {
            var obj = GameObject.Instantiate(PoolCollection.GetGameObject(poolEnum));
            obj.SetActive(false);
            WaitingObjects[poolEnum].Enqueue(obj);
        }

        var pooledObject = WaitingObjects[poolEnum].Dequeue();
        if (pooledObject == null || pooledObject.gameObject == null)
        {
            if (attempt >= maxAttempts)
            {
                Debug.LogError($"[PoolingService] Failed to get valid object for {poolEnum} after {maxAttempts} attempts.");
                return null;
            }
            return CreateInternal<T>(poolEnum, parent, attempt + 1);
        }
        pooledObject.SetActive(true);
        pooledObject.transform.SetParent(parent);
        pooledObject.transform.localScale = Vector3.one;

        var component = pooledObject.GetComponent<T>();
        if (component == null)
            Debug.LogError($"[PoolingService] Object for pool {poolEnum} is missing component {typeof(T).Name}.");
        return component;
    }

    public void Destroy(PoolEnum poolEnum, GameObject gameObject)
    {
        if (!WaitingObjects.ContainsKey(poolEnum))
        {
            WaitingObjects.Add(poolEnum, new Queue<GameObject>());
        }

        gameObject.SetActive(false);
        WaitingObjects[poolEnum].Enqueue(gameObject);
    }
}
}
