using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace Auroraland
{
    public class StandardTheaterVoiceControl: MonoBehaviour
    {
        string currentTarget;
        Dictionary<string, UnityAction> actionDict = new Dictionary<string, UnityAction>();
        //bool needCheckWhenPlay;

        private void OnEnable()
        {
            VoiceCommandRecognizer.OnSaidCommonVoiceCommand += CommonVoiceCommandHandler;
        }
        private void OnDisable()
        {
            VoiceCommandRecognizer.OnSaidCommonVoiceCommand -= CommonVoiceCommandHandler;
        }

        // Use this for initialization
        void Start()
        {
            actionDict.Add("pause", Pause);
            actionDict.Add("start", Play);
            actionDict.Add("stop", Stop);
            actionDict.Add("louder", Louder);
            actionDict.Add("lower", Lower);
            //needCheckWhenPlay = false;
        }

        private void Pause() /*Movie texture is still on the screen*/
        {
            Debug.Log("Standard Theater Pause!!!");
            PlayControl.Instance.PlayPause();
            PlayControl.Instance.TurnOnLights();
            PlayControl.Instance.SwitchMaterial(true);
        }

        private void Stop() /*Movie texture is not on the screen*/
        {
            Debug.Log("Standard Theater Stop!!!");
            PlayControl.Instance.PlayStop();
            //PlayControl.Instance.MovieTheaterScreenMesh();
            //needCheckWhenPlay = true;
            PlayControl.Instance.TurnOnLights();
            PlayControl.Instance.SwitchMaterial(true);
        }

        private void Play() /*Equals to Start Command*/
        {
            //if (needCheckWhenPlay) {
            //    PlayControl.Instance.MovieTheaterScreenMesh();
            //}
            Debug.Log("Standard Theater Play!!!");
            PlayControl.Instance.PlayContinue();
            PlayControl.Instance.TurnOffLightsSlowly();
            PlayControl.Instance.SwitchMaterial(false);
        }

        private void Louder() {
            PlayControl.Instance.LouderVolume();
        }

        private void Lower() {
            PlayControl.Instance.LowerVolume();
        }

        void CommonVoiceCommandHandler(VoiceCommandArgs args)
        {
            Debug.Log("Standard Theater gets common command:" + args.Args);

            var target = args.Args.Target;
            var action = args.Args.Action;

            if (!string.IsNullOrEmpty(action))
            {   //action is not empty
                Debug.Log("standard theater action:" + action);
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
    }
}

    
