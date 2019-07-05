using Nakama;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Auroraland
{
    public class AvatarMenu : MonoBehaviour
    {
        //Events
        public static event AvatarSkinEvent OnSetAvatar;
        public delegate void AvatarSkinEvent(INAvatar avatar);
        public static event AvatarSkinOptionEvent OnSetAvatarOptions;
        public delegate void AvatarSkinOptionEvent(List<INAvatar> options);

        [Header("UI")]
        public ScrollRectSetter ScrollRect;
        public Transform AvatarButtonPanel;
        public GameObject AvatarButtonPrefab;

        private AvatarButton selectedBtn;
        private INAvatar selectedAvatar;
        private Dictionary<INAvatar, GameObject> avatarButtonDict = new Dictionary<INAvatar, GameObject>();

        void Start()
        {
            NKController.Instance.LoadAvatarList();
            NKController.Instance.LoadAvatarListSuccess += ListAvatars;
            AvatarSkinController.OnLoadedFirstAvatar += SetDefaultAvatar;
            StartCoroutine(FixNonVRScrolling());
        }

        /// <summary>
        /// Quick fix to enable scrolling of avatar list by NonVR player
        /// </summary>
        IEnumerator FixNonVRScrolling()
        {
            while (SDK_SetupMode.Instance.CurrentSDKName == null)
            {
                yield return null; // Wait for SDK_SetupMode to initialize
            }

            if (string.Equals(SDK_SetupMode.Instance.CurrentSDKName, "NonVR"))
            {
                gameObject.GetComponent<UnityEngine.UI.GraphicRaycaster>().blockingObjects =
                    UnityEngine.UI.GraphicRaycaster.BlockingObjects.None;
            }
        }

        void OnDisable()
        {
            NKController.Instance.LoadAvatarListSuccess -= ListAvatars;
        }

        public void ListAvatars(object sender, NKListArgs<INAvatar> avatarArgs)
        {
            avatarButtonDict.Clear();

            var avatars = avatarArgs.values;
            for (int i = 0; i < avatars.Count; i++)
            {
                INAvatar avatar = avatars[i];

                // set avatar list ui
                GameObject buttonObject = Instantiate(AvatarButtonPrefab, AvatarButtonPanel);
                buttonObject.SetActive(true);

                AvatarButton avatarButton = buttonObject.GetComponent<AvatarButton>();
                avatarButton.UpdateAvatarData(avatar);
                avatarButtonDict.Add(avatar, buttonObject);
                avatarButton.DownloadAvatar();

                buttonObject.GetComponentInChildren<Button>().onClick.AddListener(() => SetSelectedAvatar(avatarButton));
            }

            if (OnSetAvatarOptions != null)
            {
                OnSetAvatarOptions(avatars);
            }
        }

        public void SelectNextAvatar()
        {
            List<INAvatar> keyList = avatarButtonDict.Keys.ToList();
            int index = keyList.IndexOf(selectedAvatar);

            if (index + 1 < avatarButtonDict.Keys.Count)
            {
                index++;
            }
            else
            {
                index = 0;
            }

            SetSelectedAvatar(avatarButtonDict[keyList[index]].GetComponent<AvatarButton>());
            ScrollRect.SetVerticalScrollRectPosition(index, avatarButtonDict.Count, AvatarButtonPrefab.GetComponent<RectTransform>().rect.height);
        }

        public void SelectPreviousAvatar()
        {
            List<INAvatar> keyList = avatarButtonDict.Keys.ToList();
            int index = keyList.IndexOf(selectedAvatar);

            if (index > 0)
            {
                index--;
            }
            else
            {
                index = avatarButtonDict.Keys.Count - 1;
            }

            SetSelectedAvatar(avatarButtonDict[keyList[index]].GetComponent<AvatarButton>());
            ScrollRect.SetVerticalScrollRectPosition(index, avatarButtonDict.Count, AvatarButtonPrefab.GetComponent<RectTransform>().rect.height);
        }

        void SetSelectedAvatar(AvatarButton avatarBtn)
        {
            if (selectedAvatar != null)
            { // Clear previous selection
                selectedBtn.Select(false); // disable frame
            }
            //set selectedAvatar to new selection
            selectedBtn = avatarBtn;
            selectedAvatar = avatarBtn.GetComponent<AvatarButton>().GetAvatarData();
            avatarBtn.Select(true); // enable frame

            if (OnSetAvatar != null)
            {
                OnSetAvatar(selectedAvatar);
            }

            NKController.Instance.ChooseAvatar(selectedAvatar.Id);
        }

        public void SetDefaultAvatar(INAvatar avatar)
        {
            if (!avatarButtonDict.ContainsKey(avatar))
            {
                return;
            }

            GameObject avatarBtn = avatarButtonDict[avatar];
            avatarBtn.GetComponent<RectTransform>().SetAsFirstSibling();
            SetSelectedAvatar(avatarBtn.GetComponent<AvatarButton>());
        }
    }
}