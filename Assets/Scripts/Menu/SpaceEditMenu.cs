using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Nakama;

namespace Auroraland
{
    public class SpaceEditMenu : MonoBehaviour {
        [Header("Configurations")]
        [SerializeField]
        SceneTransitionController _sceneTransitionController;

        public bool IsEditor = false;
		public MainMenu MainMenu;
		public SpaceMenu MySpaceMenu;
        public InputField SpaceNameInput;
		public InputField DescriptionInput;
		public InputField ImageUrlInput;
        public GameObject SpaceListScrollView;
        public ScrollRectSetter ThemeScrollRect;
        public Transform ThemeButtonRoot;
        public GameObject ThemeButtonPrefab;
		private INSpace selectedSpace;
		private ThemeButton selectedThemeButton;
        private Dictionary<INAsset, ThemeButton> ThemeButtonDict = new Dictionary<INAsset, ThemeButton>(); 
        
        void Start()
        {
            Assert.IsNotNull(_sceneTransitionController);
        }

        void OnEnable()
        {
            NKController.Instance.CreateSpaceSuccess += OnCreateSpaceSuccess;
            NKController.Instance.DeleteSpaceSuccess += OnDeleteSpaceSuccess;
            NKController.Instance.UpdateSpaceSuccess += OnUpdateSpaceSuccess;
            NKController.Instance.LoadAssetListSuccess += OnLoadThemeListSuccess;
            if (IsEditor)
            {
                selectedSpace = MySpaceMenu.SelectedSpace;
                SpaceNameInput.text = selectedSpace.DisplayName;
                DescriptionInput.text = selectedSpace.Description;
                ImageUrlInput.text = selectedSpace.ThumbnailUrl;
                //TODO parse selected space theme...
                Debug.Log("selected:" + selectedSpace.ToString());
            }
            else //create mode
            {
                NKController.Instance.LoadAssetList("theme");
                SpaceListScrollView.SetActive(false); // disable this one because the theme list overlaps with space list
            }
		}
        void OnDisable()
        {
            ClearInputfields();
            ClearScrollContent();
            selectedThemeButton = null;
            selectedSpace = null;
            SpaceListScrollView.SetActive(true);
            NKController.Instance.CreateSpaceSuccess -= OnCreateSpaceSuccess;
            NKController.Instance.UpdateSpaceSuccess -= OnUpdateSpaceSuccess;
            NKController.Instance.DeleteSpaceSuccess -= OnDeleteSpaceSuccess;
            NKController.Instance.LoadAssetListSuccess -= OnLoadThemeListSuccess;
        }

        public void SelectedSpace(SpaceButton spaceBtn){
			selectedSpace = spaceBtn.GetSpace();
		}

		public void SelectedTheme(ThemeButton themeButton){
			if (selectedThemeButton != null) {
				selectedThemeButton.Select (false);
			}
			selectedThemeButton = themeButton;
			selectedThemeButton.Select (true);
		}

        public void DeleteSpace()
        {
            string spaceId = selectedSpace.Id;
            NKController.Instance.DeleteSpace(spaceId);
        }

        public void CreateSpace(){
            string spaceName = (string.IsNullOrEmpty(SpaceNameInput.text)) ? "Untitled" : SpaceNameInput.text;
            string description = (string.IsNullOrEmpty(DescriptionInput.text)) ? "This is a space" : DescriptionInput.text;
            string defaultThumbnailUrl = "http://52.53.222.194/file/defaultSpace.png";
            if (selectedThemeButton != null)
            {
                defaultThumbnailUrl = selectedThemeButton.DefaultImageUrl;
            }
            string thumbnailUrl = (ImageUrlInput && !string.IsNullOrEmpty(ImageUrlInput.text)) ? ImageUrlInput.text : defaultThumbnailUrl;
            NKController.Instance.CreateSpace(spaceName, description, selectedThemeButton.AssetId);
        }

		public void UpdateSpace(){
            INSpace space = new NSpace(new Nakama.Space());
            space.Id = selectedSpace.Id;
            space.Metadata = selectedSpace.Metadata;
            space.Theme = selectedSpace.Theme;
            space.Lang = selectedSpace.Lang;
            space.DisplayName = (string.IsNullOrEmpty(SpaceNameInput.text)) ? "Untitled" : SpaceNameInput.text;
            space.Description = DescriptionInput.text;
            space.ThumbnailUrl = ImageUrlInput.text;

            NKController.Instance.UpdateSpace(space);
        }	

        private void OnCreateSpaceSuccess(object sender, NKSingleArg<INSpace> spaceArg)
        {
            DebugLogger.LogFormat("Successfully create space = {0}", spaceArg.value.ToString());
            selectedSpace = spaceArg.value;
            if (MainMenu.GetCurrentMenuState() != MainMenu.MENU_STATE.CREATE_SPACE) return;
            MySpaceMenu.RefreshSpaceMenu();
            MainMenu.ChangeToHomeMenu((int)MainMenu.MENU_STATE.MY_SPACE);
            MySpaceMenu.SetNewSpaceAsDefault(selectedSpace);

        }

        private void OnDeleteSpaceSuccess(object sender, NKSingleArg<string> spaceArg)
        {
            DebugLogger.LogFormat("Successfully delete space id = {0}", spaceArg.value);
            selectedSpace = null;
            MySpaceMenu.RefreshSpaceMenu();
            MainMenu.ChangeToHomeMenu((int)MainMenu.MENU_STATE.MY_SPACE);
        }

        private void OnUpdateSpaceSuccess(object sender, NKSingleArg<INSpace> spaceArg)
        {
            if (MainMenu.GetCurrentMenuState() != MainMenu.MENU_STATE.EDIT_SPACE) return;
            var space = spaceArg.value;
            Debug.Log("Successfully updated space");
            selectedSpace.Id = space.Id;
            selectedSpace.DisplayName = space.DisplayName;
            selectedSpace.Description = space.Description;
            selectedSpace.ThumbnailUrl = space.ThumbnailUrl;
            MainMenu.ChangeToHomeMenu((int)MainMenu.MENU_STATE.MY_SPACE);
        }

        private void OnLoadThemeListSuccess(object sender, NKListArgs<INAsset> assetArgs)
        {
            if (MainMenu.GetCurrentMenuState() != MainMenu.MENU_STATE.CREATE_SPACE) return;
            ThemeButtonDict.Clear();
            foreach (var theme in assetArgs.values)
            {
                Debug.Log("get theme:"+ theme.ToString());
                GameObject themeOption = Instantiate(ThemeButtonPrefab, ThemeButtonRoot) as GameObject;
                ThemeButton themeButton = themeOption.GetComponent<ThemeButton>();
                themeOption.SetActive(true);
                themeButton.SetTheme(theme);
                ThemeButtonDict.Add(theme, themeButton);
                if (selectedThemeButton == null)
                { //set as default
                    SelectedTheme(themeButton);
                }
            }
            
        }

        private void ClearInputfields()
        {
            if (SpaceNameInput) SpaceNameInput.text = "Untitled";
            if (DescriptionInput) DescriptionInput.text = "This is a space.";
            if (ImageUrlInput) ImageUrlInput.text = "";
        }

        private void ClearScrollContent()
        {
            if (ThemeButtonRoot == null) return;
            foreach (Transform child in ThemeButtonRoot)
            {
                Destroy(child.gameObject);
            }
        }

        public void SetSelectedTheme(ThemeButton themeBtn)
        {
            if (selectedThemeButton) selectedThemeButton.Select(false); // deselect previous button
            selectedThemeButton = themeBtn;
            selectedThemeButton.Select(true);
        }

        public void SelectNextTheme()
        {
            List<INAsset> keyList = ThemeButtonDict.Keys.ToList();
            int index = keyList.IndexOf(selectedThemeButton.Theme);

            if (index + 1 < ThemeButtonDict.Count)
                index++;
            else
                index = 0;

            SetSelectedTheme(ThemeButtonDict[keyList[index]]);
            ThemeScrollRect.SetHorizontalScrollRectPosition(index, ThemeButtonDict.Count, ThemeButtonPrefab.GetComponent<RectTransform>().rect.width);

        }

        public void SelectPreviousTheme()
        {
            List<INAsset> keyList = ThemeButtonDict.Keys.ToList();
            int index = keyList.IndexOf(selectedThemeButton.Theme);

            if (index > 0)
                index--;
            else
                index = ThemeButtonDict.Count - 1;

            SetSelectedTheme(ThemeButtonDict[keyList[index]]);
            ThemeScrollRect.SetHorizontalScrollRectPosition(index, ThemeButtonDict.Count, ThemeButtonPrefab.GetComponent<RectTransform>().rect.width);

        }

        public void SetDefaultTheme(INAsset theme)
        {
            if (!ThemeButtonDict.ContainsKey(theme)) return;
            ThemeButton themeBtn = ThemeButtonDict[theme];
            Debug.Log("default select theme to " + theme);
            SetSelectedTheme(themeBtn);
        }
    }
}
