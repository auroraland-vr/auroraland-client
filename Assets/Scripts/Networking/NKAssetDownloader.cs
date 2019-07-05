using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

namespace Auroraland
{

    public class NKAssetDownloader : MonoBehaviour
    {
        public event EventHandler<NKSingleArg<GameObject>> DownloadSuccess;
        public event EventHandler<NKSingleArg<GameObject>> DownloadFailure;

        private string url;
        private ulong totalBytes = 0;
        private ulong downloadedBytes = 0;
        private GameObject downloadedPrefab = null;
        private double seconds = 0;
        private UnityWebRequest request;
        private bool downloading = false;

        public NKAssetDownloader(string url)
        {
            this.url = url;
        }

        public GameObject GetPrefab()
        {
            return downloadedPrefab;
        }

        private void Awake()
        {
            StartCoroutine(GetTotalBytes());
        }

        IEnumerator GetTotalBytes()
        {
            UnityWebRequest www = UnityWebRequest.Head(url);
            yield return www.SendWebRequest();
            string contentlength = www.GetResponseHeader("Content-Length");
            Debug.Log("HEAD url: " + url + " size:" + contentlength);
            totalBytes = ulong.Parse(contentlength);
        }

        public void Download()
        {
            StartCoroutine(ProcessRequest(url));
        }

        IEnumerator ProcessRequest(string url)
        {
            var startTime = System.DateTime.Now;
            request = UnityWebRequestAssetBundle.GetAssetBundle(url);
            Debug.Log("start downloading asset bundle: " + url);

            downloading = true;
            yield return request.SendWebRequest();
            downloading = false;

            if (request.isNetworkError || request.isHttpError)
            {
                Debug.LogFormat("Error:{0}, Http Response Code:{1}", request.error, request.responseCode);
            }
            else
            {
                // Get downloaded asset bundle
                AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);

                //use load all asset asymc because some of the asset are huge, and if I called unload(false) afterwards it will cause error
                AssetBundleRequest abr = bundle.LoadAllAssetsAsync<GameObject>();
                yield return request;

                if (request.isDone)
                {
                    UnityEngine.Object[] assets = abr.allAssets;
                    // asset load time testing
                    System.DateTime endTime = System.DateTime.Now;
                    //Debug.LogFormat ("End Loading {0}- End time: {1}", url, endTime);
                    Debug.LogFormat("Loading {0} duration: {1}", url, (endTime - startTime).Duration().TotalSeconds);

                    downloadedPrefab = (GameObject)assets[0];
                    seconds = (endTime - startTime).Duration().TotalSeconds;
                    Debug.LogFormat("Finish Downloading...Loading all assets duration: {0}", (endTime - startTime).Duration().TotalSeconds);
                    bundle.Unload(false);
                    bundle = null;
                }
                else
                {

                }
            }
        }

        IEnumerator UpdateDownloadedBytes()
        {
            while (downloading)
            {
                ulong currentBytes = 0;
                if (request == null) continue;
                currentBytes += request.downloadedBytes;
                downloadedBytes = currentBytes;
                yield return new WaitForSeconds(0.1f);
            }
        }

        public bool IsCompleted()
        {
            return downloading == false;
        }


    }
}