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
using UnityEngine.Events;

/// <summary>
/// Objects that can be grabbed.
/// </summary>
public class GrabbaleObject : MonoBehaviour
{
    public static List<GrabbaleObject> instances = new List<GrabbaleObject>();

    public UnityEvent OnGetInReach;
    public UnityEvent OnOutOfReach;
    public UnityEvent OnGrab;
    public UnityEvent OnRelease;
    public VelocityEvent OnThrow;

    private OutlineCtrl outlineCtrl;
    private ThrowCtrl throwCtrl;

    private bool isFree = true;


    void Awake()
    {
        outlineCtrl = this.GetComponent<OutlineCtrl>();
        throwCtrl = this.GetComponent<ThrowCtrl>();
    }

    void OnEnable()
    {
        instances.Add(this);

        if (OnGetInReach == null)
        {
            OnGetInReach = new UnityEvent();
        }

        if (OnOutOfReach == null)
        {
            OnOutOfReach = new UnityEvent();
        }

        if (OnGrab == null)
        {
            OnGrab = new UnityEvent();
        }

        if (OnRelease == null)
        {
            OnRelease = new UnityEvent();
        }

        if (OnThrow == null)
        {
            OnThrow = new VelocityEvent();
        }

        OnThrow.AddListener((x) => { Debug.Log("Thrown at " + x.x + "," + x.y + "," + x.z); });
    }

    void OnDisable()
    {
        instances.Remove(this);
    }

    public void UpdateOutlineColorBasedOnDistance(float distance)
    {
        if (outlineCtrl)
        {
            outlineCtrl.UpdateOutlineColorBasedOnDistance(distance);
        }
    }

    public void SetOutlineColorTransparency(float alpha)
    {
        if (outlineCtrl)
        {
            outlineCtrl.SetOutlineColorTransparency(alpha);
        }
    }

    public void SetAvailableForGrab(bool available, float delay = 0)
    {
        if (delay > 0)
        {
            StartCoroutine(SetFree(available, delay));
        }
        else
        {
            isFree = available;
        }
    }

    IEnumerator SetFree(bool available, float delay)
    {
        yield return new WaitForSeconds(delay);
        isFree = available;
    }

    public bool IsAvailableForGrab()
    {
        return isFree;
    }

    public float FlyBackTime()
    {
        return (throwCtrl != null) ? throwCtrl.flyBackTime : 0;
    }
}
