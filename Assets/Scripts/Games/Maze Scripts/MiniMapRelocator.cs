using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Find the Avatar in the scene and locate self relative to parent.
public class MiniMapRelocator : MonoBehaviour
{
	RectTransform worldCanvas;
	Transform avatar;
	public Vector3 mapPositionOffset = new Vector3(.1f,1.33f,.62f);
	public Vector3 mapRotationOffset = new Vector3(38, 12, 0);
	public Vector3 mapScaleOffset = new Vector3(1, 1, 1);

	void Start ()
	{
		avatar = GameObject.FindGameObjectWithTag("Avatar").transform;
		worldCanvas = GetComponent<RectTransform>();
		PlaceMiniMapToAvatar();
		
	}


	void PlaceMiniMapToAvatar()
	{
		transform.SetParent(avatar);
		transform.localPosition = mapPositionOffset;
		transform.localEulerAngles = mapRotationOffset;
		transform.localScale = mapScaleOffset;
	}

    void OnEnable()
    {
        SceneManager.sceneUnloaded += DestroySelf;
    }

    void DestroySelf(Scene scene)
    {
        if (scene.name == "scene_maze") Destroy(gameObject);
    }

    void OnDisable()
    {
        SceneManager.sceneUnloaded -= DestroySelf;
    }
}
