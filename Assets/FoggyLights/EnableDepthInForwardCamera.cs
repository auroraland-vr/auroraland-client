using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
public class EnableDepthInForwardCamera : MonoBehaviour {

	// Use this for initialization
	void OnEnable() {
        if (GetComponent<Camera>().depthTextureMode == DepthTextureMode.None)
            GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
    }
	
	// Update is called once per frame
	void Update () {
        
    }
}
