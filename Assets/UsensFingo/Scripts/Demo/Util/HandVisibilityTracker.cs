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

public class HandVisibilityTracker: MonoBehaviour
{
    [Tooltip("The hand type of the tracked hand.")]
    public HandType handType;

    private Hand hand;

    private float handLostTime = -1;
    public float HandLostTime
    {
        get { return handLostTime; }
    }

    private float handRecoverTime = -1;
    public float HandRecoverTime
    {
        get { return handRecoverTime; }
    }

    private bool handIsLost = false;
    public bool HandIsLost
    {
        get { return handIsLost; }
    }

    private float lastTimeHandIsDetected = -1;
    private float timeThresh = 0.1667f; // roughly 10 frames in case of 60 fps

    private bool stereoCamera;


    void Start()
    {
        stereoCamera = (Camera.main.transform.Find("Main Camera Left") &&
                        Camera.main.transform.Find("Main Camera Right"));
    }

    void Update()
    {
        hand = FingoMain.Instance.GetHand(handType);

        // Track hand lost and recover time
        if (hand.IsDetected())
        {
            // hand detection is recovered or hand is detected first time
            if (lastTimeHandIsDetected < 0 || Time.time - lastTimeHandIsDetected > timeThresh)
            {
                handRecoverTime = Time.time;
                handIsLost = false;
            }
            lastTimeHandIsDetected = Time.time;
        }
        else
        {
            // hand detection is lost
            if (lastTimeHandIsDetected > 0 && Time.time - lastTimeHandIsDetected > timeThresh && 
                !handIsLost)
            {
                handLostTime = Time.time;
                handIsLost = true;
            }
        }
    }

    public bool PalmOnScreen()
    {
        if (hand != null && hand.IsDetected())
        {
            return (stereoCamera || OnScreen(hand.GetPalmPosition()));
        }
        return false;
    }

    public bool JointOnScreen(JointIndex jointIndex)
    {
        if (hand != null && hand.IsDetected())
        {
            return (stereoCamera || OnScreen(hand.GetJointPosition(jointIndex)));
        }
        return false;
    }

    public bool TipOnScreen(TipIndex tipIndex)
    {
        if (hand != null && hand.IsDetected())
        {
            return (stereoCamera || OnScreen(hand.GetTipPosition(tipIndex)));
        }
        return false;
    }

    bool OnScreen(Vector3 handPos)
    {
        // Transform position from hand local space to world space
        Vector3 worldPos = transform.parent.TransformPoint(handPos);

        // Transform position from world space to viewport space
        Vector3 screenPos = Camera.main.WorldToViewportPoint(worldPos);

        return (screenPos.x >= 0 && screenPos.x <= 1 &&
                screenPos.y >= 0 && screenPos.y <= 1 &&
                screenPos.z >= 0 && screenPos.z <= 1);
    }
}
