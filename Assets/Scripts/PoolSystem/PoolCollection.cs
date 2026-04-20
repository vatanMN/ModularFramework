using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ModularFW.Core.PoolSystem {

[CreateAssetMenu(fileName = "PoolCollection", menuName = "Custom/PoolCollection")]
public class PoolCollection : ScriptableObject
{
    [SerializeField] List<PoolObject> objects = new List<PoolObject>();
    private static Dictionary<PoolEnum, GameObject> objectsDic = new Dictionary<PoolEnum, GameObject>();

    [NonSerialized]
    private bool isLoaded = false;

    private void Load()
    {
        foreach (var item in objects)
        {
            objectsDic[item.PoolEnum] = item.GameObject;
        }
        isLoaded = true;
    }

    public GameObject GetGameObject(PoolEnum poolEnum)
    {
        if (!isLoaded) Load();
        return objectsDic[poolEnum];
    }

    public IReadOnlyList<PoolObject> GetAllObjects() => objects;

}

[Serializable]
public class PoolObject
{
    public PoolEnum PoolEnum;
    public GameObject GameObject;
    [Min(0)] public int PrewarmCount = 0;
}

}

