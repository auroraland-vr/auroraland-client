using System.Collections;
using System.Collections.Generic;

using System;
using System.IO;
using System.IO.Compression;
using UnityEngine;
using UnityEditor;

public class AutomatedBuildProcess
{
    private static string windowsBuildFolderPath = "D:/build/spacecreator/";
    public static void Build()
    {
        // LevelBuildPrep.GenerateOptimizedScenesForEnabledScenesInBuildSettings();

        #region WINDOWS BUILD

        List<string> enabledScenePathNames = new List<string>();
        foreach (var scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled && scene.path != "")
            {
                enabledScenePathNames.Add(scene.path);
            }
        }

        string path = Path.Combine(windowsBuildFolderPath, System.DateTime.Now.ToString("MM-dd"));

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        string filename = "fantastic_" + System.DateTime.Now.ToString("MMdd-HHmm") + ".exe";

        string fullpath = Path.Combine(path, filename);
        Debug.Log("Starting to make Windows Build");

        // Set quality
        // QualitySettings.SetQualityLevel(5, true);
        Debug.LogFormat("Quality Level: {0}", QualitySettings.GetQualityLevel());

        BuildPlayerOptions options = new BuildPlayerOptions();
        options.scenes = enabledScenePathNames.ToArray();
        options.locationPathName = fullpath;
        options.target = BuildTarget.StandaloneWindows64;
        options.options = BuildOptions.None;
        BuildPipeline.BuildPlayer(options);

        #endregion
    }
}