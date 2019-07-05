using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Auroraland
{
    public class SpaceVoiceControl : MonoBehaviour
    {
        [Header("Configurations")]
        [SerializeField]
        SceneTransitionController _sceneTransitionController;

        [Header("Targets")]
        public MainMenu MainMenu;
        public AssetMenu ModelMenu;
        public AssetMenu GameMenu;
        public SpaceMenu SpaceMenu;
        public Transform CategoryRoot;
        public HandController RightController;
        public GameObject ConfirmEnterMenu;
        public GameObject ConfirmLogOutMenu;

        [Header("Main Options")]
        public Button GeneralHomeButton; // home button from space panel 
        public Button GeneralSpaceButton; //space button from space panel 
        public Button HomeButton; // home button from my space panel 
        public Button SpaceButton; // space button from my space panel
        public Button ModelButton;// model button from my space panel
        public Button GameButton;// game button from my space panel
        public Button DeleteModeButton;// delete button from my space panel

        string currentTarget;

        Dictionary<string, UnityAction> actionDict = new Dictionary<string, UnityAction>();
        Dictionary<string, UnityAction> prevActionDict = new Dictionary<string, UnityAction>();
        Dictionary<string, UnityAction> nextActionDict = new Dictionary<string, UnityAction>();
        Dictionary<string, TabButton> modelTabs = new Dictionary<string, TabButton>();

        void OnEnable()
        {
            VoiceCommandRecognizer.OnSaidSystemVoiceCommand += SystemVoiceCommandHandler;
            VoiceCommandRecognizer.OnSaidCommonVoiceCommand += CommonVoiceCommandHandler;
        }
        void OnDisable()
        {
            VoiceCommandRecognizer.OnSaidSystemVoiceCommand -= SystemVoiceCommandHandler;
            VoiceCommandRecognizer.OnSaidCommonVoiceCommand -= CommonVoiceCommandHandler;
        }
        void OnDestroy()
        {
            VoiceCommandRecognizer.OnSaidSystemVoiceCommand -= SystemVoiceCommandHandler;
            VoiceCommandRecognizer.OnSaidCommonVoiceCommand -= CommonVoiceCommandHandler;
        }

        void Start()
        {
            Assert.IsNotNull(_sceneTransitionController);

            actionDict.Add("logout", LogOut);
            actionDict.Add("exit", Exit);
            actionDict.Add("start", Play);
            actionDict.Add("next", Next);
            actionDict.Add("load", LoadSpace);
            actionDict.Add("enter", EnterSpace);
            actionDict.Add("previous", Previous);
            actionDict.Add("cancel", Cancel);
            actionDict.Add("spawn", SpawnAsset);
            actionDict.Add("delete", Delete);
            actionDict.Add("delete mode", ToggleDeleteMode);
            actionDict.Add("bigger", Bigger);
            actionDict.Add("smaller", Smaller);
            actionDict.Add("yes", Confirm);
            actionDict.Add("no", Deny);
            nextActionDict.Add("space", SelectNextSpace);
            nextActionDict.Add("game", SelectNextAsset);
            nextActionDict.Add("model", SelectNextAsset);
            prevActionDict.Add("space", SelectPreviousSpace);
            prevActionDict.Add("game", SelectPreviousAsset);
            prevActionDict.Add("model", SelectPreviousAsset);

            foreach (TabButton tab in CategoryRoot.GetComponentsInChildren<TabButton>())
            {
                modelTabs.Add(tab.Name, tab);
            }
        }

        void SystemVoiceCommandHandler(VoiceCommandArgs args)
        {
            Debug.Log(args.Args);
            var target = args.Args.Target;
            var action = args.Args.Action;
            var direction = args.Args.Direction;
            if (!string.IsNullOrEmpty(action) && string.IsNullOrEmpty(direction) && string.IsNullOrEmpty(target))
            {// only action
                if (actionDict.ContainsKey(action))
                {
                    actionDict[action].Invoke();
                }
                else if (action == "rotate")
                {
                    Rotate("right");
                }
            }
            else if (!string.IsNullOrEmpty(target) && string.IsNullOrEmpty(action) && string.IsNullOrEmpty(direction))
            {
                currentTarget = target;
                switch (currentTarget)
                {
                    case "menu":
                        OpenMainMenu();
                        break;
                    case "space":
                    case "model":
                    case "game":
                    case "home":
                    case "delete":
                        ChangeTab(currentTarget);
                        break;
                    case "all":
                    case "furniture":
                    case "art":
                    case "plants":
                    case "others":
                        ChangeModelTab(currentTarget);
                        break;
                }
            }
            else if (!string.IsNullOrEmpty(action) && !string.IsNullOrEmpty(direction))
            {// take action and direction
                if (action == "rotate")
                {
                    Rotate(direction);
                }
            }
            else if (!string.IsNullOrEmpty(action) && !string.IsNullOrEmpty(target))
            {// take target and action
                if ((target == "space" || target == "my space") && action == "enter")
                {
                    EnterSpace();
                }
                else if ((target == "space" || target == "my space") && action == "load")
                {
                    LoadSpace();
                }
                else if (target == "home" && action == "enter")
                {
                    Home();
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
            if (string.IsNullOrEmpty(target) && !string.IsNullOrEmpty(action) && actionDict.ContainsKey(action))
            {
                actionDict[action].Invoke();
            }
            else if (!string.IsNullOrEmpty(target) && !string.IsNullOrEmpty(action))
            {//has target and action 
                currentTarget = target;
                if (action == "next" || action == "previous" || action == "exit" || action == "start")
                {
                    actionDict[action].Invoke();
                }
            }
        }

        #region Level1 system commands
        private void Home()
        {
            if (MainMenu.IsThemeTransiting)
            {
                return;
            }

            _sceneTransitionController.Home();
            CloseMainMenu();
        }

        private void LogOut()
        {
            _sceneTransitionController.Logout();
        }

        private void OpenMainMenu()
        {
            currentTarget = "model";
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
                {
                    if (PlayerPrefs.GetInt("isEditMode") == 1)
                    {
                        SpaceButton.onClick.Invoke();
                    }
                    else
                    {
                        GeneralSpaceButton.onClick.Invoke();
                    }

                    break;
                }
                case "home":
                {
                    if (PlayerPrefs.GetInt("isEditMode") == 1)
                    {
                        HomeButton.onClick.Invoke();
                    }
                    else
                    {
                        GeneralHomeButton.onClick.Invoke();
                    }

                    break;
                }
                case "model":
                    ModelButton.onClick.Invoke();
                    break;
                case "game":
                    GameButton.onClick.Invoke();
                    break;
                case "delete":
                    DeleteModeButton.onClick.Invoke();
                    break;
            }
        }

        private void SetTarget(string targetName)
        {
            currentTarget = targetName;
        }
        #endregion

        #region Level3 common commands
        private void Next()
        {
            switch (MainMenu.GetCurrentMenuState())
            {
                case MainMenu.MENU_STATE.MODEL_LIST: currentTarget = "model"; break;
                case MainMenu.MENU_STATE.GAME_LIST: currentTarget = "game"; break;
                case MainMenu.MENU_STATE.ALL_SPACE: currentTarget = "space"; break;
                default: return;
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
                case MainMenu.MENU_STATE.MODEL_LIST: currentTarget = "model"; break;
                case MainMenu.MENU_STATE.GAME_LIST: currentTarget = "game"; break;
                case MainMenu.MENU_STATE.ALL_SPACE: currentTarget = "space"; break;
                default: return;
            }
            if (prevActionDict.ContainsKey(currentTarget))
            {
                prevActionDict[currentTarget].Invoke();
            }
        }

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

        private void Play() /*Equals to Start Command*/
        {
            //Undefined
        }

        private void Exit() /*Equals to Close Command*/
        {
            CloseMainMenu();
        }

        private void Cancel()
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
        private void Edit() /*Equals to config Command*/
        {
            //Undefined
        }

        private void Save()
        {
            //Undefined
        }

        private void SelectNextSpace()
        {
            SpaceMenu.SelectNextSpace();
        }

        private void SelectPreviousSpace()
        {
            SpaceMenu.SelectPreviousSpace();
        }

        private void SelectNextAsset()
        {
            if (MainMenu.GetCurrentMenuState() == MainMenu.MENU_STATE.MODEL_LIST)
            {
                ModelMenu.SelectNextAsset();
            }
            else if (MainMenu.GetCurrentMenuState() == MainMenu.MENU_STATE.GAME_LIST)
            {
                GameMenu.SelectNextAsset();
            }
        }

        private void SelectPreviousAsset()
        {
            if (MainMenu.GetCurrentMenuState() == MainMenu.MENU_STATE.MODEL_LIST)
            {
                ModelMenu.SelectPreviousAsset();
            }
            else if (MainMenu.GetCurrentMenuState() == MainMenu.MENU_STATE.GAME_LIST)
            {
                GameMenu.SelectPreviousAsset();
            }
        }
        #endregion

        void LoadSpace()
        {
            if (MainMenu.gameObject.activeSelf && !MainMenu.IsThemeTransiting)
            {
                _sceneTransitionController.LoadSpace();
                CloseMainMenu();
            }
        }

        void EnterSpace()
        {
            if (!ConfirmEnterMenu.gameObject.activeInHierarchy)
            {
                return; // can't enter space if you are still loading
            }

            _sceneTransitionController.ConfirmEnterSpace(true);
        }

        void SpawnAsset()
        {
            if (MainMenu.GetCurrentMenuState() == MainMenu.MENU_STATE.MODEL_LIST)
            {
                ModelMenu.SelectedAssetButton.SpawnAsset();
                CloseMainMenu();
            }
            else if (MainMenu.GetCurrentMenuState() == MainMenu.MENU_STATE.GAME_LIST)
            {
                GameMenu.SelectedAssetButton.SpawnAsset();
                CloseMainMenu();
            }
        }
        void ToggleDeleteMode()
        {
            DeleteModeButton.onClick.Invoke();
        }

        void Delete()
        {
            if (RightController.GetComponent<VRTK.VRTK_InteractGrab>().GetGrabbedObject() == null)
            {
                return;
            }

            GameObject targetObject = RightController.GetComponent<VRTK.VRTK_InteractGrab>().GetGrabbedObject();
            targetObject.GetComponent<InteractableAsset>().DeleteAsset();
        }
        void Rotate(string direction)
        {
            ObjectManipulator.Direction dir = ObjectManipulator.Direction.Right;
            switch (direction)
            {
                case "clockwise":
                    dir = ObjectManipulator.Direction.Clockwise;
                    break;
                case "counter clockwise":
                    dir = ObjectManipulator.Direction.CounterClockwise;
                    break;
                case "backward":
                    dir = ObjectManipulator.Direction.Backward;
                    break;
                case "forward":
                    dir = ObjectManipulator.Direction.Forward;
                    break;
                case "left":
                    dir = ObjectManipulator.Direction.Left;
                    break;
                case "right":
                    dir = ObjectManipulator.Direction.Right;
                    break;

            }
            RightController.ObjectManipulator.DoRotate(dir);

        }
        void Bigger()
        {
            RightController.ObjectManipulator.DoScale(ObjectManipulator.Direction.Up);
        }
        void Smaller()
        {
            RightController.ObjectManipulator.DoScale(ObjectManipulator.Direction.Down);
        }

        void ChangeModelTab(string category)
        {
            modelTabs[category].ToggledButton();
            modelTabs[category].OnTabClick.Invoke(modelTabs[category]);
        }
    }
}