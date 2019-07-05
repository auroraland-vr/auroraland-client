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
using Fingo;

[CustomEditor(typeof(FingoMain)), CanEditMultipleObjects]
public class FingoMainEditor : FingoEditor
{
    Texture uSensLogo;

    bool enableHand;
    bool enableController;
    bool enableGesture;
    bool enableIRImage;
    bool enableRGBImage;
    bool enableHeadTracking;

    FingoMain fingoMain;

    void OnEnable()
    {
        fingoMain = (FingoMain)target;
        var resourcePath = GetResourcePath();
        uSensLogo = AssetDatabase.LoadAssetAtPath<Texture2D>(resourcePath + "Textures/uSens_Logo.png");
    }

    public override void OnInspectorGUI()
    {
        var rect = GUILayoutUtility.GetRect(Screen.width - 38, 50, GUI.skin.box);
        if (uSensLogo)
        {
            GUI.DrawTexture(rect, uSensLogo, ScaleMode.ScaleToFit);
        }

        //base.OnInspectorGUI();
        serializedObject.Update();
        

        if (fingoMain.TestDevicesCapability(FingoCapability.Hand))
        {
            enableHand = serializedObject.FindProperty("enableHandTracking").boolValue;
            GUILayout.Label("Hand Tracking: " + (enableHand ? "On" : "Off"));
            if(enableHand)
            {
                if (GUILayout.Button("Switch Off"))
                {
                    fingoMain.SetHandTrackingEnable(false);
                    serializedObject.FindProperty("enableHandTracking").boolValue = false;
                    serializedObject.ApplyModifiedProperties();
                }
            }
            else
            {
                if (GUILayout.Button("Switch On"))
                {
                    fingoMain.SetHandTrackingEnable(true);
                    serializedObject.FindProperty("enableHandTracking").boolValue = true;
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }

        if (fingoMain.TestDevicesCapability(FingoCapability.Controller))
        {
            enableController = serializedObject.FindProperty("enableControllerTracking").boolValue;
            GUILayout.Label("Controller Tracking: " + (enableController ? "On" : "Off"));
            if (enableController)
            {
                if (GUILayout.Button("Switch Off"))
                {
                    fingoMain.SetControllerTrackingEnable(false);
                    serializedObject.FindProperty("enableControllerTracking").boolValue = false;
                    serializedObject.ApplyModifiedProperties();
                }
            }
            else
            {
                if (GUILayout.Button("Switch On"))
                {
                    fingoMain.SetControllerTrackingEnable(true);
                    serializedObject.FindProperty("enableControllerTracking").boolValue = true;
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }

        if (fingoMain.TestDevicesCapability(FingoCapability.Gesture))
        {
            enableGesture = serializedObject.FindProperty("enableGestureDetection").boolValue;
            GUILayout.Label("Gesture Recognition: " + (enableGesture ? "On" : "Off"));
            if (enableGesture)
            {
                if (GUILayout.Button("Switch Off"))
                {
                    fingoMain.SetGestureDetectionEnable(false);
                    serializedObject.FindProperty("enableGestureDetection").boolValue = false;
                    serializedObject.ApplyModifiedProperties();
                }
            }
            else
            {
                if (GUILayout.Button("Switch On"))
                {
                    fingoMain.SetGestureDetectionEnable(true);
                    serializedObject.FindProperty("enableGestureDetection").boolValue = true;
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }
        if (fingoMain.TestDevicesCapability(FingoCapability.InfraredImage))
        {
            enableIRImage = serializedObject.FindProperty("enableInfraredImage").boolValue;
            GUILayout.Label("Infrared Image: " + (enableIRImage ? "On" : "Off"));
            if (enableIRImage)
            {
                if (GUILayout.Button("Switch Off"))
                {
                    fingoMain.SetIRImageEnable(false);
                    serializedObject.FindProperty("enableInfraredImage").boolValue = false;
                    serializedObject.ApplyModifiedProperties();
                }
            }
            else
            {
                if (GUILayout.Button("Switch On"))
                {
                    fingoMain.SetIRImageEnable(true);
                    serializedObject.FindProperty("enableInfraredImage").boolValue = true;
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }
        if (fingoMain.TestDevicesCapability(FingoCapability.RGBImage))
        {
            enableRGBImage = serializedObject.FindProperty("enableColorImage").boolValue;
            GUILayout.Label("Color Image: " + (enableRGBImage ? "On" : "Off"));
            if (enableRGBImage)
            {
                if (GUILayout.Button("Switch Off"))
                {
                    fingoMain.SetRGBImageEnable(false);
                    serializedObject.FindProperty("enableColorImage").boolValue = false;
                    serializedObject.ApplyModifiedProperties();
                }
            }
            else
            {
                if (GUILayout.Button("Switch On"))
                {
                    fingoMain.SetRGBImageEnable(true);
                    serializedObject.FindProperty("enableColorImage").boolValue = true;
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }
        if (fingoMain.TestDevicesCapability(FingoCapability.Marker) || fingoMain.TestDevicesCapability(FingoCapability.Slam))
        {
            enableHeadTracking = serializedObject.FindProperty("enableHeadTracking").boolValue;
            GUILayout.Label("Head tracking: " + (enableHeadTracking ? "On" : "Off"));
            if (enableHeadTracking)
            {
                if (GUILayout.Button("Switch Off"))
                {
                    fingoMain.SetHeadTrackingEnable(false);
                    serializedObject.FindProperty("enableHeadTracking").boolValue = false;
                    serializedObject.ApplyModifiedProperties();
                }
            }
            else
            {
                if (GUILayout.Button("Switch On"))
                {
                    fingoMain.SetHeadTrackingEnable(true);
                    serializedObject.FindProperty("enableHeadTracking").boolValue = true;
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }
    }
}