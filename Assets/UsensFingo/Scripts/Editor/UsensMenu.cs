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


public class UsensMenu : MonoBehaviour
{

    [MenuItem("uSens/Documentation/Developers Site", false, 100)]
    private static void OpenDocumentation()
    {
        Application.OpenURL("https://developers.usens.com");
    }

    [MenuItem("uSens/Create Hand/Stick Hand Left", false, 100)]
    private static void CreateLeftStickHand()
    {
        loadHand("StickHand_L");
    }

    [MenuItem("uSens/Create Hand/Stick Hand Right", false, 100)]
    private static void CreateRightStickHand()
    {
        loadHand("StickHand_R");
    }

    [MenuItem("uSens/Create Hand/Mesh Hand Left", false, 100)]
    private static void CreateLeftMeshHand()
    {
        loadHand("MeshHand_L");
    }

    [MenuItem("uSens/Create Hand/Mesh Hand Right", false, 100)]
    private static void CreateRightMeshHand()
    {
        loadHand("MeshHand_R");
    }

    [MenuItem("uSens/Create Hand/Collision Hand Left", false, 100)]
    private static void CreateLeftCollisionHand()
    {
        loadHand("CollisionHand_L");
    }

    [MenuItem("uSens/Create Hand/Collision Hand Right", false, 100)]
    private static void CreateRightCollisionHand()
    {
        loadHand("CollisionHand_R");
    }

    [MenuItem("uSens/Create Hand/Transparent Hand Left", false, 100)]
    private static void CreateLeftTransparentHand()
    {
        loadHand("TransparentHand_L");
    }

    [MenuItem("uSens/Create Hand/Transparent Hand Right", false, 100)]
    private static void CreateRightTransparentHand()
    {
        loadHand("TransparentHand_R");
    }

    [MenuItem("uSens/Service Mode/Local", false, 100)]
    private static void EdgeMode()
    {
        EdgeMode("Local");
    }

    [MenuItem("uSens/Service Mode/Remote", false, 100)]
    private static void RemoteMode()
    {
        EdgeMode("Remote");
    }

    private static void loadHand(string name)
    {
        GameObject hand = GameObject.Instantiate((GameObject)AssetDatabase.LoadAssetAtPath("Assets/UsensFingo/Prefab/Hands/" + name + ".prefab", typeof(GameObject)));
        hand.name = name;
    }

    private static void EdgeMode(string name)
    {

        if (name == "Local")
        {
            ScriptingDefineHelper.Remove(ScriptingDefineHelper.Remote);
            ScriptingDefineHelper.Add(ScriptingDefineHelper.LOCAL);
            EditorUtility.DisplayDialog("SUCCEED", "LOCAL Mode Selected! Please switch to another window then back, to let Unity eidtor update", "OK");
        }


        if (name == "Remote")
        {
            ScriptingDefineHelper.Remove(ScriptingDefineHelper.LOCAL);
            ScriptingDefineHelper.Add(ScriptingDefineHelper.Remote);
            EditorUtility.DisplayDialog("SUCCEED", "Remote Mode Selected! Please switch to another window then back, to let Unity eidtor update", "OK");
        }

        Menu.SetChecked("uSens/Service Mode/Local", ScriptingDefineHelper.Contains(ScriptingDefineHelper.LOCAL));
        Menu.SetChecked("uSens/Service Mode/Remote", ScriptingDefineHelper.Contains(ScriptingDefineHelper.Remote));
        EditorPrefs.SetBool("uSens/Service Mode/Local", ScriptingDefineHelper.Contains(ScriptingDefineHelper.LOCAL));
        EditorPrefs.SetBool("uSens/Service Mode/Remote", ScriptingDefineHelper.Contains(ScriptingDefineHelper.Remote));

    }



}



public class ScriptingDefineHelper
{
    public const string LOCAL = "LOCAL";
    public const string Remote = "Remote";

    public static BuildTargetGroup TargetGroup = BuildTargetGroup.Android;

    private static string[] Symbols;

    public static void Add(string NewOne)
    {
        if (Contains(NewOne))
        {
            return;
        }

        Append(NewOne);
    }

    public static void AddUnique(string NewOne)
    {
        Clear();

        Append(NewOne);
    }

    public static void Remove(string OldOne)
    {
        if (Contains(OldOne))
        {
            Clear();

            foreach (var s in Symbols)
            {
                if (s != OldOne)
                {
                    Append(s);
                }
            }
        }
    }

    public static bool Contains(string OldOne)
    {
        Split();

        foreach (var s in Symbols)
        {
            if (s == OldOne)
            {
                return true;
            }
        }

        return false;
    }

    private static void Split()
    {
        Symbols = GetSymbols().Split(';');
    }

    private static string GetSymbols()
    {
        return PlayerSettings.GetScriptingDefineSymbolsForGroup(TargetGroup);
    }

    private static void Append(string NewOne)
    {
        PlayerSettings.SetScriptingDefineSymbolsForGroup(TargetGroup, GetSymbols() + ";" + NewOne);
    }

    private static void Clear()
    {
        PlayerSettings.SetScriptingDefineSymbolsForGroup(TargetGroup, "");
    }
}
