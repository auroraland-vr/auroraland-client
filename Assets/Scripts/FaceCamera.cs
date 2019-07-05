using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour {

	public bool FixedHeight;
	public bool FollowMainCamera;
	private float distance = 2f;
	// Use this for initialization
	void Start () {
		if (Camera.main) {
			Vector3 cameraPos = Camera.main.transform.position;
			distance = Vector3.Distance (new Vector3 (cameraPos.x, 0, cameraPos.z), new Vector3 (transform.position.x, 0, transform.position.z));
		}
	}
	
	// Update is called once per frame
	void Update () {


		//facing direction
		if (Camera.main != null) {
			Vector3 facingDirection = Camera.main.transform.forward;
			facingDirection = FixedHeight ? new Vector3 (-facingDirection.x, 0, -facingDirection.z) : -facingDirection;
			Quaternion rotation = Quaternion.LookRotation (facingDirection);
			transform.rotation = rotation;
			transform.Rotate (new Vector3 (0, 180f, 0));
		}

		//tracking position
		if (FollowMainCamera) {
			Vector3 direction = Camera.main.transform.forward;
			Vector3 unit = Vector3.Normalize (new Vector3 (direction.x, 0, direction.z));
			Vector3 track = Camera.main.transform.position + unit * distance;
			transform.position = new Vector3 (track.x, transform.position.y, track.z);
		}

	}
}
