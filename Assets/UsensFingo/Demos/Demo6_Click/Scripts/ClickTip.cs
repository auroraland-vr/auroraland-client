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
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using Fingo;

/// <summary>
/// This class controls a collider that triggers clicking events. 
/// The tracked tip or joint can be modified in method TrackTipPosition().
/// </summary>
public class ClickTip : MonoBehaviour 
{
    /// <summary>
    /// The hand type of the tracked hand.
    /// </summary>
    public HandType handType;

    private Hand hand;

    public UnityEvent OnClickSucceeded;

    [Tooltip("Speed towards button has to be at least this value to be considered a click. In meters per second.")]
    public float zSpeedThresh = 0.04f; // 4 cm per second

    private float lastClickTime;
    private float lastHoverBeganTime;

    // Only one click/hover event will be fired during this period of time:
    private float timeThreshBetweenEvents = 0.5f; // half a second

    // Delegate for calculating click velocity
    private VelocityCalculator velocityCalculator;

    // Delegate for tracking hand detetion lost and recover time
    private HandVisibilityTracker handVisibilityTracker;
    private float handLostTime = -1;
    private float handRecoverTime = -1;


    void Awake()
    {
        velocityCalculator = this.GetComponent<VelocityCalculator>();
        handVisibilityTracker = this.GetComponent<HandVisibilityTracker>();
    }

    void OnEnable()
    {
        if (OnClickSucceeded == null)
        {
            OnClickSucceeded = new UnityEvent();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "ButtonHover")
        {
            // It has passed over 0.5 second (timeThreshBetweenEvents) since the last time a hover event fired.
            if (Time.time - lastHoverBeganTime > timeThreshBetweenEvents)
            {
                GameObject parent = other.gameObject.transform.parent.gameObject;
                FingoStandardButton button = parent.GetComponent<FingoStandardButton>();
                
                if (button != null && button.OnEnterHover != null)
                {
                    button.OnEnterHover.Invoke();
                }

                lastHoverBeganTime = Time.time;
            }
        }
        else if (other.tag == "Button")
        {
            FingoStandardButton button = other.gameObject.GetComponent<FingoStandardButton>();
            
            Vector3 clickVelocity = (velocityCalculator != null) ? velocityCalculator.CalculateVelocity() : Vector3.zero;
            
            // Project velocity to object axes
            float vx = Vector3.Dot(clickVelocity, other.gameObject.transform.right);
            float vy = Vector3.Dot(clickVelocity, other.gameObject.transform.up);
            float vz = Vector3.Dot(clickVelocity, -other.gameObject.transform.forward); //invert z

            // If speed along z-axis is greater than that along x-axis and y-axis, then user is clicking towards the button.
            bool isClickingForward = (vz > zSpeedThresh && vz > Mathf.Abs(vx) && vz > Mathf.Abs(vy));

            // If all three conditions below are met, then it is a valid click:
            // (1) user is clicking forward,
            // (2) user collides the button with a clicking gesture, and
            // (3) it has passed over 0.5 second (timeThreshBetweenEvents) since the last click.
            bool clicked = (isClickingForward && IsClickingGesture() && 
                (Time.time - lastClickTime > timeThreshBetweenEvents));

            if (clicked)
            {
                if (button != null && button.OnClick != null)
                {
                    button.OnClick.Invoke();
                }
                OnClickSucceeded.Invoke();

                lastClickTime = Time.time;
            }
            else
            {
                if (button != null && button.OnEnterButNotClick != null)
                {
                    button.OnEnterButNotClick.Invoke();
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "ButtonHover")
        {
            GameObject parent = other.gameObject.transform.parent.gameObject;
            FingoStandardButton button = parent.GetComponent<FingoStandardButton>();

            if (button != null && button.OnExitHover != null)
            {
                button.OnExitHover.Invoke();
            }
        }
        else if (other.tag == "Button")
        {
            FingoStandardButton button = other.gameObject.GetComponent<FingoStandardButton>();

            if (button != null && button.OnExit != null)
            {
                button.OnExit.Invoke();
            }
        }
    }

    void Update()
    {
        hand = FingoMain.Instance.GetHand(handType);

        // track hand lost and recover time
        if (handVisibilityTracker != null)
        {
            handLostTime = handVisibilityTracker.HandLostTime;
            handRecoverTime = handVisibilityTracker.HandRecoverTime;
        }

        if (hand.IsDetected())
        {
            Vector3 currentTipPos = hand.GetTipPosition(TipIndex.IndexTip);
            this.transform.localPosition = currentTipPos;
            
            if (velocityCalculator != null)
            {
                velocityCalculator.UpdatePositionData(currentTipPos, handLostTime, handRecoverTime);
            }
        }
    }

    bool IsClickingGesture()
    {
        GestureName currentGesture = hand.IsDetected() ? hand.GetGestureName() : GestureName.None;

        return (currentGesture == GestureName.Point || currentGesture == GestureName.ShootEm ||
            currentGesture == GestureName.Peace || currentGesture == GestureName.MiddleFinger);
    }
}
