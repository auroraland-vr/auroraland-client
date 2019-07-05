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

#define LOG_TIME_PROFILING

using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace Fingo
{
    /// <summary>
    /// FingoHead is a script controlling the transform of the game object
    /// which contain a Camera component to move according to the data from
    /// Fingo positional tracking data.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class FingoHead : MonoBehaviour
    {
        [Tooltip("The moving scale of head movement.")]
        public float HeadMovementScale = 100.0f; //!< The moving scale of head movement.
        [Tooltip("The button triggering the reset of positional tracking.")]
        public KeyCode ResetHeadTrackingBtn; //!< The button triggering the reset of positional tracking.
        public KeyCode InitHeadTrackingBtn;

        public bool enablePrediction;

        private int count = 0;
        private float touchTime = 0;

        private Head head;
        private bool isHeadTracked = false;
        private bool enableHeadTracking = true;

        private void Start()
        {
            StartCoroutine(WaitForHeadInitialize());
        }

        void Update()
        {
            if (enableHeadTracking)
            {
                if (isHeadTracked)
                {
                    head = FingoMain.Instance.GetHead(enablePrediction);
                    if (head != null)
                    {
                        transform.localPosition = head.GetPosition() * HeadMovementScale;
                        transform.localRotation = head.GetRotation();
                    }
                }

            }
        }

        IEnumerator WaitForHeadInitialize()
        {
            yield return new WaitForSeconds(3.0f);
			FingoManager.Instance.InitHeadTracking ();
            FingoMain.Instance.ResetHeadTracking();
            isHeadTracked = true;
        }
    }
}