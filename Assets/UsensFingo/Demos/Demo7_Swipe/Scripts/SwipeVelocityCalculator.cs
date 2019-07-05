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

public class SwipeVelocityCalculator : VelocityCalculator
{
    private Vector3 cameraToSwipeStart;

    void OnEnable()
    {
        cameraToSwipeStart = Camera.main.transform.forward; // camera forward vector by default
    }

    public void SetSwipeStartPos(Vector3 pos)
    {
        cameraToSwipeStart = Vector3.Normalize(pos - Camera.main.transform.position);
    }

    bool ValidMove(Vector3 startPos, Vector3 endPos)
    {
        Vector3 camPos = Camera.main.transform.position;

        float startAngle = Vector3.Angle(cameraToSwipeStart, startPos - camPos);
        float endAngle   = Vector3.Angle(cameraToSwipeStart, endPos - camPos);

        return (endAngle < 1e-3 || startAngle < endAngle); // either not swiping or swiping outwards
    }

    public new Vector3 CalculateVelocity()
    {
        if (velocityTrackData.Count <= 1)
            return Vector3.zero;

        Vector3 pos1 = velocityTrackData[velocityTrackData.Count - 2].pos;
        Vector3 pos2 = velocityTrackData[velocityTrackData.Count - 1].pos;
        float t1 = velocityTrackData[velocityTrackData.Count - 2].timeStamp;
        float t2 = velocityTrackData[velocityTrackData.Count - 1].timeStamp;
        Vector3 lastv = (pos2 - pos1) / (t2 - t1);

        if (velocityTrackData.Count == 2)
        {
            // Make sure it is a valid move (either not swiping or swiping outwards)
            return ValidMove(pos1, pos2) ? lastv : Vector3.zero;
        }

        // more than two frames

        float timeThresh = 0.1667f; // roughly 10 frames in case of 60 fps

        int actionStartIndex = -1;
        pos2 = pos1;
        t2 = t1;

        // Iterate back to find the frame when this action started, i.e. when a change of direction occurred.
        for (int i = velocityTrackData.Count - 3; i >= 0 && actionStartIndex < 0; i--)
        {
            pos1 = velocityTrackData[i].pos;
            t1 = velocityTrackData[i].timeStamp;
            Vector3 v = (pos2 - pos1) / (t2 - t1);

            if ((Vector3.Angle(lastv, v) > velocityAngleThresh) && // change of direction
                (velocityTrackData[velocityTrackData.Count - 1].timeStamp - t1) > timeThresh) // skip few frames
            {
                actionStartIndex = i; // search done
            }
            else
            {
                pos2 = pos1;
                t2 = t1;
                lastv = v;
            }
        }

        // This is the frame when user started the action
        actionStartIndex += 1;
        if (actionStartIndex >= 0 && actionStartIndex < velocityTrackData.Count - 1)
        {
            pos1 = velocityTrackData[actionStartIndex].pos;
            pos2 = velocityTrackData[velocityTrackData.Count - 1].pos;
            t1 = velocityTrackData[actionStartIndex].timeStamp;
            t2 = velocityTrackData[velocityTrackData.Count - 1].timeStamp;

            // Make sure it is a valid move (either not swiping or swiping outwards)
            // Also ignore small shaking
            if (ValidMove(pos1, pos2) && 
                Vector3.Magnitude(pos1 - pos2) > distThresh) 
            {
                return (pos2 - pos1) / (t2 - t1);
            }
            else
            {
                return Vector3.zero;
            }
        }
        else
        {
            return Vector3.zero;
        }
    }
}
