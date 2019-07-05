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

public class ZoomCtrl : MonoBehaviour
{
    public float minScale = 0.1f;
    public float maxScale = 10f;
    public float damping = 0.5f;

    [Tooltip("Specify a special material for use when zooming or leave it None to use the default material.")]
    public Material materialWhenZooming;
    
    private Material materialDefault;
    private Renderer targetRenderer;

    private Vector3 minScaleTarget;
    private Vector3 maxScaleTarget;

    void Awake()
    {
        targetRenderer = this.GetComponent<Renderer>();
        
        if (targetRenderer != null)
            materialDefault = targetRenderer.material;

        if (materialWhenZooming == null)
            materialWhenZooming = materialDefault; // use default if not specified
    }

    void OnEnable()
    {
        ZoomManager.ZoomEvent += Scale;
        ZoomManager.EnterZoomEvent += EnterZoomCallback;
        ZoomManager.QuitZoomEvent += QuitZoomCallback;

        Vector3 scale = this.transform.localScale;
        minScaleTarget = new Vector3(scale.x * minScale, scale.y * minScale, scale.z * minScale);
        maxScaleTarget = new Vector3(scale.x * maxScale, scale.y * maxScale, scale.z * maxScale);
    }

    void OnDisable()
    {
        ZoomManager.ZoomEvent -= Scale;
        ZoomManager.EnterZoomEvent -= EnterZoomCallback;
        ZoomManager.QuitZoomEvent -= QuitZoomCallback;
    }

    void Scale(float displacement)
    {
        Vector3 scale = this.transform.localScale;
        float sx = scale.x + displacement * damping;
        float sy = scale.y + displacement * damping;
        float sz = scale.z + displacement * damping;

        if (sx >= minScaleTarget.x && sx <= maxScaleTarget.x &&
            sy >= minScaleTarget.y && sy <= maxScaleTarget.y &&
            sz >= minScaleTarget.z && sz <= maxScaleTarget.z)
        {
            this.transform.localScale = new Vector3(sx, sy, sz);
        }
    }

    void EnterZoomCallback()
    {
        if (targetRenderer != null && materialWhenZooming != null)
        {
            targetRenderer.material = materialWhenZooming;
        }
    }

    void QuitZoomCallback()
    {
        if (targetRenderer != null && materialDefault != null)
        {
            targetRenderer.material = materialDefault;
        }
    }
}
