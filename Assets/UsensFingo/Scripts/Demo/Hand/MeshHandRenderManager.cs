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
using Fingo;

public enum HighlightedHandType
{
    Left,
    Right,
    Both,
    None
}

/// <summary>
///
/// </summary>
public class MeshHandRenderManager : MonoBehaviour
{
    /// <summary>
    /// The hand type to be highlighted
    /// </summary>
    public HighlightedHandType highlightedHandType;

    private struct HighlightedHand
    {
        public Renderer meshRenderer;
        public Material defaultMaterial;
    }
    private List<HighlightedHand> highlightedHands = new List<HighlightedHand>();
    
    // A uniform transparent material for all types of mesh hands
    public Material transparentMaterial;
    

    void OnEnable()
    {
        if (highlightedHandType == HighlightedHandType.Left || highlightedHandType == HighlightedHandType.Both)
        {
            GameObject handMeshObject = GameObject.FindWithTag("LeftHandMesh");
            if (handMeshObject != null && handMeshObject.GetComponent<SkinnedMeshRenderer>() != null)
            {
                HighlightedHand hand;
                hand.meshRenderer = handMeshObject.GetComponent<SkinnedMeshRenderer>();
                hand.defaultMaterial = handMeshObject.GetComponent<SkinnedMeshRenderer>().material;
                highlightedHands.Add(hand);
            }
        }
        if (highlightedHandType == HighlightedHandType.Right || highlightedHandType == HighlightedHandType.Both)
        {
            GameObject handMeshObject = GameObject.FindWithTag("RightHandMesh");
            if (handMeshObject != null && handMeshObject.GetComponent<SkinnedMeshRenderer>() != null)
            {
                HighlightedHand hand;
                hand.meshRenderer = handMeshObject.GetComponent<SkinnedMeshRenderer>();
                hand.defaultMaterial = handMeshObject.GetComponent<SkinnedMeshRenderer>().material;
                highlightedHands.Add(hand);
            }
        }
    }

    public void EnableHandOutline()
    {
        foreach (HighlightedHand hand in highlightedHands)
        {
            hand.meshRenderer.material.EnableKeyword("_OUTLINE_ON");
            hand.meshRenderer.material.DisableKeyword("_OUTLINE_OFF");
        }
    }

    public void DisableHandOutline()
    {
        foreach (HighlightedHand hand in highlightedHands)
        {
            hand.meshRenderer.material.EnableKeyword("_OUTLINE_OFF");
            hand.meshRenderer.material.DisableKeyword("_OUTLINE_ON");
        }
    }

    public void FadeOut()
    {
        if (transparentMaterial != null)
        {
            foreach (HighlightedHand hand in highlightedHands)
            {
                hand.meshRenderer.material = transparentMaterial;
            }
        }
    }

    public void FadeIn()
    {
        foreach (HighlightedHand hand in highlightedHands)
        {
            hand.meshRenderer.material = hand.defaultMaterial;
        }
    }
}
