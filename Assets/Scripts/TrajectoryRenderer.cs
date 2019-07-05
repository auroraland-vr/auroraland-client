using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryRenderer : MonoBehaviour {

	public GameObject Projectile;
	public GameObject Target;
	public LineRenderer lineRenderer;
	public MeshFilter meshFilter;
	public float MeshWidth;
	public int Resolution;
	public bool UseForceCalculate;
	public float Height;
	public float Velocity;
	public float Angle;
	public Vector3 Force;
	public ForceMode ForceMode;

	Mesh mesh;
	float gravity;
	float radianAngle;
	float maxDistance;
	// Use this for initialization
	void Awake () {
		mesh = meshFilter.mesh;
		Debug.Log (Physics.gravity.y);
		gravity = Mathf.Abs(Physics.gravity.y);	
	}
	void Start(){
		
		//RenderArcMesh (CalculateArcArray());

	}
	void OnValidate(){
		if (Application.isPlaying) {
			//if(lineRenderer != null)RenderArc ();
			//if(mesh != null)RenderArcMesh (CalculateArcArray());
		}
	}
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space)) {
			Projectile.GetComponent<Rigidbody> ().isKinematic = false;
			Projectile.GetComponent<Rigidbody> ().AddForce (Force, ForceMode);
		}
		if (Input.GetKeyDown (KeyCode.KeypadEnter)) {
			if (UseForceCalculate) {
				Velocity = Mathf.Sqrt (Force.y * Force.y + Force.z * Force.z);
				Angle = Mathf.Atan2 (Force.y, Force.z)* Mathf.Rad2Deg;
			} 
			else {
				float radianAngle = Mathf.Deg2Rad * Angle;
				Force = new Vector3 (0, Velocity*Mathf.Sin(radianAngle), Velocity*Mathf.Cos(radianAngle));
			}
			Projectile.GetComponent<Rigidbody> ().isKinematic = true;
			Projectile.transform.position = new Vector3 (0, Height, 0);
			RenderArc ();
			Target.transform.position = GetLandingPosition ();
		}

	}
    Vector3 GetLandingPosition(){
		return new Vector3 (0, 0, maxDistance);		
	}
	void RenderArc(){
		lineRenderer.positionCount = Resolution + 1;
		Vector3[] array = CalculateArcArray ();
		for (int i = 0; i <= Resolution; i++) {
			array [i] = new Vector3 (array[i].z, array[i].y, array[i].x);
		}
		lineRenderer.SetPositions(array);
	}
	void RenderArcMesh(Vector3[] arcVerts){

		mesh.Clear ();
		Vector3[] vertices = new Vector3[(Resolution + 1)*2]; //left and right side of arc
		int[] triangles = new int[Resolution * 6 * 2]; // one quad is 2 triangles = 6 vertices * 2 sides (top and underneath)

		for (int i = 0; i <= Resolution; i++) {
			//set vertices
			vertices[i*2] = new Vector3(MeshWidth * 0.5f, arcVerts[i].y ,arcVerts[i].x); // width, height, forward
			vertices[i*2 + 1] = new Vector3(MeshWidth * -0.5f, arcVerts[i].y ,arcVerts[i].x); 

			//set triangles
			if (i != Resolution) {
				triangles [i * 12] = i * 2;
				triangles [i * 12 + 1] = triangles [i * 12 + 4] = (i + 1) * 2; 
				triangles [i * 12 + 2] = triangles [i * 12 + 3] = i * 2 + 1;
				triangles [i * 12 + 5] = (i + 1) * 2 + 1; 

				triangles [i * 12 + 6] = i * 2;
				triangles [i * 12 + 7] = triangles [i * 12 + 10] = (i + 1) * 2; 
				triangles [i * 12 + 8] = triangles [i * 12 + 9] = i * 2 + 1;
				triangles [i * 12 + 11] = (i + 1) * 2 + 1; 
			}

			mesh.vertices = vertices;
			mesh.triangles = triangles;
		}

		
	}

	Vector3[] CalculateArcArray(){
		Vector3[] arcArray = new Vector3[Resolution + 1];
		radianAngle = Mathf.Deg2Rad * Angle;
		maxDistance = Velocity * Mathf.Cos (radianAngle) / gravity * (Velocity * Mathf.Sin (radianAngle) + Mathf.Sqrt (Velocity * Velocity * Mathf.Sin (radianAngle) * Mathf.Sin (radianAngle) + 2 * gravity * Height));
			//(Velocity * Velocity * Mathf.Sin (2 * radianAngle)) / gravity;
		for (int i = 0; i <= Resolution; i++) {
			float t = (float)i / (float)Resolution;
			arcArray [i] = CalculateArcPoint (t, maxDistance);
		}
		return arcArray;
	}

	Vector3 CalculateArcPoint(float t , float maxDistance){
		float x = t * maxDistance;
		float y =  Height + x * Mathf.Tan(radianAngle) - ((gravity* x * x) / (2 * Velocity* Mathf.Cos(radianAngle)*Velocity* Mathf.Cos(radianAngle)));
		return new Vector3 (x, y);
	}
}
