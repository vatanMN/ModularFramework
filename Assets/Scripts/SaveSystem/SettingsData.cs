using System;
using UnityEngine;

namespace ModularFW.Core.SaveSystem
{
    [Serializable]
    public class SettingsData : ISaveData
    {
        public bool AudioEnabled = true;
        public bool HapticEnabled = true;
        public float MasterVolume = 1f;
        public float SfxVolume = 1f;
        public float MusicVolume = 1f;

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
                    MasterVolume = loaded.MasterVolume;
                    SfxVolume = loaded.SfxVolume;
                    MusicVolume = loaded.MusicVolume;
                }
            }
        }

        public void Update<T>(T input) where T : ISaveData
        {
            if (input is SettingsData settingsData)
            {
                AudioEnabled = settingsData.AudioEnabled;
                HapticEnabled = settingsData.HapticEnabled;
                MasterVolume = settingsData.MasterVolume;
                SfxVolume = settingsData.SfxVolume;
                MusicVolume = settingsData.MusicVolume;
            }
        }
    }
}
