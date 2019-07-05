using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Auroraland
{
    public class T360TheaterVoiceControl : MonoBehaviour
    {
        Dictionary<string, UnityAction> actionDict = new Dictionary<string, UnityAction>();

        // Action
        public delegate void T360MovieCommandHandler();
        public static event T360MovieCommandHandler OnPlayT360Movie;
        public static event T360MovieCommandHandler OnStopT360Movie;
        public static event T360MovieCommandHandler OnPauseT360Movie;
        public static event T360MovieCommandHandler OnNextT360Movie;
        public static event T360MovieCommandHandler OnPreviousT360Movie;

        public delegate void T360TheaterVolumeCommandHandler(float volumeDelta);
        public static event T360TheaterVolumeCommandHandler OnAdjustT360MovieVolume;

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
            actionDict.Add("start", PlayT360Movie);
            actionDict.Add("pause", PauseT360Movie);
            actionDict.Add("stop", StopT360Movie);
            actionDict.Add("louder", LouderT360MovieVolume);
            actionDict.Add("lower", LowerT360MovieVolume);
            actionDict.Add("next", NextT360Movie);
            actionDict.Add("previous", PreviousT360Movie);
        }

        void CommonVoiceCommandHandler(VoiceCommandArgs args)
        {
            Debug.Log("360 Theater gets common command:" + args.Args);

            var target = args.Args.Target;
            var action = args.Args.Action;

            if (!string.IsNullOrEmpty(action))
            {   //action is not empty
                Debug.Log("360 theater action:" + action);
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

        void PlayT360Movie() {
            OnPlayT360Movie();
        }

        void StopT360Movie() {
            OnStopT360Movie();
        }

        void PauseT360Movie() {
            OnPauseT360Movie();
        }

        void LouderT360MovieVolume() {
            OnAdjustT360MovieVolume(0.1f);
        }

        void LowerT360MovieVolume() {
            OnAdjustT360MovieVolume(-0.1f);
        }

        void NextT360Movie() {
            OnNextT360Movie();
        }

        void PreviousT360Movie() {
            OnPreviousT360Movie();
        }
    }
}