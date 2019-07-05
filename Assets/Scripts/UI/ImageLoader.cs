using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

namespace Auroraland{
	public static class ImageLoader{

		public static IEnumerator LoadImage(RawImage TargetImage, Texture DefaultImage, string TargetUrl){

			TargetImage.texture = DefaultImage;

			if (string.IsNullOrEmpty (TargetUrl) || TargetUrl == "none" || TargetUrl =="http://") {
				yield break;
			}

			UnityWebRequest www = UnityWebRequestTexture.GetTexture(TargetUrl);

			yield return www.SendWebRequest ();

			if (www.isNetworkError || www.isHttpError) {
				Debug.Log (www.error);
			} 
			else {
				Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
				TargetImage.texture = myTexture;
			}
		}
	}
}
