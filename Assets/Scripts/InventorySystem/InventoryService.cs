using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class InventoryService : IService
{

    public static InventoryService Instance => SystemLocator.Instance.InventoryService;
    public bool IsReady { get; private set; }
    ItemCollection ItemCollection;
    private Dictionary<int, int> OwnItems = new Dictionary<int, int>();

    public async Task Initialize(ItemCollection itemCollection)
    {
        ItemCollection = itemCollection;
        OwnItems = SaveLoadService.Instance.GetData<InventoryData>(DataKey.Inventory).OwnItems;
        IsReady = true;
    }
    
    public ItemModel GetItemModel(int id)
    {
        return ItemCollection.GetItemModel(id);
    }

    public bool isOwned(int id)
    {
        return OwnItems.ContainsKey(id) && OwnItems[id]>0;
    }

    public void GainItem(int id, int count)
    {
        if (!OwnItems.ContainsKey(id))
        {
            OwnItems[id] = 0;
        }
        OwnItems[id] += count;
        Save();
    }

    public Dictionary<int, int> GetOwnItems()
    {
        return OwnItems;
    }

    public int GetOwnItemCount(int id)
    {
        return OwnItems[id];
    }

    public void ResetOwnedItems()
    {
        OwnItems.Clear();
        Save();
    }

    private void Save()
    {
        SaveLoadService.Instance.Save(DataKey.Inventory, new InventoryData()
        {
            OwnItems = OwnItems
        });
    }
}
