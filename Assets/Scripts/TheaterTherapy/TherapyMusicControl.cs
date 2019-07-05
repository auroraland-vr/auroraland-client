using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.IO;

namespace Auroraland {

    public class TherapyMusicControl : MonoBehaviour {
        public List<AudioClip> audioClips;
        public List<AudioSource> audioSources;
        int currentClip;

        //Dictionary<string, string> audioNameToLocalPath = new Dictionary<string, string>();
        private string filesLocalPath;

        void Awake() {
            filesLocalPath = @"d:\Auroraland\media\treatment\audio"; 
        }
        
        //void Start()
        //{
        //    TherapyGetAudioList();
        //}

        private void OnEnable()
        {
            TherapyVoiceControl.OnPlayMusic += PlayMusicHandler;
            TherapyVoiceControl.OnStopMusic += StopMusicHandler;
            TherapyVoiceControl.OnPauseMusic += PauseMusicHandler;
            TherapyVoiceControl.OnAdjustMusicVolume += AdjustMusicVolumeHandler;
            TherapyVoiceControl.OnNextMusic += NextMusicHandler;
            TherapyVoiceControl.OnPreviousMusic += PreviousMusicHandler;
        }

        private void OnDisable()
        {
            TherapyVoiceControl.OnPlayMusic -= PlayMusicHandler;
            TherapyVoiceControl.OnStopMusic -= StopMusicHandler;
            TherapyVoiceControl.OnPauseMusic -= PauseMusicHandler;
            TherapyVoiceControl.OnAdjustMusicVolume -= AdjustMusicVolumeHandler;
            TherapyVoiceControl.OnNextMusic -= NextMusicHandler;
            TherapyVoiceControl.OnPreviousMusic -= PreviousMusicHandler;
        }

        void PlayMusicHandler() {
            Debug.Log("On Play music");
            foreach (var aud in audioSources)
            {
                aud.clip = audioClips[currentClip];
                aud.Play();
            }
        }

        void StopMusicHandler() {
            Debug.Log("On Stop music");
            foreach (var aud in audioSources) aud.Stop();
        }

        void PauseMusicHandler() {
            foreach (var aud in audioSources) aud.Pause();
        }

        void AdjustMusicVolumeHandler(float amt) {
            foreach (var aud in audioSources)
            {
                float volume = Mathf.Clamp01(aud.volume + amt);
                aud.volume = volume;
            }
        }

        void NextMusicHandler() {
            StopMusicHandler();

            if (currentClip == audioClips.Count - 1)
                currentClip = 0;
            else currentClip++;

            PlayMusicHandler();
        }

        void PreviousMusicHandler() {
            StopMusicHandler();
            if (currentClip == 0)
                currentClip = audioClips.Count - 1;
            else currentClip--;
            PlayMusicHandler();
        }

        //public void TherapyGetAudioList()
        //{
        //    DirectoryInfo therapyLocalDir = new DirectoryInfo(filesLocalPath);
        //    FileInfo[] info = therapyLocalDir.GetFiles("*.mp3");
        //    foreach (FileInfo f in info)
        //    {
        //        //audioNameToLocalPath.Add(f.Name, f.FullName);
        //    }
        //}
    }
}