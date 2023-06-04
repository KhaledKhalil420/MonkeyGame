using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using System.Collections;

//Khaled_khalil's Ass-ets
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public List<Sound> Sounds;
    public AudioMixer DefaultAudioMixer;

    private bool IsFading, CanFade;

    private void Awake()
    {
        //Don't destroy this game Object when a new scene loads
        DontDestroyOnLoad(gameObject);

        //If there was 2 "AudioManagers" Delete the new Loaded one and keep the Original
        if(Instance == null) Instance = this; 
        else { Destroy(this.gameObject); return; }

        //Setup AudioManager
        SetupAudioManager();
    }

    private void Update()
    {
        if(Instance == null) Instance = this;   
    }


    void SetupAudioManager()
    {
        foreach(Sound SoundToPlay in Sounds)
        {
            //Setup AudioSource
            SoundToPlay.Source = gameObject.AddComponent<AudioSource>();

            //Setup AudioSource Data
            SoundToPlay.Source.volume = SoundToPlay.Volume;
            SoundToPlay.Source.pitch = SoundToPlay.Pitch;

            SoundToPlay.Source.loop = SoundToPlay.Loop;
            SoundToPlay.Source.clip = SoundToPlay.Clip;

            if(SoundToPlay.PlayOnAwake) SoundToPlay.Source.Play();

            SoundToPlay.Source.outputAudioMixerGroup = DefaultAudioMixer.outputAudioMixerGroup;
        }
    }

    #region Sound settings

    public void PlaySound(string SoundName)
    {
        //Find sound To play based on their name
        foreach(Sound SoundToPlay in Sounds)
            if(SoundToPlay.SoundName == SoundName) SoundToPlay.Source.Play();
    }

    public void StopSound(string SoundName)
    {
        //Find sound To stop playing based on their name
        foreach(Sound SoundToPlay in Sounds)
            if(SoundToPlay.SoundName == SoundName) SoundToPlay.Source.Stop();
    }

    public void ReplaceSoundClip(string SoundName, AudioClip NewClip)
    {
        //Find sound To relace its clip based on their name
        foreach(Sound SoundToPlay in Sounds)
            if(SoundToPlay.SoundName == SoundName) SoundToPlay.Source.clip = NewClip;
            PlaySound(SoundName);
    }

    public void PlaySoundWithRandomPitch(string SoundName, float Min, float Max)
    {
        foreach(Sound SoundToPlay in Sounds)
            if(SoundToPlay.SoundName == SoundName) 
            {
                SoundToPlay.Pitch = Random.Range(Min, Max);
                SoundToPlay.Source.Play();
            }
    }

    public void ChangeVolume(string SoundName, float Volume)
    {
        foreach(Sound SoundToPlay in Sounds)

        if(SoundToPlay.SoundName == SoundName) 
        {
            SoundToPlay.Source.volume = Volume;
        }
    }
    
    #endregion
}