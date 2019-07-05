using UnityEngine;
using System.Collections;
using UnityEditor;

public class ExportTags : MonoBehaviour
{


    [MenuItem("uSens/Export Package with Tags", false, 100)]
    private static void CreateLeftStickHand()
    {
        ExportAll();
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private static void ExportAll()
    {

        string[] projectContent = AssetDatabase.GetAllAssetPaths();
        AssetDatabase.ExportPackage(projectContent, "FingoUnitySDK1_1_0.unitypackage", ExportPackageOptions.Recurse | ExportPackageOptions.IncludeLibraryAssets);
        Debug.Log("Project Exported");
    }


}
