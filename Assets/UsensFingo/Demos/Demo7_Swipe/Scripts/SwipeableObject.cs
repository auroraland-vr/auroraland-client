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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Fingo;

// Define custom UnityEvent that can pass on MoveDir arguments
[System.Serializable]
public class SwipeEvent : UnityEvent<MoveDir> { }

[System.Serializable]
public class MoveEvent : UnityEvent<float, MoveDir> { }

public class SwipeableObject : MonoBehaviour
{
    public static List<SwipeableObject> instances = new List<SwipeableObject>();

    public Collider boundingBox;

    public UnityEvent OnActivate;
    public UnityEvent OnDeactivate;
    public UnityEvent OnSwipeAnimStarted;
    public UnityEvent OnSwipeAnimCompleted;

    public SwipeEvent OnSwipe;
    public MoveEvent OnMove;
    
    private bool isFree = true;
    public bool IsAvailableForSwipe
    {
        get { return isFree; }
        set { isFree = value; }
    }

    private bool isAnimating = false;
    public bool IsAnimating
    {
        get { return isAnimating; }
        set { isAnimating = value; }
    }

    private struct TargetRenderer
    {
        public TargetRenderer(Renderer aRenderer, Color aColor)
        {
            renderer = aRenderer;
            defaultColor = aColor;
        }
        public Renderer renderer;
        public Color defaultColor;
    }
    private List<TargetRenderer> allRenderers = new List<TargetRenderer>();


    private void Awake()
    {
        Renderer renderer = this.GetComponent<Renderer>();
        if (renderer != null)
        {
            TargetRenderer targetRenderer = new TargetRenderer(renderer, renderer.material.color);
            allRenderers.Add(targetRenderer);
        }

        // iterate all the childern
        for (int i = 0; i < this.transform.childCount; i++)
        {
            Transform child = this.transform.GetChild(i);
            Renderer childRenderer = child.GetComponent<Renderer>();
            if (childRenderer != null)
            {
                TargetRenderer targetRenderer = new TargetRenderer(childRenderer, childRenderer.material.color);
                allRenderers.Add(targetRenderer);
            }
        }
    }

    void OnEnable()
    {
        instances.Add(this);

        if (OnActivate == null)
        {
            OnActivate = new UnityEvent();
        }
        if (OnDeactivate == null)
        {
            OnDeactivate = new UnityEvent();
        }
        if (OnSwipeAnimStarted == null)
        {
            OnSwipeAnimStarted = new UnityEvent();
        }
        if (OnSwipeAnimCompleted == null)
        {
            OnSwipeAnimCompleted = new UnityEvent();
        }
        if (OnSwipe == null)
        {
            OnSwipe = new SwipeEvent();
        }
        if (OnMove == null)
        {
            OnMove = new MoveEvent();
        }
    }

    void OnDisable()
    {
        instances.Remove(this);
    }

    public void SetTransparency(float alpha)
    {
        foreach (TargetRenderer targetRenderer in allRenderers)
        {
            Color color = targetRenderer.renderer.material.color;
            color.a = alpha;
            targetRenderer.renderer.material.color = color;
        }
    }

    public void SetHighlightColor(float scale)
    {
        foreach (TargetRenderer targetRenderer in allRenderers)
        {
            Color color = targetRenderer.defaultColor;
            color = new Color(
                color.r * scale,
                color.g * scale,
                color.b * scale,
                color.a);
            targetRenderer.renderer.material.color = color;
        }
    }

    public void SetDefaultColor()
    {
        foreach (TargetRenderer targetRenderer in allRenderers)
        {
            targetRenderer.renderer.material.color = targetRenderer.defaultColor;
        }
    }

    public Color GetColor()
    {
        if (allRenderers.Count > 0)
        {
            return allRenderers[0].renderer.material.color;
        }
        return Color.black;
    }
}
