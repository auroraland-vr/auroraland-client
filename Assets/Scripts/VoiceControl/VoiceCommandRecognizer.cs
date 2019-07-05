using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;
namespace Auroraland
{
    public class VoiceCommandRecognizer : MonoBehaviour
    {
        //Events
        public static event VoiceCommandStatusHandler VoiceCommandStatusEvent;
        public delegate void VoiceCommandStatusHandler(VoiceCommandState state, string result = null, string confidence = null);
        public static event GameVoiceCommandHandler OnSaidGameVoiceCommand;
        public delegate void GameVoiceCommandHandler(string gameId, VoiceCommandArgs args);
        public static event SystemVoiceCommandHandler OnSaidSystemVoiceCommand;
        public delegate void SystemVoiceCommandHandler(VoiceCommandArgs args);
        public static event CommonVoiceCommandHandler OnSaidCommonVoiceCommand;
        public delegate void CommonVoiceCommandHandler(VoiceCommandArgs args);

        [Header("Grammar")]
        public string GrammarFile;

        GrammarRecognizer grammarRecognizer;
        string grammarFilePath;
        bool isActivated;

        void OnEnable()
        {
            grammarFilePath = grammarFilePath = Application.streamingAssetsPath + "/SRGS/" + GrammarFile;
            grammarRecognizer = new GrammarRecognizer(grammarFilePath, ConfidenceLevel.Low);
            grammarRecognizer.OnPhraseRecognized += OnPhraseRecognized;
            VoiceCommandPromptPanel.VoiceCommandTimeOutEvent += DeactivateRecognition;
            VoiceXmlEditor.OnCreateVoiceXml += ReactivateRecognition;
            grammarRecognizer.Start();
        }

        void OnDisable()
        {
            if (grammarRecognizer != null)
            {
                grammarRecognizer.Stop();
                grammarRecognizer.OnPhraseRecognized -= OnPhraseRecognized;
                grammarRecognizer.Dispose();
                grammarRecognizer = null;
            }
            VoiceCommandPromptPanel.VoiceCommandTimeOutEvent -= DeactivateRecognition;
            VoiceXmlEditor.OnCreateVoiceXml -= ReactivateRecognition;
        }

        public void ReactivateRecognition(string fileName)
        {
            if (grammarRecognizer != null)
            {
                grammarRecognizer.Stop();
                grammarRecognizer.OnPhraseRecognized -= OnPhraseRecognized;
                grammarRecognizer.Dispose();
                grammarRecognizer = null;
            }
            string path = Application.streamingAssetsPath + fileName;
            grammarRecognizer = new GrammarRecognizer(path, ConfidenceLevel.Low);
            grammarRecognizer.OnPhraseRecognized += OnPhraseRecognized;
            grammarRecognizer.Start();
        }

        public void DeactivateRecognition()
        {
            isActivated = false;
        }

        private void OnPhraseRecognized(PhraseRecognizedEventArgs args)
        {
            if (args.semanticMeanings == null || args.semanticMeanings.Length <= 0) return;

            var targetPair = args.semanticMeanings.FirstOrDefault(semantic => semantic.key == "target");
            var actionPair = args.semanticMeanings.FirstOrDefault(semantic => semantic.key == "action");
            var activatePair = args.semanticMeanings.FirstOrDefault(semantic => semantic.key == "activate");
            var directionPair = args.semanticMeanings.FirstOrDefault(semantic => semantic.key == "direction");
            var gamePair = args.semanticMeanings.FirstOrDefault(semantic => semantic.key == "game");
            var authorityPair = args.semanticMeanings.FirstOrDefault(semantic => semantic.key == "authority");

            VoiceCommandTags tag;
            tag.Target = (!string.IsNullOrEmpty(targetPair.key)) ? targetPair.values[0] : "";
            tag.Action = (!string.IsNullOrEmpty(actionPair.key)) ? actionPair.values[0] : "";
            tag.Direction = (!string.IsNullOrEmpty(directionPair.key)) ? directionPair.values[0] : "";
            VoiceCommandArgs voiceArg = new VoiceCommandArgs(tag);

            if (!string.IsNullOrEmpty(activatePair.key))
            {
                string activateValue = activatePair.values[0];
                switch (activateValue)
                {
                    case "aurora":
                    {
                        if (VoiceCommandStatusEvent != null)
                        {
                            VoiceCommandStatusEvent(VoiceCommandState.LISTENING);
                        }

                        isActivated = true;
                        break;
                    }
                    case "deactivate":
                    {
                        if (VoiceCommandStatusEvent != null)
                        {
                            VoiceCommandStatusEvent(VoiceCommandState.DEACTIVATE);
                        }

                        isActivated = false;
                        break;
                    }
                }
            }

            if (!isActivated) return;
            if (!string.IsNullOrEmpty(tag.Target) || !string.IsNullOrEmpty(tag.Action))
            {
                if (VoiceCommandStatusEvent != null)
                {
                    VoiceCommandStatusEvent(VoiceCommandState.COMPLETED, args.text, args.confidence.ToString());
                }
            }

            if (string.IsNullOrEmpty(authorityPair.key))
            {
                return;
            }

            string authority = authorityPair.values[0];
            switch (authority)
            {
                case "system":
                {
                    if (OnSaidSystemVoiceCommand != null)
                    {
                        OnSaidSystemVoiceCommand(voiceArg);
                    }

                    break;
                }
                case "user":
                {
                    // if this is a game specific voice commands
                    if (OnSaidGameVoiceCommand != null)
                    {
                        OnSaidGameVoiceCommand(gamePair.values[0], voiceArg);
                    }

                    break;
                }
                case "common":
                {
                    if (OnSaidCommonVoiceCommand != null)
                    {
                        OnSaidCommonVoiceCommand(voiceArg);
                    }

                    break;
                }
            }
        }
    }

    public struct VoiceCommandTags
    {
        public string Target;
        public string Action;
        public string Direction;
    }

    public class VoiceCommandArgs : System.EventArgs
    {
        public readonly VoiceCommandTags Args;
        public VoiceCommandArgs(VoiceCommandTags args)
        {
            Args = args;
        }
        public override string ToString()
        {
            return string.Format("Action:{0}, Target:{1}, Direction:{2}", Args.Action, Args.Target, Args.Direction);
        }
    }
}