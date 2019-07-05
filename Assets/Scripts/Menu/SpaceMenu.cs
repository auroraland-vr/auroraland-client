using System;
using Nakama;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Auroraland
{
    public class SpaceMenu : MonoBehaviour
    {
        [Header("Configurations")]
        [SerializeField]
        private SceneTransitionController _sceneTransitionController;

        public bool IsMySpace;
        [Header("Display Panel")]
        public RawImage HeroImage;
        public Texture DefaultSpaceImage;
        public Text SpaceName;
        public Text CreatorName;
        public Text Description;
        public Text CreatedAt;
        public Text UpdatedAt;
        public Button EnterSpaceButton;
        public Button ConfigButton;

        [Header("Scroll Panel")]
        public GameObject SpaceButtonPrefab;
        public GameObject SpaceCreateButton;
        public Transform ScrollContentRoot;
        public ScrollRectSetter ScrollRect;

        private SpaceButton selectedBtn;
        private bool hasNewCreatedSpace = false; // this is set to true once a new space is created
        public INSpace SelectedSpace { private set; get; }

        private Dictionary<INSpace, GameObject> spaceDict = new Dictionary<INSpace, GameObject>();

        private void Start()
        {
            Assert.IsNotNull(_sceneTransitionController);
        }

        public void OnEnable()
        {
            if (IsMySpace)
            { // my space
                NKController.Instance.LoadSpaceListSuccess += SetMySpaceList;
                NKController.Instance.LoadSpaceList(true);
            }
            else
            { // all space
                NKController.Instance.LoadSpaceListSuccess += SetAllSpaceList;
                NKController.Instance.LoadSpaceList(false);
            }
        }

        public void OnDisable()
        {
            if (IsMySpace)
            { // my space
                NKController.Instance.LoadSpaceListSuccess -= SetMySpaceList;
            }
            else
            { // all space
                NKController.Instance.LoadSpaceListSuccess -= SetAllSpaceList;
            }
            ClearScrollContent();
        }

        private IEnumerator SelectDefaultSpace(SpaceButton spaceBtn)
        {
            SelectSpace(spaceBtn);
            spaceBtn.Select(true);
            yield return new WaitWhile(() => string.IsNullOrEmpty(spaceBtn.GetComponent<SpaceButton>().GetSpaceCreatorName()));
            CreatorName.text = spaceBtn.GetComponent<SpaceButton>().GetSpaceCreatorName();
            if(hasNewCreatedSpace) hasNewCreatedSpace = false;
        }

        public IEnumerator SetNewSpaceAsDefault(INSpace space)
        {
            hasNewCreatedSpace = true;
            yield return new WaitUntil(() => spaceDict[space] != null);
            SpaceButton spaceBtn = spaceDict[space].GetComponent<SpaceButton>();
            yield return SelectDefaultSpace(spaceBtn);
        }

        public void OnClickCreateSpace(SpaceButton spaceBtn)
        {
            if (selectedBtn != null)
            { // set previous
                selectedBtn.Select(false);
            }
            selectedBtn = spaceBtn;
            spaceBtn.Select(true);
        }


        public void SelectSpace(SpaceButton spaceBtn)
        {
            if (selectedBtn != null)
            { // set previous
                selectedBtn.Select(false);
            }
            selectedBtn = spaceBtn;
            SelectedSpace = spaceBtn.GetComponent<SpaceButton>().GetSpace();
            spaceBtn.Select(true);

            bool isEditMode = spaceBtn.GetComponent<SpaceButton>().IsEditMode;
            string title = (isEditMode) ? string.Format("{0} ({1})", SelectedSpace.DisplayName.ToUpper(), "Edit") : SelectedSpace.DisplayName.ToUpper();
            SpaceName.text = title;
            Description.text = SelectedSpace.Description;
            long unixUpdatedAt = long.Parse(SelectedSpace.UpdatedAt.ToString());
            long unixCreatedAt = long.Parse(SelectedSpace.CreatedAt.ToString());
            DateTime updatedAtTime = UnixTimeConverter.UnixTimeStampToDateTime(unixUpdatedAt);
            DateTime createdAtTime = UnixTimeConverter.UnixTimeStampToDateTime(unixCreatedAt);
            UpdatedAt.text = updatedAtTime.ToShortDateString() + " " + updatedAtTime.ToShortTimeString();
            CreatedAt.text = createdAtTime.ToShortDateString() + " " + createdAtTime.ToShortTimeString();
            CreatorName.text = spaceBtn.GetComponent<SpaceButton>().GetSpaceCreatorName();
            StartCoroutine(ImageLoader.LoadImage(HeroImage, DefaultSpaceImage, SelectedSpace.ThumbnailUrl));
            EnterSpaceButton.gameObject.SetActive(true);
            HeroImage.gameObject.SetActive(true);
            if (IsMySpace) ConfigButton.gameObject.SetActive(true);

            _sceneTransitionController.SelectedSpace = SelectedSpace;
		}

        public void SelectNextSpace()
        {
            int index = spaceDict.Keys.ToList().IndexOf(SelectedSpace);

            if (index + 1 < spaceDict.Keys.Count)
            {
                index++;
            }
            else
            {
                index = 0;
            }

            INSpace select = spaceDict.Keys.ToList()[index];
            SelectSpace(spaceDict[select].GetComponent<SpaceButton>());
            int extraButtonCount = (IsMySpace) ? 1 : 0;
            ScrollRect.SetHorizontalScrollRectPosition(index, spaceDict.Count, SpaceButtonPrefab.GetComponent<RectTransform>().rect.width, extraButtonCount);
        }

        public void SelectPreviousSpace()
        {
            int index = spaceDict.Keys.ToList().IndexOf(SelectedSpace);

            if (index > 0)
            {
                index--;
            }
            else
            {
                index = spaceDict.Keys.Count - 1;
            }

            INSpace select = spaceDict.Keys.ToList()[index];
            SelectSpace(spaceDict[select].GetComponent<SpaceButton>());
            int extraButtonCount = (IsMySpace) ? 1 : 0;
            ScrollRect.SetHorizontalScrollRectPosition(index, spaceDict.Count, SpaceButtonPrefab.GetComponent<RectTransform>().rect.width, extraButtonCount);
        }

        public void ClearPanel(){ //used when clicked create
			SpaceName.text = "";
			Description.text = "";
			UpdatedAt.text = "";
			CreatedAt.text = "";
			CreatorName.text = "";
			EnterSpaceButton.gameObject.SetActive (false);
		}

		public void UpdateSpaceButton(INSpace space){
			//TODO find according space button and update space info
            if (!spaceDict.ContainsKey(space)) return;
            SpaceButton spaceBtn = spaceDict[space].GetComponent<SpaceButton>();
            spaceBtn.GetComponent<SpaceButton>().UpdateSpaceInfo (space, true);
            SelectSpace (spaceBtn);
        }

        public void RefreshSpaceMenu()
        {
            NKController.Instance.LoadSpaceList(IsMySpace);
        }

        public string GetSelectedSpaceId()
        {
            return SelectedSpace.Id;
        }

        public bool IsSelectedSpaceEditMode()
        {
            return IsMySpace;
        }

        private void ClearScrollContent()
        {
            foreach (Transform child in ScrollContentRoot)
            {
                Destroy(child.gameObject);
            }
            spaceDict.Clear();
        }

        private void SetAllSpaceList(object sender, NKListArgs<INSpace> spaceArgs)
        {
            ClearScrollContent();
            List<INSpace> buffer = spaceArgs.values;
            foreach (var entry in buffer)
            {
                INSpace space = entry;
                if (!_sceneTransitionController.IsCurrentThemeHome && SpaceManager.Instance.CurrentSpaceId == space.Id) continue; // don't display space if you are in the space
                GameObject spaceBtn = Instantiate(SpaceButtonPrefab, ScrollContentRoot);
                spaceBtn.SetActive(true);
                spaceBtn.GetComponent<SpaceButton>().UpdateSpaceInfo(space, false);
                spaceDict[entry] = spaceBtn;
            }
            var defaultEntry = spaceDict.FirstOrDefault();
            SpaceButton defaultSpaceButton = defaultEntry.Value.GetComponent<SpaceButton>();
            StartCoroutine(SelectDefaultSpace(defaultSpaceButton)); // default is the first space;
        }

        private void SetMySpaceList(object sender, NKListArgs<INSpace> spaceArgs)
        {
            ClearScrollContent();
            List<INSpace> buffer = spaceArgs.values;
            foreach (var entry in buffer)
            {
                INSpace space = entry;
                GameObject spaceBtn = Instantiate(SpaceButtonPrefab, ScrollContentRoot);
                spaceBtn.SetActive(true);
                spaceBtn.GetComponent<SpaceButton>().UpdateSpaceInfo(space, false);
                spaceDict[entry] = spaceBtn;
            }
            GameObject spaceCreateBtn = Instantiate(SpaceCreateButton, ScrollContentRoot);
            spaceCreateBtn.SetActive(true);
            spaceCreateBtn.GetComponent<RectTransform>().SetAsFirstSibling();

            if (buffer.Count > 0)
            {
                if (hasNewCreatedSpace) return;
                var defaultEntry = spaceDict.FirstOrDefault();
                SpaceButton defaultSpaceButton = defaultEntry.Value.GetComponent<SpaceButton>();
                StartCoroutine(SelectDefaultSpace(defaultSpaceButton)); // default is the first space;
            }
            else
            {
                HeroImage.gameObject.SetActive(false);
                EnterSpaceButton.gameObject.SetActive(false);
                ConfigButton.gameObject.SetActive(false);
                SpaceName.text = "You created 0 spaces.";
                CreatorName.text = "---";
                Description.text = "You haven't create any space yet. Try create one!";
                UpdatedAt.text = "-- / -- / ----";
                CreatedAt.text = "-- / -- / ----";
            }
        }
    }
}