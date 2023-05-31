using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound
{
    [Space(10)]
    //----SoundData
    public string SoundName;
    public bool Loop;
    public bool PlayOnAwake;

    [Space(10)]
    //----Sldiers
    [Range(0, 1)] public float Volume;
    [Range(0, 3)] public float Pitch = 1;
    
    [Space(10)]
    //----AudioSource Data
    [HideInInspector]
    public AudioSource Source;

    [HideInInspector]
    public AudioMixerGroup Mixer;
    public AudioClip Clip;
}