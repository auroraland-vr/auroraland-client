using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Auroraland
{
    public class TherapyVoiceControl : MonoBehaviour {

        Dictionary<string, UnityAction> actionDict = new Dictionary<string, UnityAction>();

        // Action
        public static event TherapyVideoPlayCommandHandler OnPlayVideo;
        public delegate void TherapyVideoPlayCommandHandler(string videoName);
        public static event TherapyVideoStopCommandHandler OnStopVideo;
        public delegate void TherapyVideoStopCommandHandler();
        //public static delegate void TherapyVideoStopCommandHandler();
        public delegate void TherapyMusicCommandHandler();
        public static event TherapyMusicCommandHandler OnPlayMusic;
        public static event TherapyMusicCommandHandler OnStopMusic;
        public static event TherapyMusicCommandHandler OnPauseMusic;
        public static event TherapyMusicCommandHandler OnNextMusic;
        public static event TherapyMusicCommandHandler OnPreviousMusic;
        public static event TherapyVolumeCommandHandler OnAdjustMusicVolume;
        public delegate void TherapyVolumeCommandHandler(float volumeDelta);

        private void OnEnable()
        {
            VoiceCommandRecognizer.OnSaidCommonVoiceCommand += CommonVoiceCommandHandler;
            VoiceCommandRecognizer.OnSaidSystemVoiceCommand += SystemVoiceCommandHandler;
        }
        private void OnDisable()
        {
            VoiceCommandRecognizer.OnSaidCommonVoiceCommand -= CommonVoiceCommandHandler;
            VoiceCommandRecognizer.OnSaidSystemVoiceCommand -= SystemVoiceCommandHandler;
        }

        // Use this for initialization
        void Start()
        {
            // video
            actionDict.Add("night", PlayNight);
            actionDict.Add("flowers", PlayFlowers);
            actionDict.Add("waves", PlayWaves);
            actionDict.Add("sunset", PlaySunset);
            actionDict.Add("stop", StopVideo);

            // audio
            actionDict.Add("music", PlayMusic);
            actionDict.Add("stop music", StopMusic);
            actionDict.Add("pause music", PauseMusic);
            actionDict.Add("louder", LouderMusic);
            actionDict.Add("lower", LowerMusic);
            actionDict.Add("next", NextMusic);
            actionDict.Add("previous", PreviousMusic);
        }

        void CommonVoiceCommandHandler(VoiceCommandArgs args)
        {
            Debug.Log("Therapy gets common command:" + args.Args);

            var target = args.Args.Target;
            var action = args.Args.Action;

            if (!string.IsNullOrEmpty(action))
            {   //action is not empty
                Debug.Log("therapy action:" + action);
                if (string.IsNullOrEmpty(target))
                {   //action with no target
                    if (actionDict.ContainsKey(action))
                    {
                        actionDict[action].Invoke();
                    }
                    else
                    {
                        Debug.LogFormat("{0} is undefined.", action);
                    }
                }
            }
        }

        void SystemVoiceCommandHandler(VoiceCommandArgs args) {
            Debug.Log("Therapy gets sustem command:" + args.Args);

            var target = args.Args.Target;
            var action = args.Args.Action;

            if (!string.IsNullOrEmpty(action))
            {   //action is not empty
                Debug.Log("therapy action:" + action);
                if (string.IsNullOrEmpty(target))
                {   //action with no target
                    if (actionDict.ContainsKey(action))
                    {
                        actionDict[action].Invoke();
                    }
                    else
                    {
                        Debug.LogFormat("{0} is undefined.", action);
                    }
                }
            }
        }

        // video
        void PlayNight()
        {
            Debug.Log("Call Play video night");
            OnPlayVideo("night");
        }

        void PlayFlowers()
        {
            Debug.Log("Call Play video flowers");
            OnPlayVideo("flowers");
        }

        void PlayWaves()
        {
            Debug.Log("Call Play video waves");
            OnPlayVideo("waves");
        }

        void PlaySunset()
        {
            Debug.Log("Call Play video sunset");
            OnPlayVideo("sunset");
        }

        void StopVideo()
        {
            Debug.Log("Call stop video");
            OnStopVideo();
        }

        // audio
        void PlayMusic()
        {
            OnPlayMusic();
        }

        void StopMusic()
        {
            OnStopMusic();
        }

        void PauseMusic()
        {
            OnPauseMusic();
        }

        void LouderMusic()
        {
            OnAdjustMusicVolume(0.1f);
        }

        void LowerMusic()
        {
            OnAdjustMusicVolume(-0.1f);
        }

        void NextMusic()
        {
            OnNextMusic();
        }

        void PreviousMusic()
        {
            OnPreviousMusic();
        }
    }
}
