using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Auroraland{
	
	[RequireComponent(typeof(Rigidbody))]
	public class SnapToGround : MonoBehaviour {
		public float threshold;
		private float timer;
		private Rigidbody rigidbody;
		private bool prevIsKinematic;
		void Awake(){
			
			rigidbody = GetComponent<Rigidbody> ();
			prevIsKinematic = rigidbody.isKinematic;
		}

		void OnCollisionEnter(Collision collision){
			if (collision.gameObject.tag == "Floor") {
				timer = 0;	
			}
		}

		void OnCollisionStay(Collision collision){
			if (collision.gameObject.tag == "Floor") {
				timer += Time.deltaTime;
				if (timer > threshold) {
					rigidbody.isKinematic = true;	
				}
			}
		}
	}
}
