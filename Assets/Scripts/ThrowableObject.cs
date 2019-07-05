using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Auroraland{
	public class ThrowableObject : MonoBehaviour {
		private Rigidbody rigidbody;
		public Trajectory trajectory;

		void Awake(){
			rigidbody = GetComponent<Rigidbody> ();
		}
	   
		void OnCollisionEnter(Collision collision){
			if (collision.collider.tag == "Floor") {
				trajectory.Line.enabled = false;
			}
		}

		public void Aiming(Vector3 force, float height){
			trajectory.Line.enabled = true;
			trajectory.RenderTrajectory (force, height);
		}

		public void Throw(Vector3 force, ForceMode forceMode){
			rigidbody.AddForce (force, forceMode);
		}


	}
}