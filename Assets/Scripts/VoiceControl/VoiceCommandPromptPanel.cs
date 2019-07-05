using UnityEngine;

namespace Auroraland
{
    /// <summary>
    /// The class receive and set voice command state for displaying information on the prompt panel
    /// </summary>
    public enum VoiceCommandState { PROMPT, LISTENING, CANCELED, COMPLETED, DEACTIVATE }
    public class VoiceCommandPromptPanel : MonoBehaviour
    {
        public static event VoiceCommandStatusHandler VoiceCommandTimeOutEvent;
        public delegate void VoiceCommandStatusHandler();

        [Header("Timer")]
        public float PromptDuration;
        public float ListeningDuration;
        public float CancelDuration;
        public float DeactivateDuration;

        [Header("UI")]
        public GameObject VoiceCommandCanvas;
        public PromptNotification Prompt;
        public PromptNotification Result;
        public PromptNotification Cancel;

        private readonly string listenText = "Listening...";
        private readonly string hearNothingText = "Sorry about that, I didn't hear anything...";
        private readonly string deactivateText = "Ok. See you later!";
        private VoiceCommandState _state;
        private PromptNotification current;

        void Awake()
        {
            Prompt.Duration = PromptDuration;
            Result.Duration = ListeningDuration;
            Cancel.Duration = CancelDuration;
            VoiceCommandCanvas.SetActive(false);
            Prompt.SetActive(false);
            Result.SetActive(false);
            Cancel.SetActive(false);
        }

        void OnEnable()
        {
            VoiceCommandRecognizer.VoiceCommandStatusEvent += OnSetVoiceCommandState;
            SetState(VoiceCommandState.PROMPT); ;
        }

        void OnDisable()
        {
            VoiceCommandRecognizer.VoiceCommandStatusEvent -= OnSetVoiceCommandState;
        }

        void Update()
        {
            if (current.IsCountingDown()) return;

            if (_state == VoiceCommandState.LISTENING)
            {
                SetState(VoiceCommandState.CANCELED);
                if (VoiceCommandTimeOutEvent != null)
                {
                    VoiceCommandTimeOutEvent();
                }
            }
            else
            {
                if (_state == VoiceCommandState.COMPLETED)
                {
                    if (VoiceCommandTimeOutEvent != null)
                    {
                        VoiceCommandTimeOutEvent();
                    }
                }

                SetCurrentPromptActive(false);
            }
        }

        void OnSetVoiceCommandState(VoiceCommandState state, string result, string confidence)
        {
            SetState(state);

            if (!string.IsNullOrEmpty(result) && !string.IsNullOrEmpty(confidence))
            {
                SetResult(result, confidence);
            }
        }

        void SetState(VoiceCommandState state)
        {
            this._state = state;
            if (current)
            {
                current.SetActive(false);
            }

            switch (state)
            {
                case VoiceCommandState.PROMPT:
                    current = Prompt;
                    break;
                case VoiceCommandState.LISTENING:
                    current = Result;
                    Result.SetNotification(listenText);
                    break;
                case VoiceCommandState.CANCELED:
                    current = Cancel;
                    Cancel.SetNotification(hearNothingText);
                    Cancel.Duration = CancelDuration;
                    break;
                case VoiceCommandState.COMPLETED:
                    current = Result;
                    break;
                case VoiceCommandState.DEACTIVATE:
                    current = Cancel;
                    Cancel.SetNotification(deactivateText);
                    Cancel.Duration = DeactivateDuration;
                    break;
            }
            VoiceCommandCanvas.SetActive(true);
            current.SetActive(true);
            current.ResetTimer();
            current.StartTiming();
        }

        void SetResult(string text, string confidence)
        {
            string result = "";
            if (text.Contains("aurora"))
            {
                result += "Aurora is activated!\n";
            }
            result += string.Format("Ok. You said \"{0}\"\n (confidence: {1})", text, confidence);
            Result.SetNotification(result);
        }

        void SetCurrentPromptActive(bool isOn)
        {
            VoiceCommandCanvas.SetActive(isOn);
            current.SetActive(isOn);
        }
    }
}