using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [Header("Keyboard Commands: 1. Play, 2. Pause, 3. Stop, 4. Next, 5. Previous, + - "),Space]
    public List<AudioClip> audioClips;

   
    public List<AudioSource> audioSources;

    int currentClip;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) Play();
        if (Input.GetKeyDown(KeyCode.Alpha2)) Pause();
        if (Input.GetKeyDown(KeyCode.Alpha3)) Stop();
        if (Input.GetKeyDown(KeyCode.Alpha4)) Next();
        if (Input.GetKeyDown(KeyCode.Alpha5)) Previous();
        if (Input.GetKeyDown(KeyCode.Equals) || Input.GetKeyDown(KeyCode.KeypadPlus)) AdjustVolume(0.1f);
        if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus)) AdjustVolume(-0.1f);
    }

    /// <summary>
    /// Increase or decrease the volume by an amount.
    /// Use negative values to lower the volume(0-1).
    /// </summary>
    /// <param name="amt">The amount to raise or lower the volume</param>
    void AdjustVolume(float amt)
    {
        foreach (var aud in audioSources)
        {
            float volume = Mathf.Clamp01(aud.volume + amt);
            aud.volume = volume;
        }
    }


    public void Play()
    {
        foreach (var aud in audioSources)
        {
            aud.clip = audioClips[currentClip];
            aud.Play();
        }
    }

    public void Pause()
    {
        foreach (var aud in audioSources) aud.Pause();
    }

    public void Stop()
    {
        foreach (var aud in audioSources) aud.Stop();
    }

    public void Next()
    {
        Stop();

        if (currentClip == audioClips.Count - 1)
            currentClip = 0;
        else currentClip++;

        Play();
    }

    public void Previous()
    {
        Stop();
        if (currentClip == 0)
            currentClip = audioClips.Count - 1;
        else currentClip--;
        Play();
    }
}