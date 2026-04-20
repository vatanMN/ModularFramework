
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModularFW.Core.AudioSystem
{

    [CreateAssetMenu(fileName = "AudioCollection", menuName = "Custom/AudioCollection")]
    public class AudioCollection : ScriptableObject
{
    [SerializeField] List<AudioElement> audioElements = new List<AudioElement>();

    private static Dictionary<AudioEnum, AudioElement> audioElementsDic = new Dictionary<AudioEnum, AudioElement>();

    [NonSerialized]
    private bool isLoaded = false;

    private void Load()
    {
        foreach (var item in audioElements)
        {
            audioElementsDic[item.Type] = item;
        }
        isLoaded = true;
    }

    public AudioElement GetAudioElement(AudioEnum auidoType)
    {
        if (!isLoaded)
            Load();
        return audioElementsDic[auidoType];
    }

    }

    public enum AudioCategory { Sfx, Music }

    [Serializable]
    public class AudioElement
    {
        public AudioEnum Type;
        public AudioClip Clip;
        public AudioCategory Category = AudioCategory.Sfx;
        [Range(0, 256)] public int Priority = 128;
    }
}