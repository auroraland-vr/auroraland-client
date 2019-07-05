using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Auroraland;
using VRTK;
using VRTK.GrabAttachMechanics;
using VRTK.SecondaryControllerGrabActions;
using VRTK.Highlighters;
using Auroraland.Networking;

public class MeshMerge : MonoBehaviour
{
    public float OutlineWidth = 0.008f;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Dictionary<Material, List<CombineInstance>> subMeshesDict = new Dictionary<Material, List<CombineInstance>>();
    private bool hasMeshOnRoot;

    // Use this for initialization
    public void Generate()
    {

        meshFilter = transform.GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
        }
        meshRenderer = transform.GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshRenderer.material = null;
        }

        hasMeshOnRoot = true;
        if (meshFilter.mesh.vertexCount == 0)
        {
            hasMeshOnRoot = false;
            Debug.Log(gameObject.name + "does not have mesh at root");
        }

        Debug.Log("Generate mesh:" + gameObject.name);
        //track materials and submesh

        foreach (MeshFilter filter in transform.GetComponentsInChildren<MeshFilter>(false).ToList().FindAll(filter => filter != meshFilter || hasMeshOnRoot))
        {
            for (int i = 0; i < filter.mesh.subMeshCount; i++)
            {

                Material mat = filter.GetComponent<MeshRenderer>().sharedMaterials[i];
                if (!subMeshesDict.ContainsKey(mat)) //new material, new list
                {
                    List<CombineInstance> combineInstance = new List<CombineInstance>();
                    subMeshesDict.Add(mat, combineInstance);
                }
                CombineInstance instance = new CombineInstance();
                instance.mesh = filter.mesh;
                instance.subMeshIndex = i;
                instance.transform = filter.transform.localToWorldMatrix;
                subMeshesDict[mat].Add(instance);
                //Debug.LogFormat("add mesh {0}-submesh{1}-mat{2}", filter.name, i, mat.name);
            }

        }
        List<Material> materials = subMeshesDict.Keys.ToList();
        List<Mesh> combinedMeshes = new List<Mesh>();

        // flatten submeshes into a single mesh if they have sam materials
        foreach (KeyValuePair<Material, List<CombineInstance>> pair in subMeshesDict)
        {
            Mesh mesh = new Mesh();
            mesh.CombineMeshes(pair.Value.ToArray(), true);
            combinedMeshes.Add(mesh);
        }

        //assign final mesh to root mesh filter
        Mesh finalMesh = new Mesh();
        finalMesh = CombineMeshWithDifferentMaterial(combinedMeshes);
        meshFilter.sharedMesh = finalMesh;
        meshFilter.GetComponent<MeshRenderer>().sharedMaterials = materials.ToArray();

        //clean up unused space and mesh
        subMeshesDict.Clear();
        foreach (Mesh mesh in combinedMeshes)
        {
            Destroy(mesh);
        }
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
            Destroy(child.gameObject);
        }

        //Add Mesh Collider
        if (meshFilter.sharedMesh != null)
        {
            // 0. Assign mesh to root game object
            var collider = GetComponent<MeshCollider>();
            if (collider)
            {
                DestroyImmediate(collider);
            }
            if (collider == null)
            {
                collider = gameObject.AddComponent<MeshCollider>() as MeshCollider;
            }
            collider.sharedMesh = meshFilter.sharedMesh;
            collider.cookingOptions = MeshColliderCookingOptions.InflateConvexMesh;
            collider.convex = true;
            collider.inflateMesh = true;
        }

        // 1. Add Rigid Body
        AddRigidbody(gameObject);

        //2.Add NetObject
        //AddNetObject(obj);

        // 3. Add VRTK interactable Object script
        InteractableAsset interactableAsset = AddInteractableObjectComponent(gameObject);

        // HACK Sanity check for board
        if (name.Contains("board") && GetComponent<SegmentId>() != null)
            interactableAsset.isGrabbable = false;

        // 4. Add VRTK Fixed Joint Grab Attach script
        AddGrabMechanicScript(gameObject, interactableAsset);
        // 5. Add VRTK Swap Controller Grab Action script
        AddSecondaryGrabActionScript(gameObject, interactableAsset);

        // 6. Add VRTK Outline Object script
        AddOutlineHighlighter(gameObject);
        
    }

    Mesh CombineMeshWithDifferentMaterial(List<Mesh> submeshList)
    {
        Matrix4x4 myMatrix = transform.worldToLocalMatrix;
        List<CombineInstance> finalCombiners = new List<CombineInstance>();
        for (int i = 0; i < submeshList.Count; i++)
        {
            CombineInstance instance = new CombineInstance();
            instance.mesh = submeshList[i];
            instance.subMeshIndex = 0;
            instance.transform = myMatrix * Matrix4x4.identity;
            finalCombiners.Add(instance);
        }
        Mesh finalMesh = new Mesh();
        finalMesh.CombineMeshes(finalCombiners.ToArray(), false);
        return finalMesh;
    }

    public void AddRigidbody(GameObject obj) {
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb && obj.GetComponent<SnapToGround>() == null)
        {
            DestroyImmediate(rb);
        }
        if (rb == null)
        {
            rb = obj.AddComponent<Rigidbody>() as Rigidbody;
            rb.useGravity = true;
            rb.isKinematic = true;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.mass = 1;
            rb.drag = (float)0.1;
            rb.angularDrag = (float)0.05;
        }
    }

    public void AddNetObject(GameObject obj) {
        NetObject no = obj.GetComponent(typeof(NetObject)) as NetObject;
        if (no)
        {
            Object.DestroyImmediate(no);
        }
        if (no == null)
        {
            no = obj.AddComponent(typeof(NetObject)) as NetObject;
            no.AllowSyncIn = true;
            no.AllowSyncOut = true;
        }
        no.enabled = false;
    }
    public InteractableAsset AddInteractableObjectComponent(GameObject obj) {
        VRTK_InteractableObject[] vio = obj.GetComponents<VRTK_InteractableObject>();
        Debug.Log("hey! checking..." + obj.name + " count"+ vio.Length);
        if (vio.Length > 0)
        {
            foreach (VRTK_InteractableObject copy in vio)
            {
                Destroy(copy);
            }
        }
        InteractableAsset interactableAsset = obj.AddComponent<InteractableAsset>();
        interactableAsset.Initialize();

        interactableAsset.enabled = true;
        return interactableAsset;
    }
    public void AddGrabMechanicScript(GameObject obj, InteractableAsset interactableAsset) {
        VRTK_FixedJointGrabAttach vfjga = obj.GetComponent(typeof(VRTK_FixedJointGrabAttach)) as VRTK_FixedJointGrabAttach;
        if (vfjga)
        {
            Object.DestroyImmediate(vfjga);
        }
        if (vfjga == null)
        {
            vfjga = obj.AddComponent(typeof(VRTK_FixedJointGrabAttach)) as VRTK_FixedJointGrabAttach;
            vfjga.precisionGrab = true;
            vfjga.destroyImmediatelyOnThrow = true;
            interactableAsset.grabAttachMechanicScript = vfjga;
        }

    }
    public void AddSecondaryGrabActionScript(GameObject obj, InteractableAsset interactableAsset) {
        VRTK_SwapControllerGrabAction vscga = obj.GetComponent(typeof(VRTK_SwapControllerGrabAction)) as VRTK_SwapControllerGrabAction;
        if (vscga)
        {
            Object.DestroyImmediate(vscga);
        }
        if (vscga == null)
        {
            vscga = obj.AddComponent(typeof(VRTK_SwapControllerGrabAction)) as VRTK_SwapControllerGrabAction;
            interactableAsset.secondaryGrabActionScript = vscga;
        }
    }
    public void AddOutlineHighlighter(GameObject obj) {
        VRTK_OutlineObjectCopyHighlighter vooch = obj.GetComponent(typeof(VRTK_OutlineObjectCopyHighlighter)) as VRTK_OutlineObjectCopyHighlighter;
        if (vooch)
        {
            Object.DestroyImmediate(vooch);
        }
        if (vooch == null)
        {
            vooch = obj.AddComponent(typeof(VRTK_OutlineObjectCopyHighlighter)) as VRTK_OutlineObjectCopyHighlighter;
            vooch.active = true;
            vooch.unhighlightOnDisable = true;
            vooch.enableSubmeshHighlight = true;
            vooch.customOutlineModels = new GameObject[] { gameObject };
            vooch.thickness = OutlineWidth;
        }
    }
}
