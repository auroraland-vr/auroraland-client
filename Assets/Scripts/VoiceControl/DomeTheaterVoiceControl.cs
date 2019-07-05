using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Auroraland
{
    public class DomeTheaterVoiceControl : MonoBehaviour
    {
        Dictionary<string, UnityAction> actionDict = new Dictionary<string, UnityAction>();

        // Action
        public delegate void DomeMovieCommandHandler();
        public static event DomeMovieCommandHandler OnPlayDomeMovie;
        //public delegate void DomeMoviePlayCommandHandler();
        public static event DomeMovieCommandHandler OnStopDomeMovie;
        //public delegate void DomeMovieStopCommandHandler();
        public static event DomeMovieCommandHandler OnPauseDomeMovie;
        //public delegate void DomeMoviePauseCommandHandler();
        public static event DomeMovieCommandHandler OnNextDomeMovie;
        public static event DomeMovieCommandHandler OnPreviousDomeMovie;

        public delegate void DomeTheaterVolumeCommandHandler(float volumeDelta);
        public static event DomeTheaterVolumeCommandHandler OnAdjustDomeMovieVolume;


        private void OnEnable()
        {
            VoiceCommandRecognizer.OnSaidCommonVoiceCommand += CommonVoiceCommandHandler;
        }

        private void OnDisable()
        {
            VoiceCommandRecognizer.OnSaidCommonVoiceCommand -= CommonVoiceCommandHandler;
        }

        void Start()
        {
            actionDict.Add("start", PlayDomeMovie);
            actionDict.Add("pause", PauseDomeMovie);
            actionDict.Add("stop", StopDomeMovie);
            actionDict.Add("louder", LouderDomeMovieVolume);
            actionDict.Add("lower", LowerDomeMovieVolume);
            actionDict.Add("next", NextDomeMovie);
            actionDict.Add("previous", PreviousDomeMovie);
        }

        void CommonVoiceCommandHandler(VoiceCommandArgs args)
        {
            Debug.Log("Dome Theater gets common command:" + args.Args);

            var target = args.Args.Target;
            var action = args.Args.Action;

            if (!string.IsNullOrEmpty(action))
            {   //action is not empty
                Debug.Log("dome theater action:" + action);
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

        void PlayDomeMovie() {
            OnPlayDomeMovie();
        }

        void PauseDomeMovie() {
            OnPauseDomeMovie();
        }

        void StopDomeMovie() {
            OnStopDomeMovie();
        }

        void LouderDomeMovieVolume() {
            OnAdjustDomeMovieVolume(0.1f);
        }

        void LowerDomeMovieVolume() {
            OnAdjustDomeMovieVolume(-0.1f);
        }

        void NextDomeMovie() {
            OnNextDomeMovie();
        }

        void PreviousDomeMovie()
        {
            OnPreviousDomeMovie();
        }
    }
}