using System.Collections.Generic;
using UnityEngine;

namespace ModularFW.Core.AudioSystem
{
    public class AudioSourceBlock : MonoBehaviour
    {
        [SerializeField] private List<AudioSource> AudioSources;
        [SerializeField] private AudioSource MusicSource;

        private int currentSoundSource = 0;

        public void PlaySfx(AudioClip clip, float volume, int priority = 128)
        {
            if (AudioSources == null || AudioSources.Count == 0) return;
            var source = AudioSources[currentSoundSource];
            source.clip = clip;
            source.volume = volume;
            source.priority = priority;
            source.Play();
            currentSoundSource = (currentSoundSource + 1) % AudioSources.Count;
        }

        public void PlayMusic(AudioClip clip, float volume, int priority = 128)
        {
            if (MusicSource == null) return;
            if (MusicSource.clip == clip && MusicSource.isPlaying)
            {
                MusicSource.volume = volume;
                return;
            }
            MusicSource.clip = clip;
            MusicSource.volume = volume;
            MusicSource.priority = priority;
            MusicSource.loop = true;
            MusicSource.Play();
        }

        public void SetSfxVolume(float volume)
        {
            if (AudioSources == null) return;
            foreach (var s in AudioSources) s.volume = volume;
        }

        public void SetMusicVolume(float volume)
        {
            if (MusicSource != null) MusicSource.volume = volume;
        }

        public void StopMusic() { MusicSource?.Stop(); }
    }
}
