using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using ModularFW.Core.Locator;

namespace ModularFW.Core.SaveSystem {
public class SaveLoadService : IService, ModularFW.Core.ISaveLoadService
{
    public static SaveLoadService Instance => SystemLocator.Instance.SaveLoadService;
    
    private int savePeriod = 5000;
    private const string SAVE_FILE_PATH = "Save_";
    
    public bool IsReady { get; private set; }
    private CancellationTokenSource _saveCts;

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
        dirtyKeys.Capacity = data.Count;
        _saveCts = new CancellationTokenSource();
        _ = PeriodicSave(_saveCts.Token);
        await Task.Delay(1);
        IsReady = true;
    }

    public void Shutdown()
    {
        _saveCts?.Cancel();
        _saveCts?.Dispose();
        _saveCts = null;
        SaveData();
    }

    public T GetData<T>(DataKey key) where T : ISaveData
    {
        if(data.ContainsKey(key)) return (T)data[key];
        else return default(T);
    }

    public void Save<T>(DataKey key, T newData, bool immediate = false) where T : ISaveData
    {
        data[key].Update(newData);
        if(!dirtyKeys.Contains(key)) dirtyKeys.Add(key);
        if (immediate) SaveData();
    }

    private async Task PeriodicSave(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(savePeriod, cancellationToken);
                SaveData();
            }
        }
        catch (OperationCanceledException) { }
        catch (Exception e)
        {
            Debug.LogError($"[SaveLoad] PeriodicSave error: {e.Message}");
        }
    }

    private void LoadData()
    {
        foreach (var dataEntry in data)
        {
            dataEntry.Value.LoadData(PlayerPrefs.GetString(SAVE_FILE_PATH + dataEntry.Key.ToString(), string.Empty));
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
    public void Update<T>(T input) where T : ISaveData;
}
}
