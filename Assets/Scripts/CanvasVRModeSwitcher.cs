using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
/*?
 * CanvasVRModeSwitcher is a helper class that switches between VRTK_UICanvases(used for vr) and GraphicRaycaster(used for none vr)
 * Please make sure every canvases has this component and SDK_SetUpMode component exists in every scene.
 * 
 */
namespace Auroraland
{
    public class CanvasVRModeSwitcher : MonoBehaviour
    {
        private VRTK_UICanvas[] canvases;

        private void Awake()
        {
            canvases = transform.GetComponentsInChildren<VRTK_UICanvas>(true);
        }
        private void OnEnable()
        {
            SDK_SetupMode.SwitchToNone += DisableVRCanvases;
            SDK_SetupMode.SwitchToNonVR += DisableVRCanvases;
            SDK_SetupMode.SwitchToOculus += EnableVRCanvases;
            SDK_SetupMode.SwitchToSteamVR += EnableVRCanvases;

        }

        private void OnDisable()
        {
            SDK_SetupMode.SwitchToNone -= DisableVRCanvases;
            SDK_SetupMode.SwitchToNonVR -= DisableVRCanvases;
            SDK_SetupMode.SwitchToOculus -= EnableVRCanvases;
            SDK_SetupMode.SwitchToSteamVR -= EnableVRCanvases;
        }

        private void Start()
        {
            if (SDK_SetupMode.Instance.IsVRMode) EnableVRCanvases();
            else DisableVRCanvases();
        }

        private void EnableVRCanvases() {
            foreach (VRTK_UICanvas canvas in canvases) {
                canvas.enabled = true;
            }
        }

        private void DisableVRCanvases() {
            foreach (VRTK_UICanvas canvas in canvases)
            {
                canvas.enabled = false;
            }
        }
    }

}
