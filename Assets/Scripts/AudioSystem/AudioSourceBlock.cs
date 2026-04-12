
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModularFW.Core.AudioSystem
{

public class AudioSourceBlock : MonoBehaviour
{
    [SerializeField] private List<AudioSource> AudioSources;

    private int currentSoundSource = 0;
    public void PlayAudio(AudioClip audioClip)
    {
        AudioSources[currentSoundSource].clip = audioClip;
        AudioSources[currentSoundSource].Play();
        currentSoundSource++;
        currentSoundSource %= AudioSources.Count;
    }
}

}
