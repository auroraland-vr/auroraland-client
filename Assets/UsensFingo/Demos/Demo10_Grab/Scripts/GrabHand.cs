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

public enum GrabState
{
    ObjectOutOfReach,
    ObjectInReach,
    ObjectInHold,
    ObjectReleased
}

/// <summary>
/// Make the hand able to grab objects.
/// </summary>
public class GrabHand : MonoBehaviour
{
    [Tooltip("The hand type of the tracked hand.")]
    public HandType handType;

    private Hand hand;

    [Tooltip("The object may be able to be grabbed, if its distance to the hand is smaller than this value. In meters.")]
    public float grabEnableDistance = 0.08f; // 8 cm

    [Tooltip("The object may be able to be grabbed, if finger bending angle is smaller than this value. In degrees.")]
    public float grabEnableAngle = 90;

    [Tooltip("Start grabbing when finger bending angle is larger than this value. In degrees.")]
    public float grabStartAngle = 70;

    [Tooltip("Stop grabbing when finger bending angle is smaller than this value. In degrees.")]
    public float grabEndAngle = 90;

    [Tooltip("The time window to track grab relevant data. In seconds.")]
    public float trackingTimeWindow = 1f;

    [Tooltip("Angular speed threshold value to determine if the fingers are currently closing/opening. In degrees per second.")]
    public float anguarSpeedThresh = 10; // 10 degrees per second

    private struct TrackData {
        public float bendingAngle;
        public float timeStamp;
    }
    private List<TrackData> grabTrackData = new List<TrackData>();

    private GrabState grabState;
    private GrabbaleObject activeObject = null;
    private GrabbaleObject prevActiveObject = null;
    private Transform grabbedObjectParent = null;

    private float currentBendingAngle;
    private float bendingAngularSpeed;
    
    // The render manager for updating hand material at run-time according to current grab state
    private MeshHandRenderManager meshHandRenderMgr;

    // Delegate for calculating throw velocity
    private VelocityCalculator velocityCalculator;
    
    // Delegate for tracking hand detetion lost and recover time, as well as if hand is currently inside camera view.
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
        grabState = GrabState.ObjectOutOfReach;
    }

    void Update()
    {
        hand = FingoMain.Instance.GetHand(handType);

        // Track hand lost and recover time, as well as if palm is currently inside camera view.
        if (handVisibilityTracker != null)
        {
            handLostTime = handVisibilityTracker.HandLostTime;
            handRecoverTime = handVisibilityTracker.HandRecoverTime;
            handIsLost = handVisibilityTracker.HandIsLost;
            handOnscreen = handVisibilityTracker.PalmOnScreen();
        }

        if (hand.IsDetected())
        {
            // Update data even if hand is currently outside camera view (but still detected)
            UpdateGrabTrackData();

            // Update grab states if and only if hand is currently onscreen
            if (handOnscreen)
            {
                UpdateGrabState();
            }

            // for calculating throw velocity
            if (velocityCalculator != null && grabState == GrabState.ObjectInHold)
            {
                Vector3 palmPos = hand.GetPalmPosition();
                velocityCalculator.UpdatePositionData(palmPos, handLostTime, handRecoverTime);
            }
        }

        if (handIsLost && !handIsLostLastFrame) // lost hand detection this frame
        {
            if (activeObject != null)
            {
                activeObject.SetOutlineColorTransparency(0);
            }
        }
        handIsLostLastFrame = handIsLost;
    }

    float CalculateBendingAngle()
    {
        Vector3 middleTipPos = hand.GetTipPosition(TipIndex.MiddleTip);
        Vector3 middleProximalPos = hand.GetJointPosition(JointIndex.MiddleProximal);
        Vector3 wristPos = hand.GetJointPosition(JointIndex.WristJoint);

        // Calculate the angle between two vectors: (middleTipPos - middleProximalPos) and(middleProximalPos - wristPos)
        // as the finger bending angle.
        return Vector3.Angle(Vector3.Normalize(middleTipPos - middleProximalPos),
            Vector3.Normalize(middleProximalPos - wristPos));
    }

    Vector3 CalculateGrabPoint()
    {
        Vector3 pinkyProximalPos = hand.GetJointPosition(JointIndex.PinkyProximal);
        Vector3 indexProximalPos = hand.GetJointPosition(JointIndex.IndexProximal);
        Vector3 v1 = Vector3.Normalize(pinkyProximalPos - indexProximalPos);

        Vector3 middleTipPos = hand.GetTipPosition(TipIndex.MiddleTip);
        Vector3 wristPos = hand.GetJointPosition(JointIndex.WristJoint);
        Vector3 v2 = Vector3.Normalize(middleTipPos - wristPos);
        
        Vector3 n = (handType == HandType.Right) ? Vector3.Cross(v1, v2) : Vector3.Cross(v2, v1);

        Vector3 middleProximalPos = hand.GetJointPosition(JointIndex.MiddleProximal);
        float offset = 0.04f; // 4cm
        return middleProximalPos + n * offset;
    }

    void UpdateGrabTrackData()
    {
        currentBendingAngle = CalculateBendingAngle();

        Vector3 grabPoint = CalculateGrabPoint();

        // Set transform. This is the parent transform of the object when it is in grab.
        this.transform.localPosition = grabPoint;
        this.transform.localRotation = hand.GetJointLocalRotation(JointIndex.WristJoint);

        // Remove data out of the tracking window
        // Note that we may lost data in the middle
        int trackingWindowStart = -1;
        for (int i = 0; (i < grabTrackData.Count) && (trackingWindowStart < 0); i++)
        {
            if (handLostTime > 0 && handRecoverTime > handLostTime &&
                grabTrackData[i].timeStamp < handLostTime) // this data is collected before last data lost
            {
                if (Time.time - grabTrackData[i].timeStamp <= trackingTimeWindow + (handRecoverTime - handLostTime))
                    trackingWindowStart = i;
            }
            else
            {
                if (Time.time - grabTrackData[i].timeStamp <= trackingTimeWindow)
                    trackingWindowStart = i;
            }
        }
        if (trackingWindowStart > 0)
            grabTrackData.RemoveRange(0, trackingWindowStart);

        // Insert current-frame data to the tracking window
        TrackData currentData;
        currentData.bendingAngle = currentBendingAngle;
        currentData.timeStamp = Time.time;

        grabTrackData.Insert(grabTrackData.Count, currentData);

        // Calculate measurements based on current data and data 1 second (trackingTimeWindow) ago
        bendingAngularSpeed = (grabTrackData.Count > 1) ?
            (currentBendingAngle - grabTrackData[0].bendingAngle) / (Time.time - grabTrackData[0].timeStamp) : 0;
    }

    void UpdateGrabState()
    {
        GestureName currentGesture = hand.GetGestureName();

        bool isClosingFingers = (bendingAngularSpeed > anguarSpeedThresh);
        bool isOpeningFingers = (bendingAngularSpeed < -anguarSpeedThresh);

        if (activeObject != null && activeObject.IsAvailableForGrab() == false &&
            grabState != GrabState.ObjectInHold) // grabbed by the other hand
        {
            activeObject = null;
            grabState = GrabState.ObjectOutOfReach;
        }
        
        if ((grabState == GrabState.ObjectOutOfReach) ||
            (grabState == GrabState.ObjectInReach && !isClosingFingers) ||
            (grabState == GrabState.ObjectReleased))
        {
            // Iterate all grabbable objects, find the closest available one.
            GrabbaleObject closestAvailableObj = null;
            float minDistance = 1000f;
            foreach (GrabbaleObject obj in GrabbaleObject.instances)
            {
                if (obj.IsAvailableForGrab())
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
            bool inReach = (minDistance < grabEnableDistance);
            
            if (activeObject != null && (currentGesture == GestureName.Grab || currentGesture == GestureName.Palm ||
                currentBendingAngle < grabEnableAngle))
            {
                // To give user visual feedbacks, update the outline color on the active object 
                // based on its current distance to the hand.
                activeObject.UpdateOutlineColorBasedOnDistance(minDistance);

                // Reset the outline color (fully transparent) on the last active object if it was different
                if (prevActiveObject != null && prevActiveObject != activeObject)
                    prevActiveObject.SetOutlineColorTransparency(0);

                // If we are close enough to grab the currently active object
                if (inReach && (grabState != GrabState.ObjectInReach || activeObject != prevActiveObject))
                {
                    activeObject.OnGetInReach.Invoke();
                    grabState = GrabState.ObjectInReach;
                }
            }

            // Note, we want to ignore movement (which can cause ObjectInReach state to change to ObjectOutOfReach)
            // if user is already in the middle of grabbing the object
            if (!inReach && !(grabState == GrabState.ObjectInReach && isClosingFingers))
            {
                if (grabState != GrabState.ObjectOutOfReach || activeObject != prevActiveObject)
                {
                    if (activeObject != null)
                        activeObject.OnOutOfReach.Invoke();

                    grabState = GrabState.ObjectOutOfReach;
                }
            }
        }

        if (grabState == GrabState.ObjectInReach)
        {
            if ((currentBendingAngle > grabStartAngle || currentGesture == GestureName.Fist) && isClosingFingers)
            {
                // To give user visual feedbacks, make the hand transparent.
                if (meshHandRenderMgr != null)
                {
                    meshHandRenderMgr.DisableHandOutline();
                    meshHandRenderMgr.FadeOut();
                }

                if (activeObject != null)
                {
                    activeObject.OnGrab.Invoke();
                    // Save the parent transform
                    grabbedObjectParent = activeObject.transform.parent;
                    activeObject.transform.parent = this.transform;
                    activeObject.transform.localPosition = Vector3.zero;
                    // Disable the object for grab
                    activeObject.SetAvailableForGrab(false);
                }
                grabState = GrabState.ObjectInHold;
            }
        }
        
        if (grabState == GrabState.ObjectInHold)
        {
            if (currentBendingAngle < grabEndAngle && isOpeningFingers)
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
                        // Enable the object for grab later
                        activeObject.SetAvailableForGrab(true, activeObject.FlyBackTime());
                    }
                    else
                    {
                        // Enable the object for grab right away
                        activeObject.SetAvailableForGrab(true, 0);
                    }
                    // Restore the parent transform
                    activeObject.transform.parent = grabbedObjectParent;
                }

                grabState = GrabState.ObjectReleased; 
            }
        }

        //Debug.Log("Grab state = " + grabState);


        // To give user visual feedbacks, highlight hand outline if it is hovering an object, otherwise disable outline.
        bool isHovering = (grabState == GrabState.ObjectInReach);
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
