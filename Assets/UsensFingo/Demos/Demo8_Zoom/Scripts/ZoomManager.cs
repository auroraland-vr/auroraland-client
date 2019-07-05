/*************************************************************************\
*                           USENS CONFIDENTIAL                            *
* _______________________________________________________________________ *
*                                                                         *
* [2014] - [2017] USENS Incorporated                                      *
* All Rights Reserved.                                                    *
*                                                                         *
* NOTICE:  All information contained herein is, and remains               *
* the property of uSens Incorporated and its suppliers,                   *
* if any.  The intellectual and technical concepts contained              *
* herein are proprietary to uSens Incorporated                            *
* and its suppliers and may be covered by U.S. and Foreign Patents,       *
* patents in process, and are protected by trade secret or copyright law. *
* Dissemination of this information or reproduction of this material      *
* is strictly forbidden unless prior written permission is obtained       *
* from uSens Incorporated.                                                *
*                                                                         *
\*************************************************************************/

using UnityEngine;
using System.Collections;
using Fingo;

/// <summary>
/// Use two hands pinch gesture to scale object.
/// </summary>
public class ZoomManager : MonoBehaviour 
{
    private ZoomHand leftZoomHand;
    private ZoomHand rightZoomHand;

    public delegate void ZoomEventHandler(float displacement);
    public static event ZoomEventHandler ZoomEvent;

    public delegate void EnterZoomEventHandler();
    public static event EnterZoomEventHandler EnterZoomEvent;

    public delegate void QuitZoomEventHandler();
    public static event QuitZoomEventHandler QuitZoomEvent;

    private bool isZooming = false;
    private float zoomStartedTime = -1;
    private float timeThresh = 0.5f;

    // The render manager for updating hand material depending on whether user is currently zooming
    private MeshHandRenderManager meshHandRenderMgr;
    

    void OnEnable()
    {
        meshHandRenderMgr = this.GetComponent<MeshHandRenderManager>();

        ZoomHand[] zoomHands = GetComponentsInChildren<ZoomHand>();
        foreach (ZoomHand zoomHand in zoomHands)
        {
            if (zoomHand.handType == HandType.Left)
            {
                leftZoomHand = zoomHand;
            }
            else if (zoomHand.handType == HandType.Right)
            {
                rightZoomHand = zoomHand;
            }
        }
    }
	
	void Update ()
    {
        if (leftZoomHand == null || rightZoomHand == null)
        {
            Debug.Log("NULL ZoomHand component.");
            return;
        }
           
        if (leftZoomHand.InZoomGesture && rightZoomHand.InZoomGesture)
        {
            if (!isZooming)
            {
                isZooming = true;
                zoomStartedTime = Time.time;
                StartZooming();
            }
        }
        else
        {
            // To reduce flickering, stop after zooming at least half a second (timeThresh).
            if (isZooming && Time.time - zoomStartedTime > timeThresh)
            {
                isZooming = false;
                StopZooming();
            }
        }

        if (isZooming)
        {
            Vector3 leftVelocity = leftZoomHand.ZoomVelocity;
            Vector3 rightVelocity = rightZoomHand.ZoomVelocity;

            float displacement = (leftVelocity.x * rightVelocity.x <= 0) ? // move in different direction
                    (-leftVelocity.x + rightVelocity.x) * Time.deltaTime :
                    0;

            if (ZoomEvent != null && Mathf.Abs(displacement) > 0)
                ZoomEvent(displacement);
        }
    }
    
    void StartZooming()
    {
        if (EnterZoomEvent != null)
            EnterZoomEvent();

        // To give user visual feedbacks, highlight hand outline.
        if (meshHandRenderMgr != null)
            meshHandRenderMgr.EnableHandOutline();
    }

    void StopZooming()
    {
        if (QuitZoomEvent != null)
            QuitZoomEvent();

        // To give user visual feedbacks, de-highlight hand outline.
        if (meshHandRenderMgr != null)
            meshHandRenderMgr.DisableHandOutline();
    }
}