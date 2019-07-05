/*************************************************************************\
*                           USENS CONFIDENTIAL                            *
* _______________________________________________________________________ *
*                                                                         *
* [2015] - [2017] USENS Incorporated                                      *
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
using UnityEngine.Events;
using Fingo;

public class GestureResponseCtrl : MonoBehaviour
{
    public UnityEvent OnRightHandOkayGesture;
    public UnityEvent OnLeftHandOkayGesture;

    public UnityEvent OnRightHandPeaceGesture;
    public UnityEvent OnLeftHandPeaceGesture;

    public UnityEvent OnRightHandFistGesture;
    public UnityEvent OnLeftHandFistGesture;

    public UnityEvent OnClearRight;
    public UnityEvent OnClearLeft;

    void OnEnable()
    {
        if (OnRightHandOkayGesture == null)
        {
            OnRightHandOkayGesture = new UnityEvent();
        }
        if (OnLeftHandOkayGesture == null)
        {
            OnLeftHandOkayGesture = new UnityEvent();
        }
        if (OnRightHandPeaceGesture == null)
        {
            OnRightHandPeaceGesture = new UnityEvent();
        }
        if (OnLeftHandPeaceGesture == null)
        {
            OnLeftHandPeaceGesture = new UnityEvent();
        }
        if (OnRightHandFistGesture == null)
        {
            OnRightHandFistGesture = new UnityEvent();
        }
        if (OnLeftHandFistGesture == null)
        {
            OnLeftHandFistGesture = new UnityEvent();
        }
        if (OnClearRight == null)
        {
            OnClearRight = new UnityEvent();
        }
        if (OnClearLeft == null)
        {
            OnClearLeft = new UnityEvent();
        }

        GestureManager.GestureDetected += ColorChange;
    }

    public void ColorChange(HandType handType, GestureName gestureType)
    {
        if (handType == HandType.Right)
        {
            switch (gestureType)
            {
                case GestureName.Okay:
                    OnRightHandOkayGesture.Invoke();
                    break;
                case GestureName.Peace:
                    OnRightHandPeaceGesture.Invoke();
                    break;
                case GestureName.Fist:
                    OnRightHandFistGesture.Invoke();
                    break;
                default: // Gesture.None or any other gesture
                    OnClearRight.Invoke();
                    break;
            }
        }
        else if (handType == HandType.Left)
        {
            switch (gestureType)
            {
                case GestureName.Okay:
                    OnLeftHandOkayGesture.Invoke();
                    break;
                case GestureName.Peace:
                    OnLeftHandPeaceGesture.Invoke();
                    break;
                case GestureName.Fist:
                    OnLeftHandFistGesture.Invoke();
                    break;
                default: // Gesture.None or any other gesture
                    OnClearLeft.Invoke();
                    break;
            }
        }
    }
}
