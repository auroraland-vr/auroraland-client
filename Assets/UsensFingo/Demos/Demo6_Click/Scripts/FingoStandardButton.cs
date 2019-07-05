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

/// <summary>
/// A simple standard button.
/// </summary>
public class FingoStandardButton : MonoBehaviour
{
    public UnityEvent OnClick;
    public UnityEvent OnEnterButNotClick;
    public UnityEvent OnExit;
    
    public UnityEvent OnEnterHover;
    public UnityEvent OnExitHover;

    public static List<FingoStandardButton> Buttons = new List<FingoStandardButton>();

    void OnEnable()
    {
        Buttons.Add(this);

        if (OnClick == null)
        {
            OnClick = new UnityEvent();
        }

        if (OnEnterButNotClick == null)
        {
            OnEnterButNotClick = new UnityEvent();
        }

        if (OnExit == null)
        {
            OnExit = new UnityEvent();
        }

        if (OnEnterHover == null)
        {
            OnEnterHover = new UnityEvent();
        }

        if (OnExitHover == null)
        {
            OnExitHover = new UnityEvent();
        }
    }

    void OnDisable()
    {
        Buttons.Remove(this);
    }
}
