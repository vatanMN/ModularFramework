using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class PoolingService : IService
{
    
    public static PoolingService Instance => SystemLocator.Instance.PoolingService;
    PoolCollection PoolCollection;

    Dictionary<PoolEnum, Queue<GameObject>> WaitingObjects = new Dictionary<PoolEnum, Queue<GameObject>>();

    public bool IsReady { get; private set; }

    public async Task Initialize(PoolCollection poolCollection)
    {
        PoolCollection = poolCollection;
        IsReady = true;
    }

    public T Create<T>(PoolEnum poolEnum, Transform parent) where T : MonoBehaviour
    {
        if (!WaitingObjects.ContainsKey(poolEnum))
        {
            WaitingObjects.Add(poolEnum, new Queue<GameObject>());
        }
        if (WaitingObjects[poolEnum].Count < 1)
        {
            var obj = GameObject.Instantiate(PoolCollection.GetGameObject(poolEnum));
            obj.SetActive(false);
            WaitingObjects[poolEnum].Enqueue(obj);

        }

        var res = WaitingObjects[poolEnum].Dequeue();
        if(res == null || res.gameObject == null)
        {
            return Create<T>( poolEnum,  parent);
        }
        res.SetActive(true);
        res.transform.SetParent(parent);
        res.transform.localScale = Vector3.one;

        var res2 = res.GetComponent<T>();
        return res2;
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
