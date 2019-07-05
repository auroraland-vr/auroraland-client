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

[CustomEditor(typeof(CollisionHand)), CanEditMultipleObjects]
public class CollisionHandEditor : FingoEditor {

    Texture buttonTextureGreen;
    Texture buttonTextureRed;
    CollisionHand myCollisionHand;

    void OnEnable()
    {
        var resourcePath = GetResourcePath();

        buttonTextureGreen = AssetDatabase.LoadAssetAtPath<Texture2D>(resourcePath + "Textures/Editor/Green.png");
        buttonTextureRed = AssetDatabase.LoadAssetAtPath<Texture2D>(resourcePath + "Textures/Editor/Red.png");
    }


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        myCollisionHand = (CollisionHand)target;
        #region hand
        var rect = GUILayoutUtility.GetRect(Screen.width - 30, 400, GUI.skin.box);
        GUI.Box(rect, "Hand Collider Map");
        Rect objectRect = new Rect();
        objectRect.position = new Vector2(rect.position.x + rect.size.x / 6, rect.position.y + rect.size.y * 9 / 12);
        objectRect.size = new Vector2(rect.size.x / 12 , rect.size.y / 6);
        setBoneButton(0, objectRect);
        objectRect.position -= new Vector2(rect.size.x / 24, rect.size.y / 6);
        setBoneButton(1, objectRect);
        objectRect.position -= new Vector2(rect.size.x / 24, rect.size.y / 6);
        setBoneButton(2, objectRect);
        objectRect.position = new Vector2(rect.position.x + rect.size.x / 3, rect.position.y + rect.size.y * 5 / 12);
        setBoneButton(3, objectRect);
        objectRect.position -= new Vector2(0, rect.size.y / 6);
        setBoneButton(4, objectRect);
        objectRect.position -= new Vector2(0, rect.size.y / 6);
        setBoneButton(5, objectRect);
        objectRect.position = new Vector2(rect.position.x + rect.size.x * 6 / 12, rect.position.y + rect.size.y * 5 / 12);
        setBoneButton(6, objectRect);
        objectRect.position -= new Vector2(0, rect.size.y / 6);
        setBoneButton(7, objectRect);
        objectRect.position -= new Vector2(0, rect.size.y / 6);
        setBoneButton(8, objectRect);
        objectRect.position = new Vector2(rect.position.x + rect.size.x * 8 / 12, rect.position.y + rect.size.y * 5 / 12);
        setBoneButton(9, objectRect);
        objectRect.position -= new Vector2(0, rect.size.y / 6);
        setBoneButton(10, objectRect);
        objectRect.position -= new Vector2(0, rect.size.y / 6);
        setBoneButton(11, objectRect);
        objectRect.position = new Vector2(rect.position.x + rect.size.x * 10 / 12, rect.position.y + rect.size.y * 5 / 12);
        setBoneButton(12, objectRect);
        objectRect.position -= new Vector2(0, rect.size.y / 6);
        setBoneButton(13, objectRect);
        objectRect.position -= new Vector2(0, rect.size.y / 6);
        setBoneButton(14, objectRect);
        objectRect.position = new Vector2(rect.position.x + rect.size.x / 3, rect.position.y + rect.size.y * 7 / 12);
        objectRect.size = new Vector2(rect.size.x * 7 / 12, rect.size.y / 3);
        setPalmButton(objectRect);
        #endregion hand

        if(GUILayout.Button("Enable All Collider"))
        {
            myCollisionHand.EnableAllCollision();
        }
        if(GUILayout.Button("Disable All Collider"))
        {
            myCollisionHand.DisableAllCollision();
        }
    }

    private void setBoneButton(int boneIndex, Rect rect)
    {
        if (GUI.Button(rect, ""))
        {
            myCollisionHand.EnableBone[boneIndex] = !myCollisionHand.EnableBone[boneIndex];
        }
        Rect textureRect = new Rect(rect.position.x + 4, rect.position.y + 4, rect.width - 8, rect.height - 8);
        GUI.DrawTexture(textureRect, myCollisionHand.EnableBone[boneIndex] ? buttonTextureGreen : buttonTextureRed);
    }

    private void setPalmButton(Rect rect)
    {
        if (GUI.Button(rect, ""))
        {
            myCollisionHand.EnablePalm = !myCollisionHand.EnablePalm;
        }
        Rect textureRect = new Rect(rect.position.x + 4, rect.position.y + 4, rect.width - 8, rect.height - 8);
        GUI.DrawTexture(textureRect, myCollisionHand.EnablePalm ? buttonTextureGreen : buttonTextureRed);
    }
}
