using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressData : ISaveData
{
    public int ActiveLevel;
    public int PlayerLevel;
    public int XP;
    
    public string GetSaveable()
    {
        return ActiveLevel  +  "_" + PlayerLevel + "_" + XP;
    }

    public void LoadData(string saved)
    {
        var data = saved.Split('_');
        if (data.Length < 2)
        {
            ActiveLevel = 1;
            PlayerLevel = 0;
            XP = 0;
            return;
        }
        ActiveLevel = int.Parse(data[0]);
        PlayerLevel = int.Parse(data[1]);
        XP = int.Parse(data[2]);
    }

    public void Update<T>(T input) where T : ISaveData
    {
        var classInput = input as ProgressData;
        PlayerLevel = classInput.PlayerLevel;
        XP = classInput.XP;
        ActiveLevel = classInput.ActiveLevel;
    }
}
