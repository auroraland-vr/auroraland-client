using UnityEngine;

namespace Auroraland.Scenes.Intermediate
{
    public class Intermediate_GestureTranslator : MonoBehaviour
    {
        public GameObject Menu;

        void Start()
        {
            GestureManager.GestureDetected += OnGestureDetected;
        }

        void OnGestureDetected(Fingo.HandType handType, Fingo.GestureName gestureType)
        {
            switch (gestureType)
            {
                case Fingo.GestureName.Peace:
                    Menu.SetActive(!Menu.activeInHierarchy);
                    break;
            }
        }
    }
}