using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockSaveData : ISaveData
{
    public List<string> UnlockedDrops;
    public string GetSaveable()
    {
        string saveable = "";
        
        foreach (var dId in UnlockedDrops)
        {
            saveable += dId;
            if(dId != UnlockedDrops[^1]) saveable += "/";
        }
        return saveable;
    }

    public void LoadData(string saved)
    {
        var data = saved.Split('/');
        UnlockedDrops = new List<string>();
        foreach (var dId in data)
        {
            if(string.IsNullOrEmpty(dId)) continue;
            UnlockedDrops.Add(dId);
        }
    }

    public void Update<T>(T input) where T : ISaveData
    {
        var classInput = input as UnlockSaveData;
        var newList = new List<string>(classInput.UnlockedDrops);
        foreach (var dId in newList)
        {
            if(!UnlockedDrops.Contains(dId))
                UnlockedDrops.Add(dId);
        }
    }
}
