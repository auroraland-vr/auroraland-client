using Nakama;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Auroraland
{
    public class AssetMenu : MonoBehaviour
    {

        [Header("UI")]
        public bool IsModel;
        public Transform ScrollContentRoot;
        public ScrollRectSetter ScrollRect;
        public GameObject AssetTabPrefab;
        public TabButton AllTab;
        public TabButton FurnitureTab;
        public TabButton PlantsTab;
        public TabButton ArtTab;
        public TabButton OthersTab;
        public AssetButton SelectedAssetButton { get; private set; }
        private Dictionary<INAsset, AssetButton> assetButtonDict = new Dictionary<INAsset, AssetButton>();
        public INAsset SelectedAsset { private set; get; }


        void OnEnable()
        {
            SelectedAssetButton = null;
            NKController.Instance.LoadAssetListSuccess += OnLoadAssetListSuccess;
            if (IsModel)
            {
                AllTab.IsButtonToggled = true;
            }

            RefreshAuroalandModelList(AllTab);
        }

        void OnDisable()
        {
            NKController.Instance.LoadAssetListSuccess -= OnLoadAssetListSuccess;
            ClearScrollContent();
        }

        private void ClearScrollContent()
        {
            assetButtonDict.Clear();
            foreach (Transform child in ScrollContentRoot)
            {
                Destroy(child.gameObject);
            }
        }


        public void RefreshAuroalandModelList(TabButton tabButton)
        {
            ClearScrollContent();
            if (IsModel)
            {
                List<string> categories = new List<string>();

                if (tabButton == AllTab)
                {
                    //set other buttons to toggled when alltab button is pressed
                    if (AllTab.IsButtonToggled)
                    {
                        FurnitureTab.IsButtonToggled = true;
                        ArtTab.IsButtonToggled = true;
                        PlantsTab.IsButtonToggled = true;
                        OthersTab.IsButtonToggled = true;
                    }
                }
                else
                {
                    //reset alltab button state 
                    if (FurnitureTab.IsButtonToggled && ArtTab.IsButtonToggled && PlantsTab.IsButtonToggled && OthersTab.IsButtonToggled)
                    {
                        AllTab.IsButtonToggled = true;
                    }
                    else
                    {
                        AllTab.IsButtonToggled = false;
                    }
                }

                if (FurnitureTab.IsButtonToggled)
                {
                    categories.Add("furniture");
                }
                if (ArtTab.IsButtonToggled)
                {
                    categories.Add("art");
                }
                if (PlantsTab.IsButtonToggled)
                {
                    categories.Add("plants");
                }
                if (OthersTab.IsButtonToggled)
                {
                    categories.Add("others");
                }
                Debug.Log(string.Join(",", categories.ToArray()));
                NKController.Instance.LoadAssetList("prefab", string.Join(",", categories.ToArray()));
            }
            else
            {
                NKController.Instance.LoadAssetList("game");
            }
        }

        public void OnLoadAssetListSuccess(object sender, NKListArgs<INAsset> assetArgs)
        {

            foreach (var asset in assetArgs.values)
            {
                if (IsModel && asset.Type == "game")break;
                else if (!IsModel && asset.Type == "prefab") break;
                Debug.Log("List Asset:" + asset.Name);
                GameObject button = Instantiate(AssetTabPrefab, ScrollContentRoot) as GameObject;
                button.SetActive(true);
                AssetButton assetButton = button.GetComponent<AssetButton>();
                assetButtonDict.Add(asset,assetButton);
                assetButton.SetAssetInfo(asset);
            }
            INAsset firstAsset = assetArgs.values.First();
            SetDefaultAsset(firstAsset);//set first asset as default button
        }

        public void SetSelectedAsset(AssetButton assetButton) {
            if(SelectedAssetButton) SelectedAssetButton.Deselect(); // deselect previous button
            assetButton.OnSelect();
            SelectedAssetButton = assetButton;
        }

        public void SelectNextAsset()
        {
            List<INAsset> keyList = assetButtonDict.Keys.ToList();
            int index = keyList.IndexOf(SelectedAssetButton.Asset);

            if (index + 1 < assetButtonDict.Count)
                index++;
            else
                index = 0;

            SetSelectedAsset(assetButtonDict[keyList[index]]);
            ScrollRect.SetVerticalScrollGridRectPosition(index, assetButtonDict.Count,AssetTabPrefab.GetComponent<RectTransform>().rect.height);
            
        }

        public void SelectPreviousAsset()
        {
            List<INAsset> keyList = assetButtonDict.Keys.ToList();
            int index = keyList.IndexOf(SelectedAssetButton.Asset);

            if (index > 0)
                index--;
            else
                index = assetButtonDict.Count-1;

            SetSelectedAsset(assetButtonDict[keyList[index]]);
            ScrollRect.SetVerticalScrollGridRectPosition(index, assetButtonDict.Count, AssetTabPrefab.GetComponent<RectTransform>().rect.height);

        }

        public void SetDefaultAsset(INAsset asset)
        {
            if (!assetButtonDict.ContainsKey(asset)) return;
            AssetButton assetBtn = assetButtonDict[asset];
            Debug.Log("default select asset to " + asset);
            SetSelectedAsset(assetBtn);
        }
    }

}
