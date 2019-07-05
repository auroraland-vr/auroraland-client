/*1.06 CHANGE LOG
 * Tonemap is optional now
 * Intersection softness between effect and scene can be adjusted now
 * Color, exposure, tonemap and blend corrections
 * */

using UnityEngine;
using System.Collections;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]


public class FoggyLight : MonoBehaviour
{


    [SerializeField]
    GameObject FogVolumeContainer = null;
    public bool InsideFogVolume = false;
    
    public enum BlendModeEnum { Additive, AlphaBlended };
    public BlendModeEnum BlendMode=0;
    public bool ApplyTonemap = true;
    private Component FogVolumeComponent = null;

    public Color PointLightColor = Color.white;
    Vector3 Position;
    [Range(0, 8)]
    public float PointLightIntensity = 1;
    [Range(0, 20)]
    public float FoggyLightIntensity = 1;
    public float PointLightExponent = 5, Offset = -2;
    [Range(1, 40)]
    public float IntersectionRange = 2;
    public int DrawOrder = 1;
    public bool HideWireframe = true, AttatchLight = false;
    Light AttachedLight = null;
    Material FoggyLightMaterial;
   
    public Material GetMaterial()
    {
        
            return FoggyLightMaterial;
        
    }
  
    static public void Wireframe(GameObject Obj, bool Enable)
    {
        #if UNITY_EDITOR
                EditorUtility.SetSelectedWireframeHidden(Obj.GetComponent<Renderer>(), Enable);
        #endif
    }
    void CreateMaterial()
    {
        if (!FoggyLightMaterial)
        {
            FoggyLightMaterial = new Material(Shader.Find("Hidden/FoggyLight"));
            FoggyLightMaterial.name = this.name.ToString() + " Material";
            gameObject.GetComponent<Renderer>().sharedMaterial = FoggyLightMaterial;
            FoggyLightMaterial.hideFlags = HideFlags.HideAndDontSave;
        }
    }
    void Start()
    {
        
    }
  
    void OnEnable()
    {

        CreateMaterial();
       
      
    }

    void OnWillRenderObject()
    {
     
      
        GetComponent<Renderer>().sortingOrder = DrawOrder;
        Position = gameObject.transform.position;
        PointLightExponent = Mathf.Max(1, PointLightExponent);
  
        #if UNITY_EDITOR
                Wireframe(gameObject, HideWireframe);
        #endif

        Position = gameObject.transform.position;
        GetComponent<Renderer>().sharedMaterial.SetColor("PointLightColor", PointLightColor);

        if (FogVolumeContainer && InsideFogVolume)
        {
            if (!FogVolumeComponent)
                FogVolumeComponent = FogVolumeContainer.GetComponent("FogVolume");


            FoggyLightMaterial.EnableKeyword("_FOG_CONTAINER");
            //            renderer.sharedMaterial.SetFloat("_Visibility", FogVolumeComponent.Visibility);
            float valueVisibility = (float)FogVolumeComponent.GetType().GetMethod("GetVisibility").Invoke(FogVolumeComponent, null);
            GetComponent<Renderer>().sharedMaterial.SetFloat("_Visibility", valueVisibility);



        }
        else
            FoggyLightMaterial.DisableKeyword("_FOG_CONTAINER");
        if(ApplyTonemap)
            FoggyLightMaterial.EnableKeyword("TONEMAP");
        else
            FoggyLightMaterial.DisableKeyword("TONEMAP");

        FoggyLightMaterial.SetVector("PointLightPosition", Position);
        FoggyLightMaterial.SetFloat("PointLightIntensity", PointLightIntensity * FoggyLightIntensity);
        FoggyLightMaterial.SetFloat("PointLightExponent", PointLightExponent);
        FoggyLightMaterial.SetFloat("Offset", Offset);
        FoggyLightMaterial.SetFloat("IntersectionRange", IntersectionRange);
        


        if (AttatchLight)
        {
            if (!gameObject.GetComponent<Light>())
            {
                gameObject.AddComponent<Light>();
                gameObject.GetComponent<Light>().shadows = LightShadows.Hard;
            }

            AttachedLight = gameObject.GetComponent<Light>();

            if (AttachedLight)
            {
                AttachedLight.intensity = PointLightIntensity / 2;
                AttachedLight.color = PointLightColor;

                AttachedLight.enabled = true;
            }
        }
        else
        {            
                if (AttachedLight)

                    AttachedLight.enabled = false;
           
        }


        BlendValues(BlendMode);
    }

    void BlendValues(BlendModeEnum BlendMode)
    {
       
        switch (BlendMode)
        {
            case BlendModeEnum.Additive:
                FoggyLightMaterial.EnableKeyword("_ADDITIVE");
                FoggyLightMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                FoggyLightMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
                break;
                
            case BlendModeEnum.AlphaBlended:
                 FoggyLightMaterial.DisableKeyword("_ADDITIVE");
                 FoggyLightMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                 FoggyLightMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                 break;
                
        }
    }
}
