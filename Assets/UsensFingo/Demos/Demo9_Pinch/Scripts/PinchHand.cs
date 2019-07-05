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
using System.Collections.Generic;
using Fingo;

public enum PinchState
{
    ObjectOutOfReach,
    ObjectInReach,
    ObjectInHold,
    ObjectReleased
}

/// <summary>
/// Make the hand able to pinch objects.
/// </summary>
public class PinchHand : MonoBehaviour 
{
    [Tooltip("The hand type of the tracked hand.")]
    public HandType handType;

    private Hand hand;

    [Tooltip("The object may be able to be pinched, if its distance to the hand is smaller than this value. In meters.")]
    public float pinchEnableDistance = 0.06f;

    [Tooltip("The object may be able to be pinched, if the distance between thumb tip and index tip is larger than this value. In meters.")]
    public float pinchEnableTipDistance = 0.04f;  // 4 cm

    [Tooltip("Start pinching when the distance between thumb tip and index tip is smaller than this value. In meters.")]
    public float pinchStartTipDistance = 0.04f;   // 4 cm
    
    [Tooltip("Stop pinching when the distance between thumb tip and index tip is larger than this value. In meters.")]
    public float pinchEndTipDistance = 0.06f;     // 6 cm

    [Tooltip("The time window to track pinch relevant data. In seconds.")]
    public float trackingTimeWindow = 1f;

    [Tooltip("Speed threshold value to determine if user is currently pinching (closing fingers). In meters per second.")]
    public float closingSpeedThresh = 0.01f; // 1 cm per second

    [Tooltip("Speed threshold value to determine if user is currently releasing (opening fingers). In meters per second.")]
    public float openingSpeedThresh = 0.02f; // 2 cm per second

    private struct TrackData
    {
        public float thumbIndexTipDist;
        public float timeStamp;
    }
    private List<TrackData> pinchTrackData = new List<TrackData>();

    private PinchState pinchState;
    private PinchableObject activeObject = null;
    private PinchableObject prevActiveObject = null;
    private Transform pinchedObjectParent = null;       

    private float currentTipDist;
    private float pinchSpeed;

    // The render manager for updating hand material at run-time according to current grab state
    private MeshHandRenderManager meshHandRenderMgr;

    // Delegate for calculating throw velocity
    private VelocityCalculator velocityCalculator;

    // Delegate for tracking hand detetion lost and recover time
    private HandVisibilityTracker handVisibilityTracker;
    private float handLostTime = -1;
    private float handRecoverTime = -1;
    private bool handOnscreen = true;

    private bool handIsLost = false;
    private bool handIsLostLastFrame = false;

    private bool isHoveringLastFrame = false;


    void Awake()
    {
        meshHandRenderMgr = this.GetComponent<MeshHandRenderManager>();
        velocityCalculator = this.GetComponent<VelocityCalculator>();
        handVisibilityTracker = this.GetComponent<HandVisibilityTracker>();
    }

    void OnEnable()
    {
        pinchState = PinchState.ObjectOutOfReach;
    }
	
	void Update () 
    {
        hand = FingoMain.Instance.GetHand(handType);

        // Track hand lost and recover time, as well as if index tip is currently inside camera view.
        if (handVisibilityTracker != null)
        {
            handLostTime = handVisibilityTracker.HandLostTime;
            handRecoverTime = handVisibilityTracker.HandRecoverTime;
            handIsLost = handVisibilityTracker.HandIsLost;
            handOnscreen = handVisibilityTracker.TipOnScreen(TipIndex.IndexTip);
        }

        if (hand.IsDetected())
        {
            // Update data even if hand is currently outside camera view (but still detected)
            UpdatePinchTrackData();

            // Update pinch states if and only if hand is currently onscreen
            if (handOnscreen)
            {
                UpdatePinchState();
            }

            // for calculating throw velocity
            if (velocityCalculator != null && pinchState == PinchState.ObjectInHold)
            {
                Vector3 indexProximalPos = hand.GetJointPosition(JointIndex.IndexProximal);
                velocityCalculator.UpdatePositionData(indexProximalPos, handLostTime, handRecoverTime);
            }
        }

        if (handIsLost && !handIsLostLastFrame) // lost hand detection this frame
        {
            if (activeObject != null) // hand is not detected
            {
                activeObject.SetOutlineColorTransparency(0);
            }
        }
        handIsLostLastFrame = handIsLost;
    }
    
    void UpdatePinchTrackData()
    {
        Vector3 thumbTipPos = hand.GetTipPosition(TipIndex.ThumbTip);
        Vector3 indexTipPos = hand.GetTipPosition(TipIndex.IndexTip);

        // Calculate distance between thumb tip and index tip
        currentTipDist = Vector3.Distance(thumbTipPos, indexTipPos);

        // Set transform. This is the parent transform of the object when it is in pinch.
        this.transform.localPosition = Vector3.Lerp(thumbTipPos, indexTipPos, .5f);
        this.transform.localRotation = hand.GetJointLocalRotation(JointIndex.WristJoint);

        // Remove data out of the tracking window
        // Note that we may lost data in the middle
        int trackingWindowStart = -1;
        for (int i = 0; (i < pinchTrackData.Count) && (trackingWindowStart < 0); i++)
        {
            if (handLostTime > 0 && handRecoverTime > handLostTime &&
                pinchTrackData[i].timeStamp < handLostTime) // this data is collected before last data lost
            {
                if (Time.time - pinchTrackData[i].timeStamp <= trackingTimeWindow + (handRecoverTime - handLostTime))
                    trackingWindowStart = i;
            }
            else
            {
                if (Time.time - pinchTrackData[i].timeStamp <= trackingTimeWindow)
                    trackingWindowStart = i;
            }
        }
        if (trackingWindowStart > 0)
            pinchTrackData.RemoveRange(0, trackingWindowStart);

        // Insert current-frame data to the tracking window
        TrackData currentData;
        currentData.thumbIndexTipDist = currentTipDist;
        currentData.timeStamp = Time.time;

        pinchTrackData.Insert(pinchTrackData.Count, currentData);

        // Calculate measurements based on current data and data 1 second (trackingTimeWindow) ago
        pinchSpeed = (pinchTrackData.Count > 1) ?
            (currentTipDist - pinchTrackData[0].thumbIndexTipDist) / (Time.time - pinchTrackData[0].timeStamp) : 0;
    }
    
    void UpdatePinchState()
    {
        GestureName currentGesture = hand.GetGestureName();

        bool isClosingFingers = (pinchSpeed < -closingSpeedThresh);
        bool isOpeningFingers = (pinchSpeed > openingSpeedThresh);


        if (activeObject != null && activeObject.IsAvailableForPinch() == false &&
            pinchState != PinchState.ObjectInHold) // pinched by the other hand
        {
            activeObject = null;
            pinchState = PinchState.ObjectOutOfReach;
        }

        if ((pinchState == PinchState.ObjectOutOfReach) ||
            (pinchState == PinchState.ObjectInReach && !isClosingFingers) ||
            (pinchState == PinchState.ObjectReleased))
        {
            // Iterate all pinchable objects, find the closest available one.
            PinchableObject closestAvailableObj = null;
            float minDistance = 1000f;
            foreach (PinchableObject obj in PinchableObject.instances)
            {
                if (obj.IsAvailableForPinch())
                {
                    float distance = Vector3.Distance(this.transform.position, obj.transform.position);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestAvailableObj = obj;
                    }
                }
            }

            // Mark the closest available object as active
            activeObject = closestAvailableObj;

            // Check if it is range.
            bool inReach = (minDistance < pinchEnableDistance);

            if (activeObject != null)
            {
                // To give user visual feedbacks, update the outline color on the active object 
                // based on its current distance to the hand.
                activeObject.UpdateOutlineColorBasedOnDistance(minDistance);

                // Reset the outline color (fully transparent) on the last active object if it was different
                if (prevActiveObject != null && prevActiveObject != activeObject)
                    prevActiveObject.SetOutlineColorTransparency(0);

                // If we are able to pinch the currently active object
                if (inReach && currentTipDist > pinchEnableTipDistance)
                {
                    if (pinchState != PinchState.ObjectInReach || activeObject != prevActiveObject)
                    {
                        activeObject.OnGetInReach.Invoke();
                        pinchState = PinchState.ObjectInReach;
                    }
                }
            }

            // Note, we want to ignore movement (which can cause ObjectInReach state to change to ObjectOutOfReach)
            // if user is already in the middle of pinching the object
            if (!inReach && !(pinchState == PinchState.ObjectInReach && isClosingFingers))
            {
                if (pinchState != PinchState.ObjectOutOfReach || activeObject != prevActiveObject)
                {
                    if (activeObject != null)
                        activeObject.OnOutOfReach.Invoke();

                    pinchState = PinchState.ObjectOutOfReach;
                }
            }
        }

        if (pinchState == PinchState.ObjectInReach)
        {
            if ((currentTipDist < pinchStartTipDistance || currentGesture == GestureName.PinchCloseMRP ||
                currentGesture == GestureName.PinchOpenMRP) && isClosingFingers)
            {
                // To give user visual feedbacks, make the hand transparent.
                if (meshHandRenderMgr != null)
                {
                    meshHandRenderMgr.DisableHandOutline();
                    meshHandRenderMgr.FadeOut();
                }

                if (activeObject != null)
                {
                    activeObject.OnPickUp.Invoke();
                    // Save the parent transform
                    pinchedObjectParent = activeObject.transform.parent;
                    activeObject.transform.parent = this.transform;
                    activeObject.transform.localPosition = Vector3.zero;
                    // Disable the object for pinch
                    activeObject.SetAvailableForPinch(false);
                }
                pinchState = PinchState.ObjectInHold;
            }
        }

        if (pinchState == PinchState.ObjectInHold)
        {
            if (currentTipDist > pinchEndTipDistance && isOpeningFingers)
            {
                // To give user visual feedbacks, make the hand opaque (default).
                if (meshHandRenderMgr != null)
                    meshHandRenderMgr.FadeIn();

                if (activeObject != null)
                {
                    activeObject.OnRelease.Invoke();

                    Vector3 velocity = (velocityCalculator != null) ? velocityCalculator.CalculateVelocity() : Vector3.zero;
                    if (Vector3.Magnitude(velocity) > 0)
                    {
                        activeObject.OnThrow.Invoke(velocity);
                        // Enable the object for pinch later
                        activeObject.SetAvailableForPinch(true, activeObject.FlyBackTime());
                    }
                    else
                    {
                        // Enable the object for pinch right away
                        activeObject.SetAvailableForPinch(true, 0);
                    }
                    // Restore the parent transform
                    activeObject.transform.parent = pinchedObjectParent;
                }

                pinchState = PinchState.ObjectReleased;
            }
        }

        //Debug.Log("Pinch state = " + pinchState);


        // To give user visual feedbacks, highlight hand outline if it is hovering an object, otherwise disable outline.
        bool isHovering = (pinchState == PinchState.ObjectInReach);
        if (meshHandRenderMgr != null)
        {
            if (isHovering && !isHoveringLastFrame)
            {
                meshHandRenderMgr.EnableHandOutline();
            }
            else if (!isHovering && isHoveringLastFrame)
            {
                meshHandRenderMgr.DisableHandOutline();
            }
        }
        isHoveringLastFrame = isHovering;

        prevActiveObject = activeObject;
    }

}
