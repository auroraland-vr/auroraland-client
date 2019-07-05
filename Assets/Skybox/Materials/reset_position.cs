using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class reset_position : MonoBehaviour {

	// Use this for initialization
	void Start () {
        UnityEngine.XR.InputTracking.Recenter();
        Valve.VR.OpenVR.System.ResetSeatedZeroPose();
        Valve.VR.OpenVR.Compositor.SetTrackingSpace(Valve.VR.ETrackingUniverseOrigin.TrackingUniverseSeated);
    }
	
	// Update is called once per frame
	void Update () {
        UnityEngine.XR.InputTracking.Recenter();
        Valve.VR.OpenVR.System.ResetSeatedZeroPose();
        Valve.VR.OpenVR.Compositor.SetTrackingSpace(Valve.VR.ETrackingUniverseOrigin.TrackingUniverseSeated);

    }
}
