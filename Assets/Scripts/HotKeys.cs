using UnityEngine;

namespace Auroraland
{
    public class HotKeys : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField]
        SceneTransitionController _sceneTransitionController;

        public GameObject SpaceMenu;
        public GameObject HomeMenu;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (_sceneTransitionController != null && _sceneTransitionController.IsCurrentThemeHome)
                    HomeMenu.SetActive(!HomeMenu.activeSelf);
                else
                    SpaceMenu.SetActive(!SpaceMenu.activeSelf);
            }

            if (Input.GetKeyDown(KeyCode.F4))
            {
                Application.Quit();
            }
        }
    }
}
