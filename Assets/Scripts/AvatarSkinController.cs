using Nakama;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Auroraland
{
    /// <summary>
    /// Purpose: Receive user selection decision from avatar menu and change model
    /// </summary>
    public class AvatarSkinController : MonoBehaviour
    {
        public static event AvatarMenu.AvatarSkinEvent OnLoadedFirstAvatar;
        public Transform AvatarSpawnRoot;
        public AvatarController AvatarController;
        public GameObject CurrentAvatar { get; set; }
        public bool HasInitialized { get; private set; }

        Dictionary<INAvatar, GameObject> avatarList = new Dictionary<INAvatar, GameObject>();

        void OnEnable()
        {
            AvatarMenu.OnSetAvatar += SetAvatarModel;
            AvatarMenu.OnSetAvatarOptions += SetModelOptions;
        }

        void OnDisable()
        {
            AvatarMenu.OnSetAvatar -= SetAvatarModel;
            AvatarMenu.OnSetAvatarOptions -= SetModelOptions;
        }

        public void SetModelOptions(List<INAvatar> avatars)
        {
            avatarList.Clear();
            HasInitialized = false;
            foreach (INAvatar avatar in avatars)
            {
                StartCoroutine(AddAvatarOptions(avatar));
            }
        }

        public void SetAvatarModel(INAvatar avatar)
        {
            if (CurrentAvatar != null)
            {
                Destroy(CurrentAvatar);
            }
            SpawnAvatar(avatar);
            CurrentAvatar.SetActive(true);
            AvatarController.enabled = true;
            AvatarController.InitializeAvatar(CurrentAvatar);
        }

        public void SetAvatarModel(GameObject avatarPrefab)
        {
            if (CurrentAvatar != null)
            {
                Destroy(CurrentAvatar);
            }

            GameObject avatarModel = Instantiate(avatarPrefab, AvatarSpawnRoot);
            avatarModel.tag = "Untagged";
            avatarModel.SetActive(true);
            CurrentAvatar = avatarModel;
            CurrentAvatar.SetActive(true);
            AvatarController.enabled = true;
            AvatarController.InitializeAvatar(CurrentAvatar);
        }

        IEnumerator AddAvatarOptions(INAvatar avatar)
        {
            GameObject asset = null;
            yield return new WaitUntil(() => AssetBundleDownloader.Instance.GetPrefab(avatar.Url, avatar.Version) != null);
            asset = AssetBundleDownloader.Instance.GetPrefab(avatar.Url, avatar.Version);
            avatarList.Add(avatar, asset);

            if (avatarList.Count != 1)
                yield break;

            SpawnAvatar(avatarList.First().Key);
            CurrentAvatar.SetActive(true);
            AvatarController.enabled = true;
            AvatarController.InitializeAvatar(CurrentAvatar);

            if (OnLoadedFirstAvatar != null)
            {
                OnLoadedFirstAvatar(avatar);
            }

            HasInitialized = true;
        }

        void SpawnAvatar(INAvatar avatar)
        {
            GameObject asset = avatarList[avatar];
            GameObject avatarModel = Instantiate(asset, AvatarSpawnRoot);
            avatarModel.tag = "Untagged";
            avatarModel.SetActive(true);
            CurrentAvatar = avatarModel;
        }
    }
}