using Nakama;
using UnityEngine;
using UnityEngine.UI;

namespace Auroraland
{
    public class SpaceButton : MonoBehaviour
    {
        public RawImage SpaceImage;
        public Texture DefaultSpaceImage;
        public Image Frame;
        public Text SpaceName;
        public bool IsEditMode;
        private string creatorName;
        private INSpace space;

        private void OnEnable()
        {
            NKController.Instance.LoadUserInfoSuccess += OnLoadUserInfoSuccess;
            NKController.Instance.LoadUserInfoFailure += OnLoadUserInfoFailure;
        }

        private void OnDisable()
        {
            NKController.Instance.LoadUserInfoSuccess -= OnLoadUserInfoSuccess;
            NKController.Instance.LoadUserInfoFailure -= OnLoadUserInfoFailure;
        }

        public void UpdateSpaceInfo(INSpace space, bool isEditMode)
        {
            this.space = space;
            SpaceName.text = space.DisplayName;
            IsEditMode = isEditMode;
            StartCoroutine(ImageLoader.LoadImage(SpaceImage, DefaultSpaceImage, space.ThumbnailUrl));

            NKController.Instance.LoadUserInfo(space.CreatorId);
        }

        public void OnLoadUserInfoSuccess(object sender, NKSingleArg<INUser> userArg)
        {
            if (space == null || string.IsNullOrEmpty(space.CreatorId) || userArg.value.Id != space.CreatorId)
            {
                return;
            }

            SetSpaceCreatorName(userArg.value.Fullname);
        }

        public void OnLoadUserInfoFailure(object sender, NKErrorArgs errorArgs)
        {
            SetSpaceCreatorName(space.CreatorId);
        }

        public string GetSpaceCreatorName()
        {
            return creatorName;
        }

        public void SetSpaceCreatorName(string creatorName)
        {
            this.creatorName = creatorName;

        }
        public INSpace GetSpace()
        {
            return space;
        }
        public void Select(bool select)
        {
            Frame.gameObject.SetActive(select);
        }


    }

}