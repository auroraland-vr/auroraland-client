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
using Fingo;

/// <summary>
/// FPS Display is used to display FPS and hand detection on a text mesh.
/// </summary>
[RequireComponent(typeof(TextMesh))]
public class FPSDisplay : MonoBehaviour {

    private TextMesh textMesh; //!< The text mesh show the information.
    private Hand leftHand; //!< The left hand data.
    private Hand rightHand; //!< The right hand data.

    void Awake()
    {
        textMesh = GetComponent<TextMesh>();
    }

    void Update()
    {
        string outputString = "";
        outputString += ("FPS: " + (1.0f / Time.deltaTime).ToString() + "\n");
        leftHand = FingoMain.Instance.GetHand(HandType.Left);
        rightHand = FingoMain.Instance.GetHand(HandType.Right);
        if(leftHand.IsDetected())
        {
            outputString += ("Left Hand Detected!\n");
        }
        if(rightHand.IsDetected())
        {
            outputString += ("Right Hand Detected!\n");
        }
        textMesh.text = outputString;
    }
}
