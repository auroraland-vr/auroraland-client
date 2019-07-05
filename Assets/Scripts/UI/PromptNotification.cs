using UnityEngine;
using UnityEngine.UI;

namespace Auroraland
{
    public class PromptNotification : MonoBehaviour
    {
        public Text Notification;
        public Image ProgressIcon;
        [HideInInspector]
        public float Duration;

        float timer;
        bool hasStartTiming = false;

        // Update is called once per frame
        void Update()
        {
            if (hasStartTiming)
            {
                if (timer < Duration)
                {
                    timer += Time.deltaTime;
                    SetIconProgress(timer, Duration);
                }
                else
                {
                    ProgressIcon.fillAmount = 1;
                    hasStartTiming = false;
                }
            }
            else
            {
                SetActive(false); //close this prompt panel
            }
        }

        public void SetActive(bool isOn)
        {
            Notification.gameObject.SetActive(isOn);
        }

        public void StartTiming()
        {
            hasStartTiming = true;
        }

        public void ResetTimer()
        {
            timer = 0;
            ProgressIcon.fillAmount = 0;
        }

        public void SetNotification(string text)
        {
            Notification.text = text;
        }

        public bool IsCountingDown()
        {
            return (timer < Duration);
        }

        public void SetIconProgress(float value, float maxValue)
        {
            float progress = value / maxValue;
            ProgressIcon.fillAmount = progress;
        }
    }
}