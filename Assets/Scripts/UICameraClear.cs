using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

/// <summary>
/// Separates rendering of UI from main camera to ensure UI stays in front and unaffected by post processing effects.
/// </summary>
public class UICameraClear : MonoBehaviour {

    /// <summary>
    /// Clones the main camera as a UI camera and adjusts settings for each.
    /// </summary>
    public static void Setup()
    {
        GameObject mainCamObj = VRTK.VRTK_DeviceFinder.HeadsetCamera().gameObject;
        Camera mainCam = mainCamObj.GetComponent<Camera>();

        GameObject uiCamObj = new GameObject("UICamera");
        uiCamObj.transform.parent = mainCamObj.transform.parent;
        Camera uiCam = uiCamObj.AddComponent<Camera>();
        uiCam.CopyFrom(mainCam);

        int alwaysInFrontLayer = LayerMask.GetMask("AlwaysInFront");

        mainCam.cullingMask ^= alwaysInFrontLayer;      //  Cull the UI from the main camera
        uiCam.cullingMask = alwaysInFrontLayer;         //  Draw the UI on the UI camera

        uiCam.depth += 1;   //  Enables the UI to be drawn in front
        uiCam.clearFlags = CameraClearFlags.Depth;

        Destroy(uiCamObj.GetComponent<PostProcessingBehaviour>());  // Removes post processing on UI

        VRTK.VRTK_TransformFollow transformFollow = uiCamObj.AddComponent<VRTK.VRTK_TransformFollow>();
        transformFollow.gameObjectToFollow = mainCamObj;    // Set UI camera to align with main camera
    }
}
