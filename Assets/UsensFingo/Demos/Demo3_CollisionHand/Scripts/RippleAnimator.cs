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

public class RippleAnimator : MonoBehaviour
{
    public Color rippleColor = Color.white;
    public float rippleScale = 6f;
    public float rippleDuration = 0.5f;

    private Renderer rippleRenderer;
    private bool isAnimating = false;

    void Awake()
    {
        rippleRenderer = this.GetComponent<Renderer>();

        if (rippleRenderer != null)
            rippleRenderer.material.color = rippleColor;
    }

    public void Animate()
    {
        StartCoroutine(AnimateRipple(rippleDuration));
    }

    IEnumerator AnimateRipple(float duration)
    {
        if (rippleRenderer == null)
            yield break;

        if (!isAnimating)
        {
            isAnimating = true;
            rippleRenderer.enabled = true;

            // default values
            Vector3 scale = this.transform.localScale;
            Color color = rippleColor;

            int steps = (int)(duration / 0.01f);
            for (int i = 0; i < steps; i++)
            {
                float t = (float)(i + 1) / (float)(steps);
                this.transform.localScale = Vector3.Lerp(scale, scale * rippleScale, t);

                // fade alpha to zero
                color.a = rippleColor.a * (1 - QuadEaseOut(t));
                rippleRenderer.material.color = color;

                yield return new WaitForSeconds(0.01f);
            }
            
            this.transform.localScale = scale;
            rippleRenderer.material.color = rippleColor;
            rippleRenderer.enabled = false;
            isAnimating = false;
        }
    }

    static float QuadEaseOut(float t)
    {
        if (t <= 0.5f)
            return t;
        t -= 0.5f;
        return 2.0f * t * (1.0f - t) + 0.5f;
    }
}