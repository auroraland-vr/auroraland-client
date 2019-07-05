using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

namespace Auroraland
{
    public class SDK_SetupMode : MonoBehaviour
    {
        public static SDK_SetupMode Instance;
        public delegate void SwitchSDKEvent(); 
        public static event SwitchSDKEvent SwitchToSteamVR;
        public static event SwitchSDKEvent SwitchToOculus;
        public static event SwitchSDKEvent SwitchToNone;
        public static event SwitchSDKEvent SwitchToNonVR;

        public string CurrentSDKName { private set; get; }
        public bool IsVRMode { private set; get; }
        private VRTK_SDKManager sdkManager;
        // Use this for initialization
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        private void OnEnable()
        {
            sdkManager = VRTK_SDKManager.instance;
            sdkManager.LoadedSetupChanged += OnLoadedSetupChanged;
        }

        private void OnDisable()
        {
            sdkManager.LoadedSetupChanged -= OnLoadedSetupChanged;
        }

        protected virtual void OnLoadedSetupChanged(VRTK_SDKManager sender, VRTK_SDKManager.LoadedSetupChangeEventArgs e)
        {
            if (sender.loadedSetup == null)
            {
                CurrentSDKName = "None";
                IsVRMode = false;
                SwitchToNone();
            }
            else if ((sender.loadedSetup.systemSDK.GetType() == typeof(VRTK.SDK_SteamVRSystem)))
            {
                CurrentSDKName = "SteamVR";
                IsVRMode = true;
                SwitchToSteamVR();
            }
            else if ((sender.loadedSetup.systemSDK.GetType() == typeof(VRTK.SDK_OculusSystem)))
            {
                CurrentSDKName = "Oculus";
                IsVRMode = true;
                SwitchToOculus();
            }
            else if ((sender.loadedSetup.systemSDK.GetType() == typeof(Auroraland.NonVRSimSystem)))
            {
                CurrentSDKName = "NonVR";
                IsVRMode = false;
                SwitchToNonVR();
            }
        }
    }
}

