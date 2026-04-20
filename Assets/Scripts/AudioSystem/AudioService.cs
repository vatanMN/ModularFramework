using ModularFW.Core.SaveSystem;
using System.Threading.Tasks;
using UnityEngine;
using ModularFW.Core.Locator;

namespace ModularFW.Core.AudioSystem
{
    public class AudioService : IService, ModularFW.Core.IAudioService
    {
        public static AudioService Instance => SystemLocator.Instance.AudioService;
        public bool IsReady { get; private set; }

        private bool audioEnabled = true;
        private float masterVolume = 1f;
        private float sfxVolume = 1f;
        private float musicVolume = 1f;

        public bool AudioEnabled => audioEnabled;
        public float MasterVolume => masterVolume;
        public float SfxVolume => sfxVolume;
        public float MusicVolume => musicVolume;

        private AudioSourceBlock AudioSourceBlock;
        private AudioCollection AudioCollection;

        public async Task Initialize(AudioCollection audioCollection, AudioSourceBlock audioSourceBlock)
        {
            AudioCollection = audioCollection;
            AudioSourceBlock = audioSourceBlock;
            if (AudioSourceBlock == null)
                Debug.LogError("[AudioService] AudioSourceBlock not assigned in SystemLocator inspector.");
            LoadSettings();
            ApplyVolumes();
            IsReady = true;
            await System.Threading.Tasks.Task.CompletedTask;
        }

        public void SetAudioEnabled(bool enabled)
        {
            audioEnabled = enabled;
            SaveSettings();
        }

        public void SetMasterVolume(float volume)
        {
            masterVolume = Mathf.Clamp01(volume);
            ApplyVolumes();
            SaveSettings();
        }

        public void SetSfxVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            ApplyVolumes();
            SaveSettings();
        }

        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            ApplyVolumes();
            SaveSettings();
        }

        public void Play(AudioEnum audioType)
        {
            if (!audioEnabled) return;
            if (AudioSourceBlock == null) { Debug.LogWarning("[AudioService] AudioSourceBlock is null."); return; }
            var element = AudioCollection.GetAudioElement(audioType);
            if (element == null || element.Clip == null) { Debug.LogWarning($"[AudioService] No clip found for {audioType}."); return; }

            float vol = masterVolume * (element.Category == AudioCategory.Music ? musicVolume : sfxVolume);
            if (element.Category == AudioCategory.Music)
                AudioSourceBlock.PlayMusic(element.Clip, vol, element.Priority);
            else
                AudioSourceBlock.PlaySfx(element.Clip, vol, element.Priority);
        }

        private void ApplyVolumes()
        {
            if (AudioSourceBlock == null) return;
            AudioSourceBlock.SetSfxVolume(masterVolume * sfxVolume);
            AudioSourceBlock.SetMusicVolume(masterVolume * musicVolume);
        }

        private void LoadSettings()
        {
            var settings = SaveLoadService.Instance.GetData<SettingsData>(DataKey.Settings);
            if (settings == null) return;
            audioEnabled = settings.AudioEnabled;
            masterVolume = settings.MasterVolume;
            sfxVolume = settings.SfxVolume;
            musicVolume = settings.MusicVolume;
        }

        private void SaveSettings()
        {
            var settings = SaveLoadService.Instance.GetData<SettingsData>(DataKey.Settings) ?? new SettingsData();
            settings.AudioEnabled = audioEnabled;
            settings.MasterVolume = masterVolume;
            settings.SfxVolume = sfxVolume;
            settings.MusicVolume = musicVolume;
            SaveLoadService.Instance.Save(DataKey.Settings, settings, true);
        }
    }
}
