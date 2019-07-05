using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

namespace Auroraland {
    [RequireComponent(typeof(VRTK_InteractGrab))]
    [RequireComponent(typeof(VRTK_InteractTouch))]
    public class BeizerPointerGrabController : MonoBehaviour
    {
        public GameObject Cursor;
        public bool isDebugMode = false; 
        VRTK_InteractGrab grab;
        VRTK_InteractTouch touch;
        // Use this for initialization
        void Start()
        {
            grab = GetComponent<VRTK_InteractGrab>();
            touch = GetComponent<VRTK_InteractTouch>();
            grab.ControllerStartGrabInteractableObject += DoGrab;
            //grab.ControllerGrabInteractableObject += DoGrabbing;
            grab.ControllerUngrabInteractableObject += DoRelease;

            touch.ControllerStartTouchInteractableObject += DoTouch;
            touch.ControllerStartUntouchInteractableObject += DoUntouch;
        }

        private void OnDestroy()
        {
                grab.ControllerStartGrabInteractableObject -= new ObjectInteractEventHandler(DoGrab);
                grab.ControllerUngrabInteractableObject -= new ObjectInteractEventHandler(DoRelease);
                touch.ControllerStartTouchInteractableObject -= new ObjectInteractEventHandler(DoTouch);
                touch.ControllerStartUntouchInteractableObject -= new ObjectInteractEventHandler(DoUntouch);
        }

        void DoGrab(object sender, ObjectInteractEventArgs e)
        {
            Debug.Log("Grabbed " + e.target.name);
            GameObject target = e.target;
            target.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;

        }
        void DoGrabbing(object sender, ObjectInteractEventArgs e)
        {
            Debug.Log("Grabing " + e.target.name);
            GameObject target = e.target;
        }
        void DoRelease(object sender, ObjectInteractEventArgs e)
        {
            Debug.Log("Released " + e.target.name);
            GameObject target = e.target;
            target.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }


        void DoTouch(object sender, ObjectInteractEventArgs e)
        {
            if (isDebugMode) Debug.Log("Touched " + e.target.name);
        }

        void DoUntouch(object sender, ObjectInteractEventArgs e)
        {
            if (isDebugMode) Debug.Log("Untouched " + e.target.name);
        }
    }
}

