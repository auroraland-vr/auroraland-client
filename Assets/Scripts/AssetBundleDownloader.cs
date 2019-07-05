using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Auroraland
{
    public class AssetBundleDownloader : MonoBehaviour
    {
        public static AssetBundleDownloader Instance;
        public int DownloaderMaximum;
        public bool IsDownloading { get; private set; }
        public float TotalDownloadProgress { get; private set; }
        public ulong EstimatedTotalBytes { get; private set; }
        public ulong DownloadedBytes { get; private set; }

        private int downloaderCount;
        private readonly UnityEngine.Object downloaderLock = new UnityEngine.Object();
        private Dictionary<string, GameObject> prefabDict = new Dictionary<string, GameObject>();
        private Dictionary<string, AssetBundle> sceneDict = new Dictionary<string, AssetBundle>(); //has to store asset bundle, otherwise unity won't find loaded assetbundle
        private Dictionary<string, int> themeDict = new Dictionary<string, int>();//store url and version pair that has been downloaded
        private Dictionary<string, WWW> reqDict = new Dictionary<string, WWW>();

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
        }

        void OnLevelWasLoaded(int level) // a new scene was loaded
        {
            if (reqDict.Count <= 0)
                return;

            lock (downloaderLock)
            {
                StopAllCoroutines();
                ClearDownloadHandler();
                TotalDownloadProgress = 0;
                EstimatedTotalBytes = 0;
            }
        }

        public GameObject GetPrefab(string url, int version)
        {
            if (!prefabDict.ContainsKey(url))
            {
                return null; //does not have the asset
            }

            if (!Caching.IsVersionCached(url, version))
            {
                return null;
            }

            return prefabDict[url];
        }

        public bool IsThemeDownloadCompleted(string url, int version)
        {
            if (!themeDict.ContainsKey(url))
            {
                return false; //does not have the asset
            }

            return themeDict[url] == version;
        }

        public float GetRequestProgress(string url)
        {

            if (reqDict.ContainsKey(url) && reqDict[url] != null) //if request list has the request
            {
                return reqDict[url].progress;
            }

            if (prefabDict.ContainsKey(url) || themeDict.ContainsKey(url))
            {
                return 1; // if it's a completed request
            }

            return 0;
        }

        public void AddRequest(string url, int version, bool isScene = false)
        {
            if (reqDict.ContainsKey(url))
            {
                return; //don't add duplicate request
            }

            if (url.StartsWith("http"))
            {
                StartCoroutine(ProcessRequest(url, version, isScene));
            }
        }

        IEnumerator ProcessRequest(string url, int version, bool isScene)
        {
            IsDownloading = true;

            if (reqDict.ContainsKey(url))
            {
                yield break;
            }

            DateTime startTime = DateTime.Now;
            WWW www = WWW.LoadFromCacheOrDownload(url, version);
            reqDict.Add(url, www);

            if (reqDict.Count == 1)
            { // first request
                StartCoroutine(UpdateProgress());
            }

            yield return new WaitUntil(() => downloaderCount < DownloaderMaximum);

            lock (downloaderLock)
            {
                downloaderCount++;
            }

            yield return www;

            if (!www.isDone)
                yield break;

            reqDict.Remove(url);

            if (isScene) // track scene
            {
                lock (downloaderLock)
                {
                    downloaderCount--;
                    if (themeDict.ContainsKey(url))
                    {
                        themeDict[url] = version;
                        sceneDict[url].Unload(false);
                        sceneDict[url] = www.assetBundle;
                    }
                    else
                    {
                        themeDict.Add(url, version);
                        sceneDict.Add(url, www.assetBundle);
                    }
                }
            }
            else
            {   //track prefab 
                AssetBundle bundle = www.assetBundle;
                GameObject[] assets = bundle.LoadAllAssets<GameObject>();
                lock (downloaderLock)
                {
                    downloaderCount--;
                    if (prefabDict.ContainsKey(url))
                    {
                        prefabDict[url] = assets[0];
                    }
                    else
                    {
                        prefabDict.Add(url, assets[0]);
                    }
                }
                yield return new WaitForSeconds(1f); //this is a known issue, asset can't be unload in same frame TODO: check future unity update
                bundle.Unload(false);
            }

            if (downloaderCount == 0)
            {
                ClearDownloadHandler();
            }

        }

        IEnumerator UpdateProgress()
        {
            while (IsDownloading)
            {
                ulong downloadedBytes = 0;
                ulong totalBytes = 0;
                decimal progress = 0;

                foreach (WWW www in reqDict.Values)
                {
                    if (www == null)
                    {
                        continue;
                    }

                    downloadedBytes += (ulong)www.bytesDownloaded;
                    if (www.progress > 0)
                    {
                        totalBytes += (ulong)Math.Floor(www.bytesDownloaded / www.progress);
                    }
                }
                DownloadedBytes = downloadedBytes;

                if (totalBytes != 0)
                {
                    progress = decimal.Divide(downloadedBytes, totalBytes);
                }
                TotalDownloadProgress = (float)progress;

                yield return new WaitForSeconds(0.1f);
            }
        }

        public bool HasCompletedDownloading()
        {
            if (reqDict.Count == 0)
            {
                return true;
            }
            return false;
        }

        private void ClearDownloadHandler()
        {
            foreach (WWW www in reqDict.Values)
            {
                www.Dispose();
            }
            reqDict.Clear();
            downloaderCount = 0;
            IsDownloading = false;
        }
    }
}