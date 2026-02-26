using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemCollection", menuName = "Custom/ItemCollection")]
public class ItemCollection : ScriptableObject
{
    [SerializeField] private List<ItemModel> itemModels = new List<ItemModel>();
    private static Dictionary<int, ItemModel> itemModelsDic = new Dictionary<int, ItemModel>();

    [NonSerialized]
    private bool isLoaded = false;

    private void Load()
    {
        foreach (var item in itemModels)
        {
            itemModelsDic[item.Id] = item;
        }
        isLoaded = true;
    }

    public ItemModel GetItemModel(int id)
    {
        if (!isLoaded)
            Load();
        return itemModelsDic[id];
    }
}

[Serializable]
public class ItemModel
{
    public int Id;
    public Sprite Sprite;
}
