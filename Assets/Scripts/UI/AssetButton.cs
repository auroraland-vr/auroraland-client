using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Nakama;

namespace Auroraland{
	public class AssetButton : MonoBehaviour //,IPointerEnterHandler, IPointerExitHandler
    { 
		public Text AssetName;
		public RawImage AssetImage;
		public Texture DefaultImage;
		public ProgressBar ProgressBar;
		public GameObject Cover;
		public GameObject SpawnButton;
        public Image Frame;

        public INAsset Asset { get; private set; }

		void Awake()
        {
            Cover.SetActive (false);
            Frame.gameObject.SetActive(false);
			SpawnButton.SetActive (false);
			ProgressBar.gameObject.SetActive(false);
		}

        private void Start()
        {
            if(!string.IsNullOrEmpty(Asset.ThumbnailUrl)) StartCoroutine(ImageLoader.LoadImage(AssetImage, DefaultImage, Asset.ThumbnailUrl));
        }

        public void SetAssetInfo(INAsset asset)
        { //this function will be called before Start, after awake
			this.Asset = asset;
			//set ui
			AssetName.text = asset.Name;
            if (AssetBundleDownloader.Instance.GetPrefab(asset.Url, asset.Version) != null) SpawnButton.SetActive(true);
           
		}
			
		public void DownloadAsset()
        { //called when pressed download asset
            if (AssetBundleDownloader.Instance.GetPrefab(Asset.Url, Asset.Version)) return; 
			AssetBundleDownloader.Instance.AddRequest(Asset.Url, Asset.Version);
			StartCoroutine (ShowProgress (Asset.Url));
		}

		public void SpawnAsset()
        {
			SpaceManager.Instance.OnSpawnAssetButtonClicked (Asset);
		}
        

        IEnumerator ShowProgress(string url)
        {
			Cover.SetActive (true);
			SpawnButton.SetActive (false);
			ProgressBar.gameObject.SetActive(true);
			ProgressBar.SetProgress(0);

			while(AssetBundleDownloader.Instance.GetRequestProgress(url) < 1){
				float progress = AssetBundleDownloader.Instance.GetRequestProgress(url);
				ProgressBar.SetProgress(progress);
				yield return null;
			}
			Debug.Log ("Finish Downloading..");
			ProgressBar.SetProgress(1f);
			yield return new WaitForSeconds (0.1f);
			Cover.SetActive (false);
			SpawnButton.SetActive (true);
			ProgressBar.gameObject.SetActive(false);

		}

        public void OnSelect()
        {
            Frame.gameObject.SetActive(true);
            DownloadAsset();
        }
        public void Deselect()
        {
            Frame.gameObject.SetActive(false);
        }
        public void OnPointerEnter(PointerEventData eventData)
        {     
            Frame.gameObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {       
            Frame.gameObject.SetActive(false);
        }
    }
}
