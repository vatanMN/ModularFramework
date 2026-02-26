using System;
using UnityEngine;

[Serializable]
public class SettingsData : ISaveData
{
    public bool AudioEnabled = true;
    public bool HapticEnabled = true;

    public SettingsData() {}

    public string GetSaveable()
    {
        return JsonUtility.ToJson(this);
    }

    public void LoadData(string saved)
    {
        if (!string.IsNullOrEmpty(saved))
        {
            var loaded = JsonUtility.FromJson<SettingsData>(saved);
            if (loaded != null)
            {
                AudioEnabled = loaded.AudioEnabled;
                HapticEnabled = loaded.HapticEnabled;
            }
        }
    }

    public void Update<T>(T input) where T : ISaveData
    {
        if (input is SettingsData o)
        {
            AudioEnabled = o.AudioEnabled;
            HapticEnabled = o.HapticEnabled;
        }
    }
}
