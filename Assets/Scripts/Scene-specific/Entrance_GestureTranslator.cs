using UnityEngine;
using UnityEngine.UI;

namespace Auroraland.Scenes.Entrance
{
    public class Entrance_GestureTranslator : MonoBehaviour
    {
        [Header("UI")]
        public Button SignInButton;

        void Start()
        {
            GestureManager.GestureDetected += OnGestureDetected;
        }

        void OnGestureDetected(Fingo.HandType handType, Fingo.GestureName gestureType)
        {
            switch (gestureType)
            {
                case Fingo.GestureName.Okay:
                case Fingo.GestureName.Peace:
                    SignInButton.onClick.Invoke();
                    break;
            }
        }
    }
}