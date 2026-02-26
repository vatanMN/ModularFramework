using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

public class SaveLoadService : IService
{
    public static SaveLoadService Instance => SystemLocator.Instance.SaveLoadService;
    
    private int savePeriod = 5000;
    private const string SAVE_FILE_PATH = "Save_";
    
    public bool IsReady { get; private set; }
    
    private Dictionary<DataKey, ISaveData> data = new Dictionary<DataKey, ISaveData>()
    {
        { DataKey.Progress, new ProgressData() },
        { DataKey.Unlock , new UnlockSaveData()},
        { DataKey.Inventory, new InventoryData() },
        { DataKey.Currency, new CurrencyData() },
        { DataKey.TowerUpgrades, new TowerUpgradeData() },
        { DataKey.Settings, new SettingsData() },
    };
    private List<DataKey> dirtyKeys = new List<DataKey>();
    
    public async Task Initialize()
    {
        LoadData();
        dirtyKeys.Capacity =data.Count;
        PeriodicSave();
        IsReady = true;
    }

    public T GetData<T>(DataKey key) where T : ISaveData
    {
        if(data.ContainsKey(key)) return (T)data[key];
        else return default(T);
    }

    public void Save<T>(DataKey key, T newData, bool isImmadiate = false) where T : ISaveData
    {
        data[key].Update(newData);
        if(!dirtyKeys.Contains(key)) dirtyKeys.Add(key);
        if (isImmadiate) SaveData();
    }

    private async void PeriodicSave()
    {
        while (true)
        {
            await Task.Delay(savePeriod);
            SaveData();
        }
    }

    private void LoadData()
    {
        foreach (var dataTupple in data)
        {
            dataTupple.Value.LoadData(PlayerPrefs.GetString(SAVE_FILE_PATH + dataTupple.Key.ToString(),string.Empty));
        }
    }

    private void SaveData()
    {
        foreach (var key in dirtyKeys)
        {
            PlayerPrefs.SetString(SAVE_FILE_PATH + key.ToString(), data[key].GetSaveable());
        }
        PlayerPrefs.Save();
        dirtyKeys.Clear();
    }
}

public interface ISaveData
{
    public string GetSaveable();
    public void LoadData(string saved);
    public void Update <T>(T input) where T : ISaveData;
}


