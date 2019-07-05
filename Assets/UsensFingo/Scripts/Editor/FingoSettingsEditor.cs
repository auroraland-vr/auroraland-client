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
using UnityEditor;
using System.IO;

[InitializeOnLoad]
public class FingoSettingsEditor : EditorWindow
{

    static FingoSettingsEditor()
    {
        EditorApplication.update += Update;
    }

    static void Update()
    {
        if (PlayerSettings.apiCompatibilityLevel != ApiCompatibilityLevel.NET_2_0)
        {
            Debug.Log("PlayerSettings.apiCompatibilityLevel != ApiCompatibilityLevel.NET_2_0");
            PlayerSettings.apiCompatibilityLevel = ApiCompatibilityLevel.NET_2_0;
        }
    }
}
