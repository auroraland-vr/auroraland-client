using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Auroraland
{
    public class IMAXTheaterVoiceControl : MonoBehaviour
    {
        Dictionary<string, UnityAction> actionDict = new Dictionary<string, UnityAction>();

        // Action
        public delegate void IMAXMovieCommandHandler();
        public static event IMAXMovieCommandHandler OnPlayIMAXMovie;
        public static event IMAXMovieCommandHandler OnStopIMAXMovie;
        public static event IMAXMovieCommandHandler OnPauseIMAXMovie;
        public static event IMAXMovieCommandHandler OnNextIMAXMovie;
        public static event IMAXMovieCommandHandler OnPreviousIMAXMovie;
        public delegate void IMAXTheaterVolumeCommandHandler(float volumeDelta);
        public static event IMAXTheaterVolumeCommandHandler OnAdjustIMAXMovieVolume;

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
            actionDict.Add("start", PlayIMAXMovie);
            actionDict.Add("pause", PauseIMAXMovie);
            actionDict.Add("stop", StopIMAXMovie);
            actionDict.Add("louder", LouderIMAXMovieVolume);
            actionDict.Add("lower", LowerIMAXMovieVolume);
            actionDict.Add("next", NextIMAXMovie);
            actionDict.Add("previous", PreviousIMAXMovie);
        }

        void CommonVoiceCommandHandler(VoiceCommandArgs args)
        {
            Debug.Log("IMAX Theater gets common command:" + args.Args);

            var target = args.Args.Target;
            var action = args.Args.Action;

            if (!string.IsNullOrEmpty(action))
            {   //action is not empty
                Debug.Log("IMAX theater action:" + action);
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

        void PlayIMAXMovie()
        {
            OnPlayIMAXMovie();
        }

        void PauseIMAXMovie()
        {
            OnPauseIMAXMovie();
        }

        void StopIMAXMovie()
        {
            OnStopIMAXMovie();
        }

        void LouderIMAXMovieVolume()
        {
            OnAdjustIMAXMovieVolume(0.1f);
        }

        void LowerIMAXMovieVolume()
        {
            OnAdjustIMAXMovieVolume(-0.1f);
        }

        void NextIMAXMovie()
        {
            OnNextIMAXMovie();
        }

        void PreviousIMAXMovie()
        {
            OnPreviousIMAXMovie();
        }
    }
}