using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRTK;
//
// DEPRECATED!
// This class is not used any more!
namespace Auroraland
{
    public class GrabSimulator : MonoBehaviour
    {
        public Text Log;
        public Transform IntereactRoot;
        public GameObject player;
        private GameObject grabbedObject;
        private Color originalColor;
        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButton(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 100f))
                {
                    GameObject target = hit.collider.gameObject;
                    if (target.tag == "Grabbable")
                    {
                        if (grabbedObject == null)
                        {
                            Grab(target);
                        }
                        else if (grabbedObject != target)
                        {
                            Release();
                            Grab(target);
                        }
                    }
                }
            }

            if (Input.GetMouseButton(1))
            {
                if (grabbedObject != null) Release();
            }
            if (grabbedObject != null)
            {
                grabbedObject.transform.position = new Vector3(player.transform.position.x, 1f, player.transform.position.z + 0.5f);
            }
            if (IntereactRoot != null)
            {
                foreach (Transform child in IntereactRoot)
                {
                    Log.text = child.name + " pos:" + child.position + "\n ownerId=" + child.GetComponent<NetObject>().GetUserId();
                }
            }
        }

        public void Grab(GameObject target)
        {

            if (!string.IsNullOrEmpty(target.GetComponent<NetObject>().GetUserId()))
                return;
            target.GetComponent<NetObject>().TakeOwnership();
            originalColor = target.GetComponent<MeshRenderer>().material.color;
            target.GetComponent<MeshRenderer>().material.color = new Color(1, 0, 0);
            // target.GetComponent<Rigidbody>().isKinematic = true;
            grabbedObject = target;
        }

        public void Release()
        {
            grabbedObject.GetComponent<NetObject>().ReleaseOwnership();
            grabbedObject.GetComponent<MeshRenderer>().material.color = originalColor;
            // grabbedObject.GetComponent<Rigidbody>().isKinematic = false;
            grabbedObject = null;
        }
    }
}
