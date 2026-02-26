using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryData : ISaveData
{
    public Dictionary<int, int> OwnItems = new Dictionary<int, int>();
    public string GetSaveable()
    {
        var res = "";
        foreach (var item in OwnItems)
        {
            res += item.Key + ":" + item.Value + ";";
        }
        return res;
    }

    public void LoadData(string saved)
    {
        string[] strAr = saved.Split(";");
        foreach (var item in strAr)
        {
            if(item.Length > 0)
            {
                string[] itemStr = item.Split(":");
                OwnItems[int.Parse(itemStr[0])] = int.Parse(itemStr[1]);
            }
        }
    }

    public void Update<T>(T input) where T : ISaveData
    {
        var classInput = input as InventoryData;
        var newList = new Dictionary<int, int> (classInput.OwnItems);
        foreach (var dId in newList)
        {
            OwnItems[dId.Key] = dId.Value;
        }
    }
}
