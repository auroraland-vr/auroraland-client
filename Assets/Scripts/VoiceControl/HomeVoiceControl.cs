using Auroraland.Temporary;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Auroraland
{
    public class HomeVoiceControl : MonoBehaviour
    {
        [Header("Configurations")]
        [SerializeField]
        SceneTransitionController _sceneTransitionController;

        [Header("Targets")]
        public MainMenu MainMenu;
        public AvatarMenu AvatarMenu;
        public SpaceMenu SpaceMenu;
        public SpaceMenu MySpaceMenu;
        public SpaceEditMenu CreateSpacePanel;
        public SpaceEditMenu EditSpacePanel;
        public UserProfileMenu ProfileEditorPanel;
        public VideoReference VideoReference;
        public GameObject ConfirmEnterMenu;
        public GameObject ConfirmLogOutMenu;

        [Header("Main Options")]
        public Button SpaceButton;
        public Button MySpaceButton;
        public Button ProfileButton;
        public Button LogOutButton;

        string currentTarget;
        Dictionary<string, UnityAction> actionDict = new Dictionary<string, UnityAction>();
        Dictionary<string, UnityAction> prevActionDict = new Dictionary<string, UnityAction>();
        Dictionary<string, UnityAction> nextActionDict = new Dictionary<string, UnityAction>();

        void OnEnable()
        {
            SceneTransitionController.OnActivateLoadedTheme += GetCrossSceneReference;
            VoiceCommandRecognizer.OnSaidSystemVoiceCommand += SystemVoiceCommandHandler;
            VoiceCommandRecognizer.OnSaidCommonVoiceCommand += CommonVoiceCommandHandler;
        }
        void OnDisable()
        {
            SceneTransitionController.OnActivateLoadedTheme -= GetCrossSceneReference;
            VoiceCommandRecognizer.OnSaidSystemVoiceCommand -= SystemVoiceCommandHandler;
            VoiceCommandRecognizer.OnSaidCommonVoiceCommand -= CommonVoiceCommandHandler;
        }
        void OnDestroy()
        {
            SceneTransitionController.OnActivateLoadedTheme -= GetCrossSceneReference;
            VoiceCommandRecognizer.OnSaidSystemVoiceCommand -= SystemVoiceCommandHandler;
            VoiceCommandRecognizer.OnSaidCommonVoiceCommand -= CommonVoiceCommandHandler;
        }

        void Start()
        {
            Assert.IsNotNull(_sceneTransitionController);

            actionDict.Add("logout", LogOut);
            actionDict.Add("save", Save);
            actionDict.Add("edit", Edit);
            actionDict.Add("exit", Exit);
            actionDict.Add("stop", Stop);
            actionDict.Add("start", Play);
            actionDict.Add("cancel", Cancel);
            actionDict.Add("next", Next);
            actionDict.Add("previous", Previous);
            actionDict.Add("load", LoadSpace);
            actionDict.Add("enter", EnterSpace);
            actionDict.Add("create", CreateSpace);
            actionDict.Add("delete", DeleteSpace);
            actionDict.Add("yes", Confirm);
            actionDict.Add("no", Deny);
            nextActionDict.Add("avatar", SelectNextAvatar);
            nextActionDict.Add("space", SelectNextSpace);
            nextActionDict.Add("my space", SelectNextSpace);
            nextActionDict.Add("theme", SelectNextTheme);
            prevActionDict.Add("avatar", SelectPreviousAvatar);
            prevActionDict.Add("space", SelectPreviousSpace);
            prevActionDict.Add("my space", SelectPreviousSpace);
            prevActionDict.Add("theme", SelectPreviousTheme);
        }

        // HACK Won't work in future versions of Unity -- cross-scene references are soon to be prohibited and enforced by Unity itself
        void GetCrossSceneReference()
        {
            AvatarMenu = FindObjectOfType<AvatarMenu>();
            VideoReference = FindObjectOfType<VideoReference>();
        }

        void SystemVoiceCommandHandler(VoiceCommandArgs args)
        {
            var target = args.Args.Target;
            var action = args.Args.Action;
            if (string.IsNullOrEmpty(target) && !string.IsNullOrEmpty(action))
            {// only action
                if (actionDict.ContainsKey(action))
                {
                    actionDict[action].Invoke();
                }
            }
            else if (!string.IsNullOrEmpty(target) && string.IsNullOrEmpty(action))
            {
                //only target
                currentTarget = target;
                switch (currentTarget)
                {
                    case "menu":
                        OpenMainMenu();
                        break;
                    case "space":
                        ChangeTab(MainMenu.MENU_STATE.ALL_SPACE);
                        break;
                    case "my space":
                        ChangeTab(MainMenu.MENU_STATE.MY_SPACE);
                        break;
                    case "profile":
                        ChangeTab(MainMenu.MENU_STATE.USER_PROFILE);
                        break;
                }
            }
            else if (!string.IsNullOrEmpty(target) && !string.IsNullOrEmpty(action))
            {//take target and action 
                currentTarget = target;
                if ((target == "space" || target == "my space") && action == "enter")
                {
                    EnterSpace();
                }
                else if ((target == "space" || target == "my space") && action == "load")
                {
                    LoadSpace();
                }
                else if (target == "space" && action == "create")
                {
                    CreateSpace();
                }
                else if (target == "space" && action == "delete")
                {
                    DeleteSpace();
                }
                else if (target == "menu" && action == "open")
                {
                    OpenMainMenu();
                }
            }

        }
        void CommonVoiceCommandHandler(VoiceCommandArgs args)
        {
            var target = args.Args.Target;
            var action = args.Args.Action;
            if (string.IsNullOrEmpty(target) && !string.IsNullOrEmpty(action))
            {// only action
                if (actionDict.ContainsKey(action))
                {
                    actionDict[action].Invoke();
                }
            }
            else if (!string.IsNullOrEmpty(target) && !string.IsNullOrEmpty(action))
            {//has target and action 
                currentTarget = target;
                if (action == "next" || action == "previous" || action == "exit" || action == "start" || action == "stop")
                {
                    actionDict[action].Invoke();
                }
            }
        }

        #region Level1 system commands
        private void LogOut()
        {
            _sceneTransitionController.Logout();
        }
        private void OpenMainMenu()
        {
            currentTarget = "space";
            MainMenu.gameObject.SetActive(true);
        }

        private void CloseMainMenu()
        {
            currentTarget = "";
            MainMenu.gameObject.SetActive(false);
        }
        #endregion

        #region Level2 system commands
        private void ChangeTab(string currentTarget)
        {
            switch (currentTarget)
            {
                case "space":
                    SpaceButton.onClick.Invoke();
                    break;
                case "logout":
                    LogOutButton.onClick.Invoke();
                    break;
                case "my space":
                    MySpaceButton.onClick.Invoke();
                    break;
                case "profile":
                    ProfileButton.onClick.Invoke();
                    break;
            }
        }
        private void ChangeTab(MainMenu.MENU_STATE state)
        {
            MainMenu.ChangeToHomeMenu((int)state);
        }
        private void SetTarget(string targetName)
        {
            currentTarget = targetName;
        }
        #endregion

        #region Level3 common commands
        private void Confirm()
        {
            if (ConfirmLogOutMenu.activeInHierarchy)
            {
                _sceneTransitionController.ConfirmLogout(true);
            }
            else if (ConfirmEnterMenu.activeInHierarchy)
            {
                _sceneTransitionController.ConfirmEnterSpace(true);
            }
        }

        private void Deny()
        {
            if (ConfirmEnterMenu.gameObject.activeInHierarchy)
            {
                _sceneTransitionController.ConfirmEnterSpace(false);
            }
            else if (ConfirmLogOutMenu.gameObject.activeInHierarchy)
            {
                _sceneTransitionController.ConfirmLogout(false);
            }
        }

        private void Next()
        {
            switch (MainMenu.GetCurrentMenuState())
            {
                case MainMenu.MENU_STATE.MY_SPACE: currentTarget = "my space"; break;
                case MainMenu.MENU_STATE.ALL_SPACE: currentTarget = "space"; break;
                case MainMenu.MENU_STATE.CREATE_SPACE: currentTarget = "theme"; break;
                default: currentTarget = "avatar"; break; //TODO: need to check user's view
            }
            if (nextActionDict.ContainsKey(currentTarget))
            {
                nextActionDict[currentTarget].Invoke();
            }
        }

        private void Previous()
        {
            switch (MainMenu.GetCurrentMenuState())
            {
                case MainMenu.MENU_STATE.MY_SPACE: currentTarget = "my space"; break;
                case MainMenu.MENU_STATE.ALL_SPACE: currentTarget = "space"; break;
                case MainMenu.MENU_STATE.CREATE_SPACE: currentTarget = "theme"; break;
                default: currentTarget = "avatar"; break; //TODO: need to check user's view
            }
            if (prevActionDict.ContainsKey(currentTarget))
            {
                prevActionDict[currentTarget].Invoke();
            }
        }

        private void Play() /*Equals to Start Command*/
        {
            if (currentTarget == "video")
            {
                VideoReference.TurnOn();
            }
        }

        private void Stop() /*Equals to Stop Command*/
        {
            if (currentTarget == "video")
            {
                VideoReference.TurnOff();
            }
        }

        private void Exit() /*Equals to Close Command*/
        {
            CloseMainMenu();
        }

        private void Cancel()
        {
            if (MainMenu.GetCurrentMenuState() == MainMenu.MENU_STATE.EDIT_USER_PROFILE)
            {
                ChangeTab(MainMenu.MENU_STATE.USER_PROFILE);
            }
            else if (MainMenu.GetCurrentMenuState() == MainMenu.MENU_STATE.CREATE_SPACE)
            {
                ChangeTab(MainMenu.MENU_STATE.MY_SPACE);
            }
            else if (MainMenu.GetCurrentMenuState() == MainMenu.MENU_STATE.EDIT_SPACE)
            {
                ChangeTab(MainMenu.MENU_STATE.MY_SPACE);
            }
            else if (ConfirmEnterMenu.gameObject.activeInHierarchy)
            {
                _sceneTransitionController.ConfirmEnterSpace(false);
            }
            else if (ConfirmLogOutMenu.gameObject.activeInHierarchy)
            {
                _sceneTransitionController.ConfirmLogout(false);
            }
        }
        private void Edit()
        {
            if (MainMenu.GetCurrentMenuState() == MainMenu.MENU_STATE.USER_PROFILE)
            {
                ChangeTab(MainMenu.MENU_STATE.EDIT_USER_PROFILE);
            }
            else if (MainMenu.GetCurrentMenuState() == MainMenu.MENU_STATE.MY_SPACE)
            {
                ChangeTab(MainMenu.MENU_STATE.EDIT_SPACE);
            }
        }

        private void Save()
        {
            if (MainMenu.GetCurrentMenuState() == MainMenu.MENU_STATE.EDIT_USER_PROFILE)
            {
                ProfileEditorPanel.SaveUserProfile();
                ChangeTab(MainMenu.MENU_STATE.USER_PROFILE);
            }
            else if (MainMenu.GetCurrentMenuState() == MainMenu.MENU_STATE.EDIT_SPACE)
            {
                EditSpacePanel.UpdateSpace();
            }
        }

        private void SelectNextAvatar()
        {
            AvatarMenu.SelectNextAvatar();
        }

        private void SelectPreviousAvatar()
        {
            AvatarMenu.SelectPreviousAvatar();
        }

        private void SelectNextTheme()
        {
            CreateSpacePanel.SelectNextTheme();
        }

        private void SelectPreviousTheme()
        {
            CreateSpacePanel.SelectPreviousTheme();
        }
        private void SelectNextSpace()
        {
            if (MainMenu.GetCurrentMenuState() == MainMenu.MENU_STATE.ALL_SPACE)
            {
                SpaceMenu.SelectNextSpace();
            }
            else if (MainMenu.GetCurrentMenuState() == MainMenu.MENU_STATE.MY_SPACE)
            {
                MySpaceMenu.SelectNextSpace();
            }
        }

        private void SelectPreviousSpace()
        {
            if (MainMenu.GetCurrentMenuState() == MainMenu.MENU_STATE.ALL_SPACE)
            {
                SpaceMenu.SelectPreviousSpace();
            }
            else if (MainMenu.GetCurrentMenuState() == MainMenu.MENU_STATE.MY_SPACE)
            {
                MySpaceMenu.SelectPreviousSpace();
            }
        }
        #endregion

        private void LoadSpace()
        {
            if (!MainMenu.gameObject.activeSelf || MainMenu.IsThemeTransiting) return;
            _sceneTransitionController.LoadSpace();
            CloseMainMenu();
        }


        private void EnterSpace()
        {
            if (!ConfirmEnterMenu.gameObject.activeInHierarchy)
            {
                return; // can't enter space if you are still loading
            }

            _sceneTransitionController.ConfirmEnterSpace(true);
        }

        private void DeleteSpace()
        {
            if (MainMenu.GetCurrentMenuState() == MainMenu.MENU_STATE.EDIT_SPACE)
            {
                EditSpacePanel.DeleteSpace();
            }
        }

        private void CreateSpace()
        {
            if (MainMenu.GetCurrentMenuState() == MainMenu.MENU_STATE.MY_SPACE)
            {
                ChangeTab(MainMenu.MENU_STATE.CREATE_SPACE);
            }
            else if (MainMenu.GetCurrentMenuState() == MainMenu.MENU_STATE.CREATE_SPACE)
            {
                CreateSpacePanel.CreateSpace();
            }
        }
    }
}