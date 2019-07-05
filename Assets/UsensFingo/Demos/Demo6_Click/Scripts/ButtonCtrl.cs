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

public class ButtonCtrl : MonoBehaviour
{
    public GameObject switchOffOn;
    public GameObject switchOnOff;
    public GameObject buttonBase;
    public Color baseHoverColor;

    [HideInInspector]
    public bool isOn = false;

    private Animation turnOnAnimation;
    private Animation turnOffAnimation;

    private Renderer switchOffOnRenderer;
    private Renderer switchOnOffRenderer;
    private Renderer baseRenderer;
    private Color baseNormalColor;


    Renderer GetSwitchRenderer(GameObject aswitch)
    {
        Renderer switchRenderer = aswitch.GetComponent<Renderer>();
        if (switchRenderer == null)
        {
            // Find renderer component on child instead
            Transform child = aswitch.transform.GetChild(0);
            if (child != null)
                switchRenderer = child.GetComponent<Renderer>();
        }
        return switchRenderer;
    }

    void Awake()
    {
        if (switchOffOn != null)
        {
            switchOffOnRenderer = GetSwitchRenderer(switchOffOn);
            turnOnAnimation = switchOffOn.GetComponent<Animation>();
        }

        if (switchOnOff != null)
        {
            switchOnOffRenderer = GetSwitchRenderer(switchOnOff);
            turnOffAnimation = switchOnOff.GetComponent<Animation>();
        }

        if (buttonBase != null)
        {
            baseRenderer = buttonBase.GetComponent<Renderer>();
            if (baseRenderer != null)
                baseNormalColor = baseRenderer.material.color;
        }
    }

    void OnEnable()
    {
        if (switchOffOn != null)
            switchOffOn.SetActive(!isOn);

        if (switchOnOff != null)
            switchOnOff.SetActive(isOn);
    }
    
    public void SetButtonToNormalState()
    {
        // Set Normal color
        SetEmissionColor(switchOffOnRenderer, Color.black);
        SetEmissionColor(switchOnOffRenderer, Color.black);
        SetColor(baseRenderer, baseNormalColor);

        // Make button opaque
        SetButtonTransparency(1f);
    }

    public void SetButtonToHoverState()
    {
        // Set Hover color
        SetEmissionColor(switchOffOnRenderer, baseHoverColor);
        SetEmissionColor(switchOnOffRenderer, baseHoverColor);
        SetColor(baseRenderer, baseHoverColor);
    }

    public void SetButtonToPressedState(float alpha)
    {
        // Set Pressed color
        SetEmissionColor(switchOffOnRenderer, Color.black);
        SetEmissionColor(switchOnOffRenderer, Color.black);
        SetColor(baseRenderer, baseNormalColor);

        // Set color transparency
        SetButtonTransparency(alpha);

        ToggleSwitch();
    }

    void ToggleSwitch()
    {
        if (switchOffOn != null)
            switchOffOn.SetActive(!isOn);

        if (switchOnOff != null)
            switchOnOff.SetActive(isOn);

        if (!isOn)
        {
            if (turnOnAnimation != null)
                turnOnAnimation.Play();
        }
        else
        {
            if (turnOffAnimation != null)
                turnOffAnimation.Play();
        }
        
        isOn = !isOn;
    }

    public void SetButtonTransparency(float alpha)
    {
        SetTransparency(switchOffOnRenderer, alpha);
        SetTransparency(switchOnOffRenderer, alpha);
        SetTransparency(baseRenderer, alpha);
    }

    void SetColor(Renderer targetRenderer, Color color)
    {
        if (targetRenderer != null)
        {
            targetRenderer.material.color = color;
        }
    }

    void SetEmissionColor(Renderer targetRenderer, Color color)
    {
        if (targetRenderer != null)
        {
            targetRenderer.material.SetColor("_EmissionColor", color);
        }
    }

    void SetTransparency(Renderer targetRenderer, float alpha)
    {
        if (targetRenderer != null)
        {
            Color color = targetRenderer.material.color;
            color.a = alpha;
            targetRenderer.material.color = color;
        }
    }
}
