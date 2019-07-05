using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Nakama;

namespace Auroraland{

    public class ThemeButton : MonoBehaviour {
        public Image Frame;
        public RawImage ThemeImage;
        public Texture LoadingImage;
        public Text ThemeDisplayName;
        public string ThemeName { private set; get; }
        public string DefaultImageUrl { private set; get; }
        public string AssetId { private set; get; }
        public INAsset Theme{ private set; get; }
        // Use this for initialization
        public void SetTheme(INAsset theme)
        {
            ThemeName = theme.Name;
            Theme = theme;
            AssetId = theme.Id;
            DefaultImageUrl = theme.ThumbnailUrl;
            StartCoroutine(ImageLoader.LoadImage(ThemeImage, LoadingImage, DefaultImageUrl));
            ThemeDisplayName.text = ThemeName;
        }
		public void Select(bool select){
			Frame.gameObject.SetActive(select);
		}
    }
}
