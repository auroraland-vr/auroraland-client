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

/// <summary>
/// Helper to calculate the velocity of an action. The type of action can be click, swipe, throw, etc.
/// </summary>
public class VelocityCalculator : MonoBehaviour
{
    [Tooltip("The time window to track position. In seconds.")]
    public float trackingTimeWindow = 1f;

    [Tooltip("When the action (click/swipe/throw) started, the angle between this frame's velocity and last is greater than this threshold value. In degrees.")]
    public float velocityAngleThresh = 80f;
    
    [Tooltip("Ignore movement less than this thresold value. In meters.")]
    public float distThresh = 0.01f; // 1 cm

    protected struct TrackData
    {
        public Vector3 pos;
        public float timeStamp;
    }
    protected List<TrackData> velocityTrackData = new List<TrackData>();


    public void UpdatePositionData(Vector3 newPos, float dataLostTime, float dataRecoverTime)
    {
        // Remove data out of the tracking window
        // Note that we may lost data in the middle
        int trackingWindowStart = -1;
        for (int i = 0; (i < velocityTrackData.Count) && (trackingWindowStart < 0); i++)
        {
            if (dataLostTime > 0 && dataRecoverTime > dataLostTime &&
                velocityTrackData[i].timeStamp < dataLostTime) // this data is collected before last data lost
            {
                if (Time.time - velocityTrackData[i].timeStamp <= trackingTimeWindow + (dataRecoverTime - dataLostTime))
                    trackingWindowStart = i;
            }
            else
            {
                if (Time.time - velocityTrackData[i].timeStamp <= trackingTimeWindow)
                    trackingWindowStart = i;
            }
        }
        if (trackingWindowStart > 0)
            velocityTrackData.RemoveRange(0, trackingWindowStart);

        // Insert new data to the tracking window
        TrackData currentData;
        currentData.pos = newPos;
        currentData.timeStamp = Time.time;

        velocityTrackData.Insert(velocityTrackData.Count, currentData);
    }

    public void ClearPositionData()
    {
        if (velocityTrackData.Count > 0)
            velocityTrackData.Clear();
    }

    public Vector3 CalculateVelocity()
    {
        if (velocityTrackData.Count <= 1)
            return Vector3.zero;

        Vector3 pos1 = velocityTrackData[velocityTrackData.Count - 2].pos;
        Vector3 pos2 = velocityTrackData[velocityTrackData.Count - 1].pos;
        float t1 = velocityTrackData[velocityTrackData.Count - 2].timeStamp;
        float t2 = velocityTrackData[velocityTrackData.Count - 1].timeStamp;
        Vector3 lastv = (pos2 - pos1) / (t2 - t1);

        if (velocityTrackData.Count == 2)
            return lastv;

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

            // Ingore small shaking  
            if (Vector3.Magnitude(pos1 - pos2) > distThresh)
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
