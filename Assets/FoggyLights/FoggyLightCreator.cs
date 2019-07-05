#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Reflection;

public class FoggyLightCreator : Editor
{


    [UnityEditor.MenuItem("GameObject/Create Other/Foggy Light")]
    static public void CreateFoggyLight()
    {
        
        GameObject FoggyLight = new GameObject();

        //Icon stuff
        Texture Icon = Resources.Load("FoggyLightsIcon") as Texture;
        Icon.hideFlags = HideFlags.HideAndDontSave;
        var editorGUI = typeof(EditorGUIUtility);
        var bindingFlags = BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.NonPublic;
        var args = new object[] { FoggyLight, Icon };
        editorGUI.InvokeMember("SetIconForObject", bindingFlags, null, null, args);

        FoggyLight.name = "Foggy Light";
        
        FoggyLight.AddComponent<MeshFilter>();
        FoggyLight.AddComponent<MeshRenderer>();
        FoggyLight.AddComponent<FoggyLight>();
        FoggyLight.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        FoggyLight.GetComponent<Renderer>().reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
        FoggyLight.GetComponent<Renderer>().receiveShadows = false;
        
        GameObject FoggyLightMeshGameObject = Resources.Load("FoggyLight") as GameObject;
        FoggyLight.GetComponent<MeshFilter>().sharedMesh = FoggyLightMeshGameObject.GetComponent<MeshFilter>().sharedMesh;
        Selection.activeObject = FoggyLight;
        if (UnityEditor.SceneView.currentDrawingSceneView) UnityEditor.SceneView.currentDrawingSceneView.MoveToView(FoggyLight.transform);
    }
    
}
#endif