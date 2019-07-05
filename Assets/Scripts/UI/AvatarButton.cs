using Nakama;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Auroraland
{
    public class AvatarButton : MonoBehaviour
    {
        public Text AvatarName;
        public Image Frame;
        public RawImage Thumbnail;
        public Texture DefaultAvatarImage;
        public ProgressBar ProgressBar;
        public GameObject Cover;
        INAvatar avatar;

        public void UpdateAvatarData(INAvatar avatar)
        {
            this.avatar = avatar;
            string avatarName = System.IO.Path.GetFileNameWithoutExtension(avatar.Url);
            if (avatarName != null)
            {
                avatarName = avatarName.Replace("character_", "");

                if (avatarName.Length > 1)
                    avatarName = char.ToUpper(avatarName[0]) + avatarName.Substring(1);

                AvatarName.text = avatarName;
            }

            StartCoroutine(ImageLoader.LoadImage(Thumbnail, DefaultAvatarImage, avatar.ThumbnailUrl));
        }

        public INAvatar GetAvatarData()
        {
            return avatar;
        }

        public void Select(bool select)
        {
            Frame.gameObject.SetActive(select);
        }

        public void DownloadAvatar()
        { //called when pressed download asset
            AssetBundleDownloader.Instance.AddRequest(avatar.Url, avatar.Version);
            StartCoroutine(ShowProgress(avatar.Url));
        }

        IEnumerator ShowProgress(string url)
        {
            Cover.SetActive(true);
            ProgressBar.gameObject.SetActive(true);
            ProgressBar.SetProgress(0);

            while (AssetBundleDownloader.Instance.GetRequestProgress(url) < 1)
            {
                float progress = AssetBundleDownloader.Instance.GetRequestProgress(url);
                ProgressBar.SetProgress(progress);
                yield return null;
            }

            ProgressBar.SetProgress(1f);
            yield return new WaitForSeconds(0.1f);
            Cover.SetActive(false);
            ProgressBar.gameObject.SetActive(false);
        }
    }
}
