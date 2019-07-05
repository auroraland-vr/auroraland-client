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

public class OutlineCtrl : MonoBehaviour
{
    // Variate outline color transparency when the distance between object and hand is within:
    public float distanceMin = 0.08f;
    public float distanceMax = 0.2f;

    // Variate outline color transparency between: 
    public float alphaDistanceMin = 1f;
    public float alphaDistanceMax = 0f;

    private Renderer targetRenderer;

    void Awake()
    {
        targetRenderer = this.GetComponent<Renderer>();
    }

    public void SetOutlineColorTransparency(float alpha)
    {
        if (targetRenderer != null && targetRenderer.material.HasProperty("_OutlineColor"))
        {
            Color color = targetRenderer.material.GetColor("_OutlineColor");
            color.a = alpha;
            targetRenderer.material.SetColor("_OutlineColor", color);
        }
    }

    public void UpdateOutlineColorBasedOnDistance(float distance)
    {
        if (targetRenderer != null && targetRenderer.material.HasProperty("_OutlineColor"))
        {
            // At distanceMin: alpha = alphaDistanceMin (e.g. more oapque)
            // At distanceMax: alpha = alphaDistanceMax (e.g. more transparent)
            // In between: alpha is a lerp based on distance
            float factor = Normalize(distance, distanceMin, distanceMax);
            float alpha = (1-factor) * alphaDistanceMin + factor * alphaDistanceMax;

            // Update outline color transparency on material
            Color color = targetRenderer.material.GetColor("_OutlineColor");
            color.a = alpha;
            targetRenderer.material.SetColor("_OutlineColor", color);
        }
    }
    
    static float Normalize(float input, float min, float max)
    {
        if (min == max)
        {
            return 0;
        }
        else
        {
            if (min > max)
            {
                // swap min and max
                float tmp = min;
                min = max;
                max = tmp;
            }
            return ((input <= min) ? 0 : ((input >= max) ? 1 : (input - min) / (max - min)));
        }
    }
}
