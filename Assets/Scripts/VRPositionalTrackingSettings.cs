using UnityEngine;
using VRTK;

namespace Auroraland.VR
{
    public class VRPositionalTrackingSettings : MonoBehaviour
    {
        public bool DisablePositionalTracking;

        void Start()
        {
            UnityEngine.XR.InputTracking.disablePositionalTracking = DisablePositionalTracking;
            VRTK_SDKManager.instance.LoadedSetupChanged += SDKManager_Instance_LoadedSetupChanged;
        }

        void OnDestroy()
        {
            VRTK_SDKManager.instance.LoadedSetupChanged -= SDKManager_Instance_LoadedSetupChanged;
        }

        void SDKManager_Instance_LoadedSetupChanged(VRTK_SDKManager sender, VRTK_SDKManager.LoadedSetupChangeEventArgs e)
        {
            UnityEngine.XR.InputTracking.disablePositionalTracking = DisablePositionalTracking;
        }
    }
}