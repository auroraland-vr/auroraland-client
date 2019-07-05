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

public class FadeColor : MonoBehaviour
{
    public float fadeDuration = 0.5f;

    private Renderer targetRenderer;
    private bool hidden = false;

    void Awake()
    {
        targetRenderer = this.GetComponent<Renderer>();
    }

    public void FadeOut()
    {
        if (!hidden)
        {
            StartCoroutine(AnimateAlpha(0, fadeDuration));
            hidden = true;
        }
    }

    public void FadeIn()
    {
        if (hidden)
        {
            StartCoroutine(AnimateAlpha(1, fadeDuration));
            hidden = false;
        }
    }

    IEnumerator AnimateAlpha(float targetAlpha, float duration)
    {
        Color color = targetRenderer.material.color;
        float alpha = color.a;
        
        int steps = Mathf.Max(1, (int)(duration / 0.01f));
        for (int i = 0; i < steps; i++)
        {
            float t = (float)(i + 1) / (float)(steps);
            color.a = alpha * (1 - t) + targetAlpha * t;
            targetRenderer.material.color = color;

            if (i < steps - 1)
            {
                yield return new WaitForSeconds(0.01f);
            }
        }
    }
}