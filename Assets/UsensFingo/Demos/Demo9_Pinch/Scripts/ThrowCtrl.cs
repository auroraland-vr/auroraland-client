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

[RequireComponent(typeof(Rigidbody))]
public class ThrowCtrl : MonoBehaviour
{
    public float factor = 3f;
    public bool enableFlyBack = false;
    public float flyBackTime = 1.0f;

    private Rigidbody rigidBody;

    void Awake()
    {
        rigidBody = this.GetComponent<Rigidbody>();
    }

    public void Throw(Vector3 velocity)
    {
        if (Vector3.Magnitude(velocity) > 0)
        {
            if (rigidBody != null)
            {
                rigidBody.velocity = velocity * factor;
            }

            if (enableFlyBack)
            {
                StartCoroutine(AutoFlyBack(transform.position, transform.rotation));
            }
        }
    }

    IEnumerator AutoFlyBack(Vector3 startPosition, Quaternion startRotation)
    {
        yield return new WaitForSeconds(flyBackTime);

        this.transform.position = startPosition;
        this.transform.rotation = startRotation;

        if (rigidBody != null)
        {
            rigidBody.velocity = Vector3.zero;
        }
    }
}
