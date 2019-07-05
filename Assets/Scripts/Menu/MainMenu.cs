using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

namespace Auroraland
{
    public class MainMenu : MonoBehaviour
    {
        public enum MENU_STATE { ALL_SPACE = 0, MY_SPACE, CREATE_SPACE, EDIT_SPACE, USER_PROFILE, EDIT_USER_PROFILE, MODEL_LIST, GAME_LIST, HOME, NONE };
        [System.Serializable]
        public struct MenuLayout
        {
            public string Name; // to show in inspector
            public MENU_STATE State;
            public Button MenuButton;
            public GameObject[] Components;
        }
        public static event MainMenuEvent OnDisableMainMenu;
        public static event MainMenuEvent OnEnableMainMenu;
        public delegate void MainMenuEvent();

        public bool IsHome;
        public GameObject VRKeyboard;

        [Header("Main Options")]
        public GameObject HomeMenuOptions;
        public GameObject SpaceMenuOptions;
        public GameObject MySpaceMenuOptions;
        public Animator DeleteModeButtonAnim; //special case of the buttons

        [Header("Menu Components")]
        public List<MenuLayout> HomeMenus = new List<MenuLayout>();
        public List<MenuLayout> SpaceMenus = new List<MenuLayout>();
        public List<MenuLayout> MySpaceMenus = new List<MenuLayout>();
        private bool isEditMode = false;
        private bool isToggledDeleteMode = false;
        public bool IsThemeTransiting { set; get; }
        private MENU_STATE currentMenuState = MENU_STATE.NONE;

        // Use this for initialization
        void OnEnable()
        {
            isEditMode = (PlayerPrefs.GetInt("isEditMode") == 1);

            ParentConstraint constraint = transform.parent.GetComponent<ParentConstraint>();

            if (constraint != null)
            {
                constraint.translationAxis = Axis.None;
                constraint.rotationAxis = Axis.None;
            }

            SetMenu(true);

            if (OnEnableMainMenu != null)
            {
                OnEnableMainMenu();
            }
        }

        void OnDisable()
        {
            SetMenu(false);
            VRKeyboard.SetActive(false);

            ParentConstraint constraint = transform.parent.GetComponent<ParentConstraint>();
            if (constraint != null)
            {
                constraint.translationAxis = Axis.X | Axis.Y | Axis.Z;
                constraint.rotationAxis = Axis.Y;
            }

            if (OnDisableMainMenu != null)
            {
                OnDisableMainMenu();
            }
        }

        private void Start()
        {
            StartCoroutine(FixNonVRInputUI());
        }

        /// <summary>
        /// Quick fix to enable interaction with menu by NonVR player
        /// </summary>
        IEnumerator FixNonVRInputUI()
        {
            while (SDK_SetupMode.Instance.CurrentSDKName == null)
            {
                yield return null; // Wait for SDK_SetupMode to initialize
            }

            if (string.Equals(SDK_SetupMode.Instance.CurrentSDKName, "NonVR"))
            {
                gameObject.AddComponent<GraphicRaycaster>();
            }
        }

        public void SetMenu(bool isOn)
        {
            gameObject.SetActive(isOn);
            currentMenuState = MENU_STATE.NONE;

            if (IsHome)
            {
                //set home menu
                HomeMenuOptions.SetActive(isOn);
                if (isOn)
                {
                    int profileIndex = (int)MENU_STATE.USER_PROFILE;
                    int homeMenuIndex = (int)MENU_STATE.ALL_SPACE;
                    ChangeToHomeMenu((IsThemeTransiting) ? profileIndex : homeMenuIndex); // all space as default
                }
                else
                {
                    CloseAllHomeMenus();
                    ResetAllButtonAnimations(HomeMenuOptions.transform);
                }
            }
            else
            {
                if (isEditMode)
                {
                    //set space menu with editors (for space creator)
                    MySpaceMenuOptions.SetActive(isOn);
                    SpaceMenuOptions.SetActive(false);
                    if (isOn)
                    {
                        ChangeToSpaceMenu(6); //model list as default
                        SetDeleteModeButtonAnimation(); //needs to set animation parameters up front
                    }
                    else
                    {
                        CloseAllSpaceMenus(MySpaceMenus);
                        ResetAllButtonAnimations(MySpaceMenuOptions.transform);
                    }
                }
                else
                {
                    //set space menu for visitor
                    MySpaceMenuOptions.SetActive(false);
                    SpaceMenuOptions.SetActive(isOn);
                    if (isOn)
                    {
                        if (IsThemeTransiting)
                        {
                            CloseAllSpaceMenus(SpaceMenus);
                        }
                        else
                        {
                            ChangeToSpaceMenu(0); // all space as default
                        }

                    }
                    else
                    {
                        CloseAllSpaceMenus(SpaceMenus);
                        ResetAllButtonAnimations(SpaceMenuOptions.transform);
                    }
                }
            }

        }

        public void ChangeToHomeMenu(int state)
        {
            currentMenuState = (MENU_STATE)state;
            CloseAllHomeMenus();
            VRKeyboard.SetActive(false);

            if (currentMenuState == MENU_STATE.CREATE_SPACE || currentMenuState == MENU_STATE.EDIT_SPACE || currentMenuState == MENU_STATE.EDIT_USER_PROFILE)
            {
                VRKeyboard.SetActive(true);
            }

            foreach (MenuLayout menu in HomeMenus)
            {

                if (menu.State == currentMenuState)
                {
                    if (menu.MenuButton != null)
                    {
                        Animator animator = menu.MenuButton.GetComponent<Animator>();
                        animator.ResetTrigger("Normal");
                        animator.SetTrigger("Highlighted");
                        animator.SetBool("Toggled", true);
                    }
                    foreach (GameObject component in menu.Components)
                    {
                        component.SetActive(true);
                    }
                }
                else
                {
                    if (menu.MenuButton != null)
                    {
                        Animator animator = menu.MenuButton.GetComponent<Animator>();
                        animator.ResetTrigger("Highlighted");
                        animator.SetTrigger("Normal");
                        animator.SetBool("Toggled", false);
                    }
                }

                if (IsThemeTransiting &&
                    (menu.State == MENU_STATE.ALL_SPACE || menu.State == MENU_STATE.MY_SPACE))
                {
                    continue;
                }

                if (menu.MenuButton != null)
                {
                    menu.MenuButton.gameObject.SetActive(true);
                }
            }
        }

        public void ChangeToSpaceMenu(int state)
        {
            currentMenuState = (MENU_STATE)state;
            List<MenuLayout> targetMenu = (isEditMode) ? MySpaceMenus : SpaceMenus;
            CloseAllSpaceMenus(targetMenu);
            foreach (MenuLayout menu in targetMenu)
            {
                if (menu.State == currentMenuState)
                {
                    if (menu.MenuButton != null)
                    {
                        Animator animator = menu.MenuButton.GetComponent<Animator>();
                        animator.ResetTrigger("Normal");
                        animator.SetTrigger("Highlighted");
                        animator.SetBool("Toggled", true);
                    }
                    foreach (GameObject component in menu.Components)
                    {
                        component.SetActive(true);
                    }
                }
                else
                {
                    if (menu.MenuButton != null)
                    {
                        Animator animator = menu.MenuButton.GetComponent<Animator>();
                        animator.ResetTrigger("Highlighted");
                        animator.SetTrigger("Normal");
                        animator.SetBool("Toggled", false);
                    }
                }

                if (IsThemeTransiting &&
                    (menu.State == MENU_STATE.ALL_SPACE || menu.State == MENU_STATE.HOME))
                {
                    continue;
                }

                if (menu.MenuButton != null)
                {
                    menu.MenuButton.gameObject.SetActive(true);
                }
            }
        }

        public MENU_STATE GetCurrentMenuState()
        {
            return currentMenuState;
        }
        public void ToggleDeleteMode()
        {
            isToggledDeleteMode = !isToggledDeleteMode;
            if (isToggledDeleteMode)
            {
                gameObject.SetActive(false);
            }
        }

        public void SetDeleteMode(bool isOn)
        {
            isToggledDeleteMode = isOn;
            //if the delete mode is toggle, close the main menu
            if (isToggledDeleteMode)
            {
                gameObject.SetActive(false);
            }
        }

        public void CloseAllHomeMenus()
        {
            foreach (MenuLayout menu in HomeMenus)
            {
                foreach (GameObject component in menu.Components)
                {
                    component.SetActive(false);
                }
                if (menu.MenuButton != null)
                {
                    menu.MenuButton.gameObject.SetActive(false);
                }
            }

        }

        public void CloseAllSpaceMenus(List<MenuLayout> targetMenu)
        {
            foreach (MenuLayout menu in targetMenu)
            {
                foreach (GameObject component in menu.Components)
                {
                    component.SetActive(false);
                }
                if (menu.MenuButton != null)
                {
                    menu.MenuButton.gameObject.SetActive(false);
                }
            }

        }

        public void SetSpaceTabs(bool isOn)
        {
            // we don't allow user to change to another space when a selected space is selected.
            //TODO: remove this function when unload not loaded scene is completed
            IsThemeTransiting = !isOn;
            if (IsHome)
            {
                MenuLayout allSpace = HomeMenus.First(menu => menu.State == MENU_STATE.ALL_SPACE);
                MenuLayout mySpace = HomeMenus.First(menu => menu.State == MENU_STATE.MY_SPACE);
                allSpace.MenuButton.gameObject.SetActive(isOn);
                mySpace.MenuButton.gameObject.SetActive(isOn);
            }
            else
            {
                if (isEditMode)
                {
                    MenuLayout allSpace = MySpaceMenus.First(menu => menu.State == MENU_STATE.ALL_SPACE);
                    MenuLayout home = MySpaceMenus.First(menu => menu.State == MENU_STATE.HOME);
                    allSpace.MenuButton.gameObject.SetActive(isOn);
                    home.MenuButton.gameObject.SetActive(isOn);
                }
                else
                {
                    MenuLayout allSpace = SpaceMenus.First(menu => menu.State == MENU_STATE.ALL_SPACE);
                    MenuLayout home = MySpaceMenus.First(menu => menu.State == MENU_STATE.HOME);
                    allSpace.MenuButton.gameObject.SetActive(isOn);
                    home.MenuButton.gameObject.SetActive(isOn);
                }
            }

        }

        private void ResetAllButtonAnimations(Transform target)
        {
            foreach (Transform option in target)
            {
                Animator animator = option.GetComponent<Animator>();
                if (animator == null || animator == DeleteModeButtonAnim)
                {
                    continue;
                }

                animator.ResetTrigger("Normal");
                animator.ResetTrigger("Highlighted");
                animator.ResetTrigger("Pressed");
                animator.SetBool("Toggled", false);
            }
        }

        private void SetDeleteModeButtonAnimation()
        {
            if (isToggledDeleteMode)
            {
                DeleteModeButtonAnim.ResetTrigger("Normal");
                DeleteModeButtonAnim.ResetTrigger("Pressed");
                DeleteModeButtonAnim.SetTrigger("Highlighted");
                DeleteModeButtonAnim.SetBool("Toggled", true);
            }
            else
            {
                DeleteModeButtonAnim.ResetTrigger("Highlighted");
                DeleteModeButtonAnim.ResetTrigger("Pressed");
                DeleteModeButtonAnim.SetTrigger("Normal");
                DeleteModeButtonAnim.SetBool("Toggled", false);
            }
        }
    }
}