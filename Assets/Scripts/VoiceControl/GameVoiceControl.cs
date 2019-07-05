using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProjectRenaissance;

namespace Auroraland
{
    public class GameVoiceControl : MonoBehaviour
    {
        [Header("Targets")]
        public Game Game;
        public Button StartGameButton;
        public Button HitButton;
        public Button StandButton;
        public Button DoubleButton;
        public Button SplitButton;

        private void OnEnable()
        {
            VoiceCommandRecognizer.OnSaidCommonVoiceCommand += CommonVoiceCommandHandler;
            VoiceCommandRecognizer.OnSaidGameVoiceCommand += GameVoiceCommandHandler;
        }
        private void OnDisable()
        {
            VoiceCommandRecognizer.OnSaidCommonVoiceCommand -= CommonVoiceCommandHandler;
            VoiceCommandRecognizer.OnSaidGameVoiceCommand -= GameVoiceCommandHandler;
        }

        void GameVoiceCommandHandler(string gameId, VoiceCommandArgs args)
        {
            Debug.Log("Game gets user defined command:" + args.Args);
            //TODO: check if the player is actually playing this game 
            if (Game == null) return;
            if (Game.LocalGambler != null && Game.LocalGambler.IsTakingTurn)
            {
                var target = args.Args.Target;
                var action = args.Args.Action;
                if (!string.IsNullOrEmpty(action))
                { //action is not empty
                    Debug.Log("action:" + action);
                    if (string.IsNullOrEmpty(target))
                    { //action with no target
                        switch (action)
                        {
                            case "hit":
                                HitButton.onClick.Invoke();
                                break;
                            case "double":
                                if (DoubleButton.IsActive())
                                    DoubleButton.onClick.Invoke();
                                break;
                            case "split":
                                if (SplitButton.IsActive())
                                    SplitButton.onClick.Invoke();
                                break;
                            case "stand":
                                StandButton.onClick.Invoke();
                                break;
                        }
                    }
                }
            }
        }
        void CommonVoiceCommandHandler(VoiceCommandArgs args)
        {
            Debug.Log("Game gets common command:" + args.Args);
            //TODO: check if the player is actually playing this game 
            if (Game == null) return;
            if (Game.LocalGambler != null)
            {
                var target = args.Args.Target;
                var action = args.Args.Action;
                if (!string.IsNullOrEmpty(action))
                { //action is not empty
                    Debug.Log("action:" + action);
                    if (string.IsNullOrEmpty(target))
                    { //action with no target
                        switch (action)
                        {
                            case "start":
                                if (Game.IsDone)
                                    StartGameButton.onClick.Invoke();
                                break;
                        }
                    }
                }
            }
        }
    }
}

