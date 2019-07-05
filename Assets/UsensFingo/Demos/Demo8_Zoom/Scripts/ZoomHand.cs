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
/// One of the hands in pinch gesture to to drag object nearer or farther.
/// </summary>
public class ZoomHand : MonoBehaviour 
{
    [Tooltip("The hand type of the tracked hand.")]
    public HandType handType;

    [Tooltip("If the distance between thumb tip and index tip is smaller than this value, then the hand is ready to zoom.")]
    public float zoomGestureThreshold = 0.05f; // 5 cm

    private Hand hand;

    // Delegate for calculating velocity
    private VelocityCalculator velocityCalculator;

    // Delegate for tracking hand detetion lost and recover time, as well as if hand is currently inside camera view.
    private HandVisibilityTracker handVisibilityTracker;
    private float handLostTime = -1;
    private float handRecoverTime = -1;

    private bool handIsLost = false;
    private bool handIsLostLastFrame = false;

    // Allow zooming after hand is detected for a short period of time
    private float timeThresh = 0.5f;

    private bool inZoomGesture = false;
    public bool InZoomGesture
    {
        get { return inZoomGesture; }
    }

    private Vector3 zoomVelocity = Vector3.zero;
    public Vector3 ZoomVelocity
    {
        get { return zoomVelocity; }
    }


    void Awake()
    {
        velocityCalculator = this.GetComponent<VelocityCalculator>();
        handVisibilityTracker = this.GetComponent<HandVisibilityTracker>();
    }

    void Update()
    {
        hand = FingoMain.Instance.GetHand(handType);

        // Track hand lost and recover time
        if (handVisibilityTracker != null)
        {
            handLostTime = handVisibilityTracker.HandLostTime;
            handRecoverTime = handVisibilityTracker.HandRecoverTime;
            handIsLost = handVisibilityTracker.HandIsLost;
        }

        if (hand.IsDetected())
        {
            // Allow zooming after hand is detected for a short period of time
            if (handRecoverTime > 0 && Time.time - handRecoverTime > timeThresh)
            {
                // Track if hand is in zoom (pinch) gesture
                TrackGesture();

                // Track how fast hand is moving
                if (velocityCalculator != null)
                {
                    Vector3 pos = hand.GetJointPosition(JointIndex.IndexProximal);
                    velocityCalculator.UpdatePositionData(pos, handLostTime, handRecoverTime);
                    zoomVelocity = velocityCalculator.CalculateVelocity();
                }
            }
        }

        if (handIsLost && !handIsLostLastFrame) // lost hand detection this frame
        {
            inZoomGesture = false;
            zoomVelocity = Vector3.zero;

            if (velocityCalculator != null)
            {
                velocityCalculator.ClearPositionData();
            }
        }
        handIsLostLastFrame = handIsLost;
    }

    void TrackGesture()
    {
        Vector3 thumbTipPos = hand.GetTipPosition(TipIndex.ThumbTip);
        Vector3 indexTipPos = hand.GetTipPosition(TipIndex.IndexTip);
        float thumbIndexTipDistance = Vector3.Distance(thumbTipPos, indexTipPos);

        GestureName currentGesture = hand.GetGestureName();

        inZoomGesture = (thumbIndexTipDistance < zoomGestureThreshold ||
                currentGesture == GestureName.PinchCloseMRP ||
                currentGesture == GestureName.PinchCloseMRP);
    }
}