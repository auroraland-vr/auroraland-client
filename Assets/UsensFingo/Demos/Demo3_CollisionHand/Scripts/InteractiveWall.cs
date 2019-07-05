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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractiveWall : MonoBehaviour
{
    public GameObject ripple;

    // Make surrounding bars glow when wall is hit
    public Material barGlowMaterial;
    public float barGlowDuration = 0.5f;

    private RippleAnimator rippleAnimator;

    private List<Renderer> barRenderers = new List<Renderer>();
    private Material barDefaultMaterial;

    void Awake()
    {
        if (ripple != null)
            rippleAnimator = ripple.GetComponent<RippleAnimator>();

        for (int i = 0; i < this.transform.childCount; i++)
        {
            Transform child = this.transform.GetChild(i);
            if (child.tag == "Bar")
            {
                Renderer childRenderer = child.GetComponent<Renderer>();
                if (childRenderer != null)
                    barRenderers.Add(childRenderer);
            }
        }

        if (barRenderers.Count > 0)
            barDefaultMaterial = barRenderers[0].material;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (ripple == null || rippleAnimator == null)
            return;

        Transform parentOfOther = collision.gameObject.transform.parent;
        bool hitHand = (parentOfOther != null && (parentOfOther.name == "CollisionHand_L" || parentOfOther.name == "CollisionHand_R"));

        if (!hitHand)
        {
            // Move ripple to the contact position
            ContactPoint contact = collision.contacts[0];
            if (name == "Left" || name == "Right")
            {
                ripple.transform.position = new Vector3(ripple.transform.position.x, contact.point.y, contact.point.z);
            }
            else if (name == "Front" || name == "Back")
            {
                ripple.transform.position = new Vector3(contact.point.x, contact.point.y, ripple.transform.position.z);
            }
            else if (name == "Top" || name == "Bottom")
            {
                ripple.transform.position = new Vector3(contact.point.x, ripple.transform.position.y, contact.point.z);
            }

            // Animate ripple
            rippleAnimator.Animate();

            // Make surrounding bars glow
            foreach (Renderer barRenderer in barRenderers)
            {
                StartCoroutine(MakeBarGlow(barRenderer, barGlowDuration));
            }
        }
    }

    IEnumerator MakeBarGlow(Renderer barRenderer, float duration)
    {
        if (barRenderer == null || barGlowMaterial == null || barDefaultMaterial == null)
            yield break;
        
        barRenderer.material = barGlowMaterial;
        yield return new WaitForSeconds(duration);
        barRenderer.material = barDefaultMaterial;
    }
}