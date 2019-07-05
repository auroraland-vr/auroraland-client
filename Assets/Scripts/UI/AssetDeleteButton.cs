using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Nakama;

namespace Auroraland{
	public class AssetDeleteButton : MonoBehaviour {

		public GameObject Target;
		public Button DeleteButton;
		public float HeightOffset;

		private Bounds bounds;
		private float height;
		 
		// Use this for initialization
		void OnEnable(){
			DeleteButton.onClick.AddListener (()=> {DeleteAsset();});
		}

		void OnDisable(){
			DeleteButton.onClick.RemoveListener (()=> {DeleteAsset();});
		}

		void Start(){
			SetPosition ();
		}

        Bounds CalculateBounds() {

            Vector2 x = new Vector2(int.MaxValue, int.MinValue); //x.x: min of x, x.y: min of x
            Vector2 y = new Vector2(int.MaxValue, int.MinValue); //y.x: min of y, y.y: min of y
            Vector2 z = new Vector2(int.MaxValue, int.MinValue); //z.x: min of z, z.y: min of z
         
            foreach (var collider in Target.GetComponentsInChildren<Collider>()) {
                if (collider.gameObject.layer == LayerMask.NameToLayer("UI")) continue; // skip UI layer components
                if (collider.GetComponent<MeshFilter>() == null) continue; //skip no mesh
                if (collider.GetComponent<MeshRenderer>() == null) continue; //skip no mesh
                if (!collider.GetComponent<MeshRenderer>().enabled) continue; // skip object that doesn't show
                if (collider.transform.position.x + collider.bounds.max.x > x.y) { // max value of x
                    x.y = collider.transform.position.x + collider.bounds.max.x;
                }
                if (collider.transform.position.x + collider.bounds.min.x < x.x) // min value of x
                {
                    x.x = collider.transform.position.x + collider.bounds.min.x;
                }
                if (collider.transform.position.y + collider.bounds.max.y > y.y) // min value of y
                {
                    y.y = collider.transform.position.y + collider.bounds.max.y;
                }
                if (collider.transform.position.y + collider.bounds.min.y < y.x) // min value of y
                {
                    y.x = collider.transform.position.y + collider.bounds.min.y;
                }
                if (collider.transform.position.z + collider.bounds.max.z > z.y) // max value of z
                {
                    z.y = collider.transform.position.z + collider.bounds.max.z;
                }
                if (collider.transform.position.z + collider.bounds.min.z < z.x) // min value of z
                {
                    z.x = collider.transform.position.z + collider.bounds.min.z;
                }
            }
            Vector3 center = Vector3.zero;
            Vector3 size = Vector3.zero;
            size.x = Mathf.Abs(x.y - x.x);
            size.y = Mathf.Abs(y.y - y.x);
            size.z = Mathf.Abs(z.y - z.x);
            center.x = (x.y + x.x) / 2 - Target.transform.position.x;
            center.y = (y.y + y.x) / 2 - Target.transform.position.y;
            center.z = (z.y + z.x) / 2 - Target.transform.position.z;
            Bounds bounds = new Bounds(center, size);
            //Debug.Log(Target.name + " bounds:"+ bounds);
            return bounds;

        }
		void FixedUpdate(){
			if (Target.GetComponent<VRTK.VRTK_InteractableObject> () && Target.GetComponent<VRTK.VRTK_InteractableObject> ().IsGrabbed()) {
				SetPosition ();
			}
		}

		public void SetPosition(){
            bounds = CalculateBounds();
            height = bounds.max.y + HeightOffset;
            transform.position = new Vector3 ( Target.transform.position.x, height , Target.transform.position.z);
		}

		public void DeleteAsset(){
            INEntity entity = Target.GetComponent<NetObject>().GetNEntity();
			NKController.Instance.DeleteEntity (entity);
		}
	}

}