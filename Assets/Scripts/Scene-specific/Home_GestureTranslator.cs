using UnityEngine;

namespace Auroraland.Scenes.Home
{
    public class Home_GestureTranslator : MonoBehaviour
    {
        public GameObject Menu;
        public AvatarMenu AvatarMenu;

        void Start()
        {
            GestureManager.GestureDetected += OnGestureDetected;
            AvatarMenu = GameObject.FindObjectOfType<AvatarMenu>();
        }

        void OnGestureDetected(Fingo.HandType handType, Fingo.GestureName gestureType)
        {
            switch (gestureType)
            {
                case Fingo.GestureName.Okay:
                    AvatarMenu.SelectNextAvatar();
                    break;
                case Fingo.GestureName.Peace:
                    Menu.SetActive(!Menu.activeInHierarchy);
                    break;
            }
        }
    }
}