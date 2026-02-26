using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AudioService: IService{

    private bool audioEnabled = true;
    public bool AudioEnabled => audioEnabled;

    public void SetAudioEnabled(bool enabled)
    {
        audioEnabled = enabled;
        SaveSettings();
    }

    private void LoadSettings()
    {
        var settings = SaveLoadService.Instance.GetData<SettingsData>(DataKey.Settings);
        if (settings != null) audioEnabled = settings.AudioEnabled;
    }

    private void SaveSettings()
    {
        var settings = SaveLoadService.Instance.GetData<SettingsData>(DataKey.Settings) ?? new SettingsData();
        settings.AudioEnabled = audioEnabled;
        SaveLoadService.Instance.Save(DataKey.Settings, settings, true);
    }

    
    public static AudioService Instance => SystemLocator.Instance.AudioService;
    
    public bool IsReady { get; private set; }
    public AudioClip CollectedSound;
    private AudioSourceBlock AudioSourceBlock;

    private AudioCollection AudioCollection;

    public async Task Initialize(AudioCollection audioCollection)
    {
        AudioCollection = audioCollection;
        while (AudioSourceBlock == null)
        {
            await Task.Delay(1);
            AudioSourceBlock = GameObject.FindAnyObjectByType<AudioSourceBlock>();
        }
        LoadSettings();
        IsReady = true;
    }

    public void Play(AudioEnum audioType)
    {
        if (!audioEnabled) return;
        if (AudioSourceBlock == null)
        {
            Debug.Log("AudioSourceBlock is null");
        }
        if(AudioCollection.GetAudioElement(audioType) == null)
        {
            Debug.Log("audioType is null");
        }
        AudioSourceBlock.PlayAudio(AudioCollection.GetAudioElement(audioType).Clip);
    }

}
