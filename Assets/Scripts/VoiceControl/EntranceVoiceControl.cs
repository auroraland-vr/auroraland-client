using UnityEngine;
using UnityEngine.UI;

namespace Auroraland
{
    public class EntranceVoiceControl : MonoBehaviour
    {
        [Header("Targets")]
        public EntranceSceneController EntranceSceneController;
        public GameObject SignInPanel;
        public GameObject SignUpPanel;
        public Button SignInButton;
        public Button SignUpButton;

        private void OnEnable()
        {
            VoiceCommandRecognizer.OnSaidSystemVoiceCommand += SystemVoiceCommandHandler;
        }
        private void OnDisable()
        {
            VoiceCommandRecognizer.OnSaidSystemVoiceCommand -= SystemVoiceCommandHandler;
        }

        void SystemVoiceCommandHandler(VoiceCommandArgs args)
        {
            if (!string.IsNullOrEmpty(args.Args.Action))
            {
                string actionName = args.Args.Action;

                if (actionName == "sign in")
                {
                    if (SignInPanel.activeSelf)
                    {
                        EntranceSceneController.SignIn();
                    }
                    else
                    {
                        SignInButton.onClick.Invoke();
                    }
                }
                else if (actionName == "sign up")
                {
                    if (SignUpPanel.activeSelf)
                    {
                        EntranceSceneController.SignUp();
                    }
                    else
                    {
                        SignUpButton.onClick.Invoke();
                    }
                }
            }
        }
    }
}

