using UnityEngine;
using System.Collections;
using Valve.VR;

public class ResetSeatedPosition : MonoBehaviour
{
    [Tooltip("Desired head position of player when seated")]
    public Transform desiredHeadPosition;
    private Transform steamCamera;
    private Transform steamController;

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (desiredHeadPosition != null)
            {
                ResetSeatedPos(desiredHeadPosition);
            }
            else
            {
                Debug.Log("Target Transform required. Assign in inspector.");
            }

        }
    }

    private void ResetSeatedPos(Transform desiredHeadPos)
    {

        //find VR Camera and CameraRig transforms in scene
        steamCamera = FindObjectOfType<SteamVR_Camera>().gameObject.transform;
        steamController = FindObjectOfType<SteamVR_ControllerManager>().transform;

        if ((steamCamera != null) && (steamController != null))
        {
            //{first rotation}
            //get current head heading in scene
            //(y-only, to avoid tilting the floor)
            float offsetAngle = steamCamera.rotation.eulerAngles.y;
            //now rotate CameraRig in opposite direction to compensate
            steamController.Rotate(0f, -offsetAngle, 0f);

            //{now position}
            //calculate postional offset between CameraRig and Camera
            Vector3 offsetPos = steamCamera.position - steamController.position;
            //reposition CameraRig to desired position minus offset
            steamController.position = (desiredHeadPos.position - offsetPos);

            Debug.Log("Seat recentered!");
        }
        else
        {
            Debug.Log("Error: SteamVR objects not found!");
        }
    }
}