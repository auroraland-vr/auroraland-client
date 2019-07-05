using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Auroraland
{
    public class SceneController : MonoBehaviour
    {
        public float LoadSceneProgress { get; private set; }
        protected bool canActivateTheme;
        protected bool isLoading;
        protected bool isDownloadingTheme;
        protected bool isUnloading;
        protected const string HomeSceneName = "Home";
        protected const string EntranceSceneName = "Entrance";
        protected const string TransitionSceneName = "Transit";
        protected string targetThemeName;
        protected Scene targetScene;
        protected Scene currentScene;
        protected Scene currentTheme;
        protected Scene previousTheme;

        protected virtual void Start()
        {
            NKController.Instance.SessionDisconnect += OnSessionDisconnect;
            NKController.Instance.SessionError += OnSessionError;
            NKController.Instance.LeaveSpaceSuccess += OnLeaveSpaceSuccess;
        }

        protected virtual void OnDisable()
        {
            NKController.Instance.SessionDisconnect -= OnSessionDisconnect;
            NKController.Instance.SessionError -= OnSessionError;
            NKController.Instance.LeaveSpaceSuccess -= OnLeaveSpaceSuccess;
        }

        /*Called from home or space*/
        public virtual void Logout()
        {
            NKController.Instance.Logout();
            SceneManager.LoadScene(EntranceSceneName);
        }

        /*Called from Entrance Scene*/
        public virtual void Home()
        {
            SceneManager.LoadScene(TransitionSceneName);
        }

        public string GetThemeName(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return "";
            }

            string theme = System.IO.Path.GetFileNameWithoutExtension(url);
            return theme;
        }

        protected virtual void OnSessionError(object sender, NKErrorArgs e) { }
        protected virtual void OnSessionDisconnect(object sender, NKErrorArgs e) { }
        protected virtual void OnLeaveSpaceSuccess(object sender, NKSingleArg<bool> success) { }
        protected virtual void OnThemeActivated(string selectedTheme) { }
        protected virtual void OnLoadThemeCompleted(string selectedTheme) { }
        protected virtual void OnDownloadThemeCompleted(string selectedTheme) { }

        protected IEnumerator LoadThemeImmediate(string themeName, bool unload)
        {
            SceneManager.LoadScene(themeName, LoadSceneMode.Additive);
            Scene scene = SceneManager.GetSceneByName(themeName);
            targetThemeName = themeName;
            targetScene = scene;
            yield return new WaitUntil(() => SceneManager.GetSceneByName(themeName).isLoaded);
            OnLoadThemeCompleted(themeName);
            previousTheme = currentTheme;
            currentTheme = scene;
            SceneManager.SetActiveScene(currentTheme);
            yield return new WaitUntil(() => SceneManager.GetActiveScene() == currentTheme);
            OnThemeActivated(themeName);
        }

        protected IEnumerator LoadTheme(string themeName)
        {
            canActivateTheme = false;
            targetThemeName = themeName;
            yield return new WaitUntil(() => !isDownloadingTheme);
            AsyncOperation async = SceneManager.LoadSceneAsync(themeName, LoadSceneMode.Additive);
            async.allowSceneActivation = false; // use this for postpone immediate activation
            Scene scene = SceneManager.GetSceneByName(themeName);
            targetScene = scene;

            while (!async.isDone)
            {
                isLoading = true;
                LoadSceneProgress = async.progress / 0.9f;
                yield return null;
                if (async.progress == 0.9f)
                {  // 0.9 is the max number for load scene progress
                    LoadSceneProgress = 1f;
                    OnLoadThemeCompleted(themeName);
                    yield return new WaitUntil(() => canActivateTheme);
                    async.allowSceneActivation = true; //unless allow scene activation is set back to true the async.isDone won't be true
                    yield return null;
                }
            }
            isLoading = false;
            previousTheme = currentTheme;
            currentTheme = scene;
            SceneManager.SetActiveScene(scene);
            yield return new WaitUntil(() => SceneManager.GetActiveScene() == currentTheme);
            OnThemeActivated(themeName);
        }

        protected IEnumerator UnloadTheme(Scene targetScene)
        {
            AsyncOperation async = SceneManager.UnloadSceneAsync(targetScene);
            while (async != null && !async.isDone)
            {
                isUnloading = true;
                yield return null;
            }
            isUnloading = false;
        }

        protected IEnumerator StartDownloadThemeFile(string url, int version)
        {
            isDownloadingTheme = true;
            AssetBundleDownloader.Instance.AddRequest(url, version, true);
            yield return new WaitUntil(() => AssetBundleDownloader.Instance.IsThemeDownloadCompleted(url, version));
            string targetThemeName = GetThemeName(url);
            OnDownloadThemeCompleted(targetThemeName);
            isDownloadingTheme = false;

        }

        IEnumerator StartDownloadFromLocalFile(string fileName) //  example file name: musuem.dlc
        {
            isDownloadingTheme = true;
            var loadRequest = AssetBundle.LoadFromFileAsync(System.IO.Path.Combine(Application.streamingAssetsPath, fileName));
            yield return loadRequest;
            while (!loadRequest.isDone)
            {
                yield return null;
            }
            AssetBundle assetBundle = loadRequest.assetBundle;
            if (assetBundle == null)
            {
                yield break;
            }
            isDownloadingTheme = false;
        }
    }
}
