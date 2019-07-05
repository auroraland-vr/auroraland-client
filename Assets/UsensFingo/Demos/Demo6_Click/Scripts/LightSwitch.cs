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

public class LightSwitch : MonoBehaviour
{
    public GameObject button;
    public GameObject alight;

    public Material lightOnMaterial;
    public Material lightOffMaterial;

    private ButtonCtrl buttonCtrl;
    private Renderer targetRenderer;
    
    
    void Awake()
    {
        if (button != null)
            buttonCtrl = button.GetComponent<ButtonCtrl>();

        targetRenderer = this.GetComponent<Renderer>();
    }

    public void ToggleLight()
    {
        if (buttonCtrl != null && targetRenderer != null)
        {
            if (buttonCtrl.isOn)
            {
                if (lightOnMaterial != null)
                    targetRenderer.material = lightOnMaterial;
                if (alight != null)
                    alight.SetActive(true);
            }
            else
            {
                if (lightOffMaterial != null)
                    targetRenderer.material = lightOffMaterial;
                if (alight != null)
                    alight.SetActive(false);
            }
        }
    }
}
