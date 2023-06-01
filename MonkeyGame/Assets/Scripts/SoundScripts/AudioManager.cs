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
    public float FadingSpeed;
    float CurrnetMusicVolume = 1;

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

    private void FixedUpdate()
    {
        FadeMusic();
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
    
    #region DyanmicMusic

    public IEnumerator FadeMusicIn(float FadingTime, AudioClip clip)
    {
        CanFade = true;
        IsFading = true;

        yield return new WaitForSeconds(FadingTime);

        ReplaceSoundClip("Music", clip);
        IsFading = false;
    }


    void FadeMusic()
    {
        Sound Music = new Sound();
        bool FoundSound = new bool();

        if(!FoundSound)
        {
            foreach(Sound SoundToFind in Sounds)
            {
                if(SoundToFind.SoundName == "Music") Music = SoundToFind;
                FoundSound = true;
            }
        }

        if(CanFade)
        {
            Music.Volume = Mathf.Clamp(CurrnetMusicVolume, 0, 1);
            if(IsFading)
            {
                if(Music.Volume > 0)
                {
                    CurrnetMusicVolume -= Time.fixedDeltaTime * FadingSpeed;
                }
            }    

            if(!IsFading & CanFade)
            {
                CurrnetMusicVolume += Time.fixedDeltaTime  * FadingSpeed;

                if(Music.Volume >= 0.99f)
                {
                    CanFade = false;
                }
            }
            ChangeVolume("Music", CurrnetMusicVolume);
        }
    }

    #endregion
}