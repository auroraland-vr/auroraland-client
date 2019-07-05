using Nakama;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VRTK;

namespace Auroraland
{
    /// <summary>
    /// Purpose: Control the sequence of downloading theme dlc, loading scene, 
    /// unloading previous scene, loading entities, activating scene and activating entities.
    /// Only use this script in Transit scene.
    /// </summary>
    public class SceneTransitionController : SceneController
    {
        public static event SceneTransitionEvent OnActivateLoadedTheme;
        public delegate void SceneTransitionEvent();
        public TransitDoor TransitionDoor;
        public GameObject SpaceRoot;
        public GameObject HomeRoot;
        public GameObject PlayerRoot;
        public GameObject LocalAvatar;
        public HandController RightController;
        public float DistanceToDoor = 3f;
        [Header("UI")]
        public ProgressBar Progressbar;
        public GameObject ConfirmTransitMenu;
        public Text SpaceText;
        public GameObject ConfirmLogOutMenu;
        public Transform HomeMenu;
        public Transform SpaceMenu;
        public INSpace SelectedSpace { get; set; }
        public bool IsCurrentThemeHome { get; set; }
        private float entitiesDownloadProgress = 0; //the progress of downloading entities from server
        private float themeDownloadProgress = 0; // the progress of downloading scene from server
        private float themeLoadProgress = 0; //the progress of loading scene 
        private bool hasSpaceAssetsList = false;
        private bool hasInitializedSpace = false;
        private bool skipLoadThemeProcess = false;
        bool _hasInitializedAlwaysInFrontCamera;

        protected override void Start()
        {
            base.Start();
            ResetSceneController();
            currentScene = SceneManager.GetActiveScene(); // get transit scene
            StartCoroutine(LoadThemeImmediate(HomeSceneName, false));
            StartCoroutine(TransitSpace(true)); //Transit space immediate
        }

        /*enter space from selecting a space from menu*/
        public void LoadSpace()
        {
            PlayerPrefs.SetString("spaceId", SelectedSpace.Id);
            ChangeTheme();
        }

        /*Go home from space*/
        public override void Home()
        {
            SelectedSpace = null;
            targetThemeName = HomeSceneName;
            ResetSceneController();
            StartCoroutine(LoadTheme(HomeSceneName));
            StartCoroutine(DisplayProgress());
            StartCoroutine(WaitUntilSpaceIsReady());
            RightController.GetComponent<AutoRaycaster>().enabled = true;
            RightController.GetComponent<VRTK_Pointer>().Toggle(false);
            RightController._ikController.isRightHandIKEnabled = false;
        }

        public override void Logout()
        {
            // close the main menu
            if (currentTheme.name == HomeSceneName)
            {
                HomeMenu.gameObject.SetActive(false);
            }
            else
            {
                SpaceMenu.gameObject.SetActive(false);
            }
            // show the door
            TransitionDoor.gameObject.SetActive(true);
            TransitionDoor.SetupDoor();
            SetDoorInFrontOfPlayer();
            ConfirmLogOutMenu.gameObject.SetActive(true);
            ConfirmTransitMenu.gameObject.SetActive(false);
        }
        public void ConfirmLogout(bool confirmed)
        {
            if (confirmed)
            {
                StartCoroutine(TransitLogOut());
            }
            else
            {
                TransitionDoor.gameObject.SetActive(false);
                ConfirmLogOutMenu.gameObject.SetActive(false);
                ConfirmTransitMenu.gameObject.SetActive(false);
            }
        }

        public void ConfirmEnterSpace(bool confirmed)
        {
            if (confirmed)
            {
                if (currentTheme.name != HomeSceneName)
                { //space to space
                    NKController.Instance.LeaveSpace(); // leave space first
                }
                else
                { //home to space
                    StartCoroutine(TransitSpace(false));
                }
            }
        }

        /*callback function when theme was downloaded*/
        protected override void OnDownloadThemeCompleted(string selectedTheme)
        {
            StartCoroutine(LoadTheme(selectedTheme));
        }

        /*callback function when theme was loaded */
        protected override void OnLoadThemeCompleted(string selectedTheme)
        {
            if (selectedTheme != HomeSceneName)
            {
                StartCoroutine(DownloadSpaceAssets(SelectedSpace.Id));
            }
        }

        /*callback function when theme was activated, used for space intialization*/
        protected override void OnThemeActivated(string selectedTheme)
        {
            //set main controllers
            switch (selectedTheme)
            {
                case HomeSceneName:
                    RightController.SetMenu(HomeMenu.gameObject);
                    break;
                case TransitionSceneName:
                    if (SelectedSpace != null && !string.IsNullOrEmpty(SelectedSpace.Url))
                    {
                        RightController.SetMenu(SpaceMenu.gameObject);
                    }
                    else
                    {
                        RightController.SetMenu(HomeMenu.gameObject);
                    }

                    break;
                default: /*load other scene*/
                    RightController.SetMenu(SpaceMenu.gameObject);
                    break;
            }

            if (!string.IsNullOrEmpty(previousTheme.name) && !skipLoadThemeProcess)
            {
                StartCoroutine(UnloadTheme(previousTheme));
            }

            if (selectedTheme != HomeSceneName)
            {
                IsCurrentThemeHome = false;
                HomeRoot.SetActive(false);
                SpaceRoot.SetActive(true);
            }
            else
            {
                IsCurrentThemeHome = true;
                HomeRoot.SetActive(true);
                SpaceRoot.SetActive(false);
            }

            //change scene setting
            AuroralandSceneSettings[] settings = FindObjectsOfType<AuroralandSceneSettings>();
            AuroralandSceneSettings targetSetting = settings.First(setting => setting.gameObject.scene == currentTheme);
            if (targetSetting != null)
            {
                LocalAvatar.transform.position = targetSetting.Spawnpoint.transform.position;
                LocalAvatar.transform.rotation = targetSetting.Spawnpoint.transform.rotation;
            }

            if (!_hasInitializedAlwaysInFrontCamera)
            {
                UICameraClear.Setup();
                _hasInitializedAlwaysInFrontCamera = true;
            }

            hasInitializedSpace = true;
            if (OnActivateLoadedTheme != null)
            {
                OnActivateLoadedTheme();
            }
        }

        private void ResetSceneController()
        {
            hasInitializedSpace = false;
            canActivateTheme = false;
            hasSpaceAssetsList = false;
            skipLoadThemeProcess = false;
        }

        private void ChangeTheme()
        {
            ResetSceneController();
            string themeName = GetThemeName(SelectedSpace.Url);
            targetThemeName = themeName;

            if (targetThemeName == currentTheme.name)
            {
                skipLoadThemeProcess = true;
                OnLoadThemeCompleted(targetThemeName);
            }
            else
            {
                skipLoadThemeProcess = false;
                StartCoroutine(StartDownloadThemeFile(SelectedSpace.Url, SelectedSpace.Version));
            }

            StartCoroutine(DisplayProgress());
            StartCoroutine(WaitUntilSpaceIsReady());
        }

        protected override void OnSessionError(object sender, NKErrorArgs e)
        {
        }
        protected override void OnSessionDisconnect(object sender, NKErrorArgs e)
        {
            SceneManager.LoadScene(EntranceSceneName);
        }
        protected override void OnLeaveSpaceSuccess(object sender, NKSingleArg<bool> success)
        {
            StartCoroutine(TransitSpace(false));
        }

        IEnumerator DownloadSpaceAssets(string spaceId)
        {
            hasSpaceAssetsList = false;

            var message = NSpaceAssetsListMessage.Default(spaceId);
            NKController.Instance.Send(message,
            results =>
            {
                foreach (var asset in results.Results)
                {
                    //TODO this is a temp fix that skip the theme url in asset list
                    if (asset.Type != "theme")
                    {
                        AssetBundleDownloader.Instance.AddRequest(asset.Url, asset.Version);
                    }
                }
                hasSpaceAssetsList = true;
            },
            (INError err) =>
            {
                hasSpaceAssetsList = true;
            });

            yield return new WaitWhile(() => AssetBundleDownloader.Instance.IsDownloading);
        }

        IEnumerator WaitUntilSpaceIsReady()
        {
            string targetThemeName = "";

            if (SelectedSpace != null && !string.IsNullOrEmpty(SelectedSpace.Url))
            {
                SpaceText.text = "Ready to go to " + SelectedSpace.DisplayName + "?";
            }
            else
            {
                SpaceText.text = "Ready to go home?";
            }

            TransitionDoor.gameObject.SetActive(true);
            TransitionDoor.SetupDoor();
            SetDoorInFrontOfPlayer();
            ConfirmTransitMenu.SetActive(false);
            ConfirmLogOutMenu.SetActive(false);
            if (IsCurrentThemeHome)
            {
                HomeMenu.GetComponent<MainMenu>().SetSpaceTabs(false);
            }
            else
            {
                SpaceMenu.GetComponent<MainMenu>().SetSpaceTabs(false);
            }
            yield return new WaitUntil(() => !Progressbar.gameObject.activeInHierarchy);

            ConfirmTransitMenu.SetActive(true);
            ConfirmLogOutMenu.SetActive(false);
        }

        IEnumerator TransitLogOut()
        {
            TransitionDoor.ScreenFade.FadeIn();
            yield return new WaitWhile(() => TransitionDoor.IsAnimating);
            TransitionDoor.gameObject.SetActive(false);
            ConfirmTransitMenu.SetActive(false);
            ConfirmLogOutMenu.SetActive(true);
            NKController.Instance.Logout();
            SceneManager.LoadScene(EntranceSceneName);
        }

        IEnumerator TransitSpace(bool skipEnterTransition = true)
        {
            ConfirmTransitMenu.SetActive(false);
            LocalAvatar.GetComponent<AvatarController>().enabled = false;

            if (!skipEnterTransition)
            {
                TransitionDoor.OpenDoor();
                yield return new WaitWhile(() => TransitionDoor.IsAnimating);
            }
            //activate scene
            canActivateTheme = true; // start finishing downloading process
            if (skipLoadThemeProcess)
            {
                OnThemeActivated(targetThemeName);
            }

            if (SelectedSpace != null && !string.IsNullOrEmpty(SelectedSpace.CreatorId))
            {
                bool isEditMode = (NKController.Instance.GetLocalUserId() == SelectedSpace.CreatorId);
                PlayerPrefs.SetInt("isEditMode", (isEditMode) ? 1 : 0);
            }
            yield return new WaitUntil(() => SceneManager.GetActiveScene() == currentTheme && hasInitializedSpace);

            if (currentTheme.name != HomeSceneName)
            {
                if (currentTheme.name.Contains("casino"))
                {   // TODO: check all of the games with types? instead of string comparison 
                    SpaceManager.Instance.HasBlackJack = true; // for setting xml file
                }

                SpaceManager.Instance.EnterSpace();
                yield return new WaitUntil(() => SpaceManager.Instance.HasSpawnedAllAssets());

            }
            else
            {
                yield return new WaitUntil(() => LocalAvatar.GetComponent<AvatarController>().IsAvatarInitialized());
                LocalAvatar.GetComponent<AvatarController>().enabled = true;
            }
            yield return new WaitForSeconds(1f); // wait a lit bit longer for player to fall to the floor
            SetDoorInBackOfPlayer();
            TransitionDoor.ScreenFade.FadeIn();
            yield return new WaitWhile(() => TransitionDoor.IsAnimating);
            TransitionDoor.gameObject.SetActive(false);

            if (targetThemeName != HomeSceneName)
            {
                RightController.GetComponent<AutoRaycaster>().enabled = false;
                RightController.GetComponent<VRTK_Pointer>().Toggle(true);
                RightController._ikController.isRightHandIKEnabled = true;
            }

            if (IsCurrentThemeHome)
            {
                HomeMenu.GetComponent<MainMenu>().SetSpaceTabs(true);
            }
            else
            {
                SpaceMenu.GetComponent<MainMenu>().SetSpaceTabs(true);
            }
        }

        /**
        * Display the current downloaded progress bar and make the assets visible after every asset is dowloaded from the list
         */
        IEnumerator DisplayProgress()
        {
            Progressbar.gameObject.SetActive(true);
            entitiesDownloadProgress = 0;
            themeDownloadProgress = 0;
            themeLoadProgress = 0;
            int totalTask = (targetThemeName == HomeSceneName || skipLoadThemeProcess) ? 1 : 3;
            int taskIndex = 1;

            if (!skipLoadThemeProcess && targetThemeName != HomeSceneName)
            {
                Progressbar.SetTaskCount(taskIndex++, totalTask);
                Progressbar.SetProgress(0f);
                while (AssetBundleDownloader.Instance.IsDownloading)
                {
                    themeDownloadProgress = AssetBundleDownloader.Instance.TotalDownloadProgress;
                    Progressbar.SetProgress(themeDownloadProgress);
                    yield return new WaitForSeconds(0.1f);
                }

                Progressbar.SetProgress(1f);
                yield return new WaitForSeconds(1f); // display 100% for a bit loger
                yield return new WaitUntil(() => !isDownloadingTheme);
            }

            if (!skipLoadThemeProcess)
            {
                Progressbar.SetTaskCount(taskIndex++, totalTask);
                Progressbar.SetProgress(0f);
                while (themeLoadProgress < 1)
                {
                    themeLoadProgress = LoadSceneProgress;
                    Progressbar.SetProgress(themeLoadProgress);
                    yield return null;
                }
                Progressbar.SetProgress(1f);
                yield return new WaitForSeconds(1f); // display 100% for a bit loger
            }

            if (targetThemeName != HomeSceneName)
            {
                Progressbar.SetTaskCount(taskIndex, totalTask);
                Progressbar.SetProgress(0f);
                yield return new WaitUntil(() => hasSpaceAssetsList);

                while (AssetBundleDownloader.Instance.IsDownloading)
                {
                    entitiesDownloadProgress = AssetBundleDownloader.Instance.TotalDownloadProgress;
                    Progressbar.SetProgress(entitiesDownloadProgress);
                    yield return new WaitForSeconds(0.1f);
                }

                Progressbar.SetProgress(1f);
                yield return new WaitForSeconds(1f); // display 100% for a bit loger
            }

            Progressbar.gameObject.SetActive(false);
        }

        private void SetDoorInFrontOfPlayer()
        {
            Vector3 doorFacingDirection = -Camera.main.transform.forward;
            Vector3 playerPos = LocalAvatar.transform.position;
            Vector3 offset = doorFacingDirection.normalized * DistanceToDoor;
            offset = new Vector3(offset.x, 0, offset.z);
            // put the door in front
            TransitionDoor.transform.position = playerPos - offset;
            Vector3 cameraPos = Camera.main.transform.position;
            cameraPos.y = TransitionDoor.transform.position.y;
            TransitionDoor.transform.LookAt(cameraPos);
            TransitionDoor.transform.Rotate(new Vector3(0, 180f, 0));
        }

        private void SetDoorInBackOfPlayer()
        {
            Vector3 doorFacingDirection = -Camera.main.transform.forward;
            Vector3 playerPos = LocalAvatar.transform.position;
            Vector3 offset = doorFacingDirection.normalized * DistanceToDoor;
            offset = new Vector3(offset.x, 0, offset.z);
            // put the door in at the back
            TransitionDoor.transform.position = playerPos + offset;
            Vector3 cameraPos = Camera.main.transform.position;
            cameraPos.y = TransitionDoor.transform.position.y;
            TransitionDoor.transform.LookAt(cameraPos);
            TransitionDoor.transform.Rotate(new Vector3(0, 180f, 0));
        }
    }
}