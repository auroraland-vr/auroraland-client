using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRTK;

namespace Auroraland
{
    public class NonVRInputSimulator : MonoBehaviour
    {

        [Tooltip("Key used to switch between left and righ hand.")]
        public KeyCode changeHands = KeyCode.Tab;
        [Tooltip("Mouse cursor on screen")]
        public GameObject Cursor;

        [Header("Controller Key Bindings")]
        [Tooltip("Key used to simulate trigger button.")]
        public KeyCode triggerAlias = KeyCode.Mouse1;
        [Tooltip("Key used to simulate grip button.")]
        public KeyCode gripAlias = KeyCode.Mouse0;
        [Tooltip("Key used to simulate touchpad button.")]
        public KeyCode touchpadAlias = KeyCode.Q;
        [Tooltip("Key used to simulate button one.")]
        public KeyCode buttonOneAlias = KeyCode.E;
        [Tooltip("Key used to simulate button two.")]
        public KeyCode buttonTwoAlias = KeyCode.R;
        [Tooltip("Key used to simulate start menu button.")]
        public KeyCode startMenuAlias = KeyCode.F;
        [Tooltip("Key used to switch between button touch and button press mode.")]
        public KeyCode touchModifier = KeyCode.T;
        [Tooltip("Key used to switch between hair touch mode.")]
        public KeyCode hairTouchModifier = KeyCode.H;

        private Transform rightHand;
        private Transform leftHand;
        private Transform currentHand;
        private Vector3 mousePosition;
        private float raycastMaxDistance = 10f;
        private float distance = 0; //distance between touched interactable objects and ray origin
        private float previousDistance = 0; // previous distance between touched interactable objects and ray origin

        //private Transform neck;
        private NonVRControllerSimulator rightController;
        private NonVRControllerSimulator leftController;
        private static GameObject cachedCameraRig;
        private static bool destroyed = false;

        /// <summary>
        /// The FindInScene method is used to find the `NonVRSimulatorCameraRig` GameObject within the current scene.
        /// </summary>
        /// <returns>Returns the found `NonVRSimulatorCameraRig` GameObject if it is found. If it is not found then it prints a debug log error.</returns>
        public static GameObject FindInScene()
        {
            if (cachedCameraRig == null && !destroyed)
            {
                cachedCameraRig = VRTK_SharedMethods.FindEvenInactiveGameObject<NonVRInputSimulator>();
                if (!cachedCameraRig)
                {
                    VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_SCENE, "NonVRSimulatorCameraRig", "NonVRSimulator", ". check that the `NonVRSimulator` prefab been added to the scene."));
                }
            }
            return cachedCameraRig;
        }

        private void Awake()
        {
            VRTK_SDKManager.instance.AddBehaviourToToggleOnLoadedSetupChange(this);
        }

        private void OnEnable()
        {
            rightHand = transform.Find("RightHand");
            rightHand.gameObject.SetActive(false);
            leftHand = transform.Find("LeftHand");
            leftHand.gameObject.SetActive(false);
            currentHand = rightHand;
            //leftHand.Find("Hand").GetComponent<Renderer>().material.color = Color.red;
            //rightHand.Find("Hand").GetComponent<Renderer>().material.color = Color.green;
            rightController = rightHand.GetComponent<NonVRControllerSimulator>();
            leftController = leftHand.GetComponent<NonVRControllerSimulator>();
            rightController.Selected = true;
            leftController.Selected = false;
            destroyed = false;

            var controllerSDK = VRTK_SDK_Bridge.GetControllerSDK() as NonVRSimController;
            if (controllerSDK != null)
            {
                Dictionary<string, KeyCode> keyMappings = new Dictionary<string, KeyCode>() {
                    { "Trigger", triggerAlias },
                    { "Grip", gripAlias },
                    { "TouchpadPress", touchpadAlias },
                    { "ButtonOne", buttonOneAlias },
                    { "ButtonTwo", buttonTwoAlias },
                    { "StartMenu", startMenuAlias },
                    { "TouchModifier", touchModifier },
                    { "HairTouchModifier", hairTouchModifier }
                };
                controllerSDK.SetKeyMappings(keyMappings);
            }
            rightHand.gameObject.SetActive(true);
            leftHand.gameObject.SetActive(true);
        }

        private void OnDestroy()
        {
            VRTK_SDKManager.instance.RemoveBehaviourToToggleOnLoadedSetupChange(this);
            destroyed = true;
        }

        private void Update()
        {
            mousePosition = Input.mousePosition;
            // distance from camera (this value must set as none zero, otherwise screen to world position will be fixed value)
            mousePosition.z = Camera.main.nearClipPlane; 

            if (Input.GetKeyDown(changeHands))
            {
                if (currentHand.name == "LeftHand")
                {
                    currentHand = rightHand;
                    rightController.Selected = true;
                    leftController.Selected = false;
                }
                else
                {
                    currentHand = leftHand;
                    rightController.Selected = false;
                    leftController.Selected = true;
                }
            }

            UpdateHands();

            if (Input.GetKeyDown(gripAlias))
            {
                if (currentHand == leftHand)
                {
                    TryPickup(false);
                }
                else
                {
                    TryPickup(true);
                }
            }

        }

        /// <summary>
        /// Get the game object for the current hand
        /// </summary>
        /// <returns>GameObject of current hand</returns>
        private GameObject GetHand()
        {
            return (rightHand) ?
                VRTK_DeviceFinder.GetControllerRightHand() :
                VRTK_DeviceFinder.GetControllerLeftHand();
        }

        /// <summary>
        /// Gets the VRTK Interact Grab script for the given hand
        /// </summary>
        /// <param name="hand">GameObject for hand</param>
        /// <returns>VRTK Interact Grab script</returns>
        private VRTK_InteractGrab GetGrab(GameObject hand)
        {
            return hand.GetComponent<VRTK_InteractGrab>();
        }

        private void TryPickup(bool rightHand)
        {
            //update the hand WORLD position 
            Ray screenRay = Camera.main.ScreenPointToRay(mousePosition);
            // Debug.DrawRay(screenRay.origin, screenRay.direction * 10, Color.yellow);
            RaycastHit hit;
            if (Physics.Raycast(screenRay, out hit))
            {
                //Debug.Log("grab" + hit.collider.name);
                VRTK_InteractableObject io = hit.collider.gameObject.GetComponent<VRTK_InteractableObject>();
                if (io)
                {
                    GameObject hand = GetHand();
                    VRTK_InteractGrab grab = GetGrab(hand);
                    if (grab.GetGrabbedObject() == null)
                    {
                        hand.GetComponent<VRTK_InteractTouch>().ForceTouch(hit.collider.gameObject);
                        grab.AttemptGrab();
                    }
                }
            }
        }

        /// <summary>
        /// Updates the hand position and cursor to follow mouse pointer
        /// </summary>
        private void UpdateHands()
        {
            // Cast a ray through the screen at the current mouse position
            Ray screenRay = Camera.main.ScreenPointToRay(mousePosition);

            float offset = 0.3f;    // amount of space between cursor and hand
            float dist = offset;    // used to find point along screen ray
            Vector3 cursorPosition = screenRay.GetPoint(dist); // draw cursor on screen
            dist += offset;
            Vector3 handPos = screenRay.GetPoint(dist); // draw hand behind cursor
            dist += offset;
            Vector3 handLookPos = screenRay.GetPoint(dist); // hand will look in direction of ray

            // Set positions and orient hand
            Cursor.transform.position = cursorPosition;
            currentHand.transform.position = handPos;
            currentHand.transform.LookAt(handLookPos);
        }
    }
}
