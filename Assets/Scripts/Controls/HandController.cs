using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Assertions;
using VRTK;
using VRTK.GrabAttachMechanics;

namespace Auroraland
{
    /// <summary>
    /// Is responsible for all Controller Event responses and inputting those responses into the IK and Avatar systems
    /// </summary>
    [RequireComponent(typeof(AutoRaycaster))]
    [RequireComponent(typeof(VRTK_InteractGrab))]
    [RequireComponent(typeof(VRTK_ControllerEvents))]
    public class HandController : MonoBehaviour
    {
        public bool isLeftHand;
        public bool isDebugMode;
        
        public AvatarController _avatar;
        [SerializeField]
        Transform _headsetTransform;
       
        /// <summary>
        /// Stores player input -- for Vive, pad press/shift. For Oculus, stick shift. 
        /// </summary>
        Vector2 _inputAxis;

        [SerializeField] float turnSpeed = 45;
        [SerializeField]
        /// <summary>
        /// Game object that serves as an attachment point for grabbed objects.
        /// </summary>
        GameObject _attachPoint;
        /// <summary>
        /// Component found on attachPoint.
        /// </summary>
        public ObjectManipulator ObjectManipulator;

        GameObject _mainMenu;
        ParentConstraint _menuConstraint;
        public IKController _ikController;
        AutoRaycaster _autoRaycaster;
        VRTK_InteractGrab _vrtkGrab;
        public VRTK_ControllerEvents _vrtkControllerEvents { get; private set; }

        bool _isAutoraycasting;
        bool _isTouchpadPressed;
        bool _isThumbstickTouched;
        bool _isGrabbing;
        
        void Start()
        {
            _autoRaycaster = GetComponent<AutoRaycaster>();

            Assert.IsNotNull(_attachPoint);
            Assert.IsNotNull(_avatar);
            Assert.IsNotNull(_headsetTransform);
            Assert.IsNotNull(_autoRaycaster);

            if (_attachPoint != null && ObjectManipulator == null)
            {
                ObjectManipulator = _attachPoint.GetComponent<ObjectManipulator>();

                if (ObjectManipulator == null)
                    ObjectManipulator = _attachPoint.AddComponent<ObjectManipulator>();
            }

            _autoRaycaster.Toggled += Autoraycaster_Toggled;
            _vrtkGrab = GetComponent<VRTK_InteractGrab>();
            _vrtkControllerEvents = GetComponent<VRTK_ControllerEvents>();

            // TODO Should HandController know about NKGameManager, or should it be the other way around?
            if (NKGameManager.Instance != null)
            {
                if (isLeftHand)
                    NKGameManager.Instance.LeftController.transform.SetParent(transform);
                else
                    NKGameManager.Instance.RightController.transform.SetParent(transform);
            }

            _vrtkGrab.ControllerStartGrabInteractableObject += DoGrab;
            _vrtkGrab.ControllerUngrabInteractableObject += DoRelease;

            _vrtkControllerEvents.TouchpadPressed += DoTouchpadPressed;
            _vrtkControllerEvents.TouchpadReleased += DoTouchpadReleased;
            _vrtkControllerEvents.TouchpadAxisChanged += DoTouchpadAxisChanged;
            _vrtkControllerEvents.TouchpadTouchStart += DoTouchpadTouchStart;
            _vrtkControllerEvents.TouchpadTouchEnd += DoTouchpadTouchEnd;

            _vrtkControllerEvents.TriggerPressed += DoTriggerPressed;
            _vrtkControllerEvents.TriggerReleased += DoTriggerReleased;

            _vrtkControllerEvents.ButtonTwoPressed += DoButtonTwoPressed;
        }

        private void Autoraycaster_Toggled(object sender, AutoRaycaster.AutoRaycasterEventArgs e)
        {
            _isAutoraycasting = e.IsOn;

            if (_ikController != null)
                _ikController.isRightHandIKEnabled = e.IsOn;
        }

        void OnDestroy()
        {
            if (_vrtkControllerEvents != null)
            {
                _vrtkControllerEvents.TriggerPressed -= DoTriggerPressed;
                _vrtkControllerEvents.TriggerReleased -= DoTriggerReleased;

                _vrtkControllerEvents.ButtonTwoPressed -= DoButtonTwoPressed;

                _vrtkControllerEvents.TouchpadTouchStart -= DoTouchpadTouchStart;
                _vrtkControllerEvents.TouchpadTouchEnd -= DoTouchpadTouchEnd;
                _vrtkControllerEvents.TouchpadPressed -= DoTouchpadPressed;
                _vrtkControllerEvents.TouchpadReleased -= DoTouchpadReleased;
                _vrtkControllerEvents.TouchpadAxisChanged -= DoTouchpadAxisChanged;

                _vrtkGrab.ControllerStartGrabInteractableObject -= DoGrab;
                _vrtkGrab.ControllerUngrabInteractableObject -= DoRelease;
            }
        }

        void Update()
        {
            /* TODO Removed, non-VR build is still unstable
            if (string.Equals(SDK_SetupMode.Instance.CurrentSDKName, "NonVR"))
            {
                ProcessNonVRInput();
            }
            */

            if (_isTouchpadPressed || _isThumbstickTouched)
            {
                ProcessVRInput();
            }
        }

        void ProcessNonVRInput()
        {
            if (isLeftHand)
            {
                return;
            }

            if (!_isGrabbing) //walking, rotating...
            {
                _avatar.Direction = Input.GetAxis("Vertical");

                float lazyTurn = Input.GetAxis("Horizontal");

                if (lazyTurn != 0)
                    _avatar.LazyTurn = 45 * lazyTurn;
                else
                    _avatar.LazyTurn = 0;
            }
            else
            {
                float yAxis = Input.GetAxis("Vertical");

                if (yAxis != 0)
                    ObjectManipulator.DoMove(yAxis > 0 ? ObjectManipulator.Direction.Backward : ObjectManipulator.Direction.Forward);
            }
        }

        void ProcessVRInput()
        {
            float yAxisThreshold = 0.5f;

            if (!_isGrabbing)
            {
                if (_inputAxis.x != 0)
                    _avatar.LazyTurn = turnSpeed * _inputAxis.x;
                else
                    _avatar.LazyTurn = 0;

                if (Mathf.Abs(_inputAxis.y) > Mathf.Abs(_inputAxis.x))
                    _avatar.Direction = _inputAxis.y;
            }
            else
            {
                
                if (_inputAxis.y > yAxisThreshold)
                    ObjectManipulator.DoMove(ObjectManipulator.Direction.Backward);
                else if (_inputAxis.y < -yAxisThreshold)
                    ObjectManipulator.DoMove(ObjectManipulator.Direction.Forward);
                    
            }
        }

        #region Controller events
        void DoTriggerPressed(object sender, ControllerInteractionEventArgs e)
        {
            if (_ikController != null)
            {
                if (isLeftHand)
                    _ikController.intensityOfLeftHand = 1;
                else
                    _ikController.intensityOfRightHand = 1;
            }

            DebugLogger(VRTK_ControllerReference.GetRealIndex(e.controllerReference), "TRIGGER", "pressed", e);
        }
        void DoTriggerReleased(object sender, ControllerInteractionEventArgs e)
        {
            if (_ikController != null)
            {
                if (isLeftHand)
                    _ikController.intensityOfLeftHand = 0;
                else
                    _ikController.intensityOfRightHand = 0;
            }

            DebugLogger(VRTK_ControllerReference.GetRealIndex(e.controllerReference), "TRIGGER", "unpressed", e);
        }

        void DoTouchpadTouchStart(object sender, ControllerInteractionEventArgs e)
        {
            if (VRTK_DeviceFinder.GetCurrentControllerType() == SDK_BaseController.ControllerType.Oculus_OculusTouch || VRTK_DeviceFinder.GetCurrentControllerType() == SDK_BaseController.ControllerType.SteamVR_OculusTouch)
                _isThumbstickTouched = true;

            DebugLogger(VRTK_ControllerReference.GetRealIndex(e.controllerReference), "TOUCHPAD", "touch start", e);
        }
        void DoTouchpadTouchEnd(object sender, ControllerInteractionEventArgs e)
        {
            _isThumbstickTouched = false;
            _inputAxis = Vector2.zero;
            _avatar.LazyTurn = 0;
            _avatar.Direction = 0;
            _avatar.AngularDirection = 0;

            if (_ikController != null && (_isAutoraycasting || !_autoRaycaster.enabled))
                _ikController.isRightHandIKEnabled = true;

            DebugLogger(VRTK_ControllerReference.GetRealIndex(e.controllerReference), "TOUCHPAD", "touch end", e);
        }
        void DoTouchpadAxisChanged(object sender, ControllerInteractionEventArgs e)
        {
            if (_ikController != null && !_isAutoraycasting && _autoRaycaster.enabled && VRTK_DeviceFinder.GetCurrentControllerType() == SDK_BaseController.ControllerType.Oculus_OculusTouch)
                _ikController.isRightHandIKEnabled = false;

            _inputAxis = e.touchpadAxis;
            DebugLogger(VRTK_ControllerReference.GetRealIndex(e.controllerReference), "TOUCHPAD", "axis changed", e);
        }

        void DoTouchpadPressed(object sender, ControllerInteractionEventArgs e)
        {
            _isTouchpadPressed = true;

            if (VRTK_DeviceFinder.GetCurrentControllerType() == SDK_BaseController.ControllerType.SteamVR_ViveWand)
            {
                _inputAxis = e.touchpadAxis;

                if (_ikController != null && !_isAutoraycasting && _autoRaycaster.enabled)
                    _ikController.isRightHandIKEnabled = false;
            }

            DebugLogger(VRTK_ControllerReference.GetRealIndex(e.controllerReference), "TOUCHPAD", "pressed down", e);
        }
        void DoTouchpadReleased(object sender, ControllerInteractionEventArgs e)
        {
            _isTouchpadPressed = false;

            if (VRTK_DeviceFinder.GetCurrentControllerType() == SDK_BaseController.ControllerType.SteamVR_ViveWand)
            {
                _inputAxis = Vector2.zero;
                _avatar.LazyTurn = 0;
                _avatar.Direction = 0;
                _avatar.AngularDirection = 0;
            }

            DebugLogger(VRTK_ControllerReference.GetRealIndex(e.controllerReference), "TOUCHPAD", "released", e);
        }

        void DoButtonTwoPressed(object sender, ControllerInteractionEventArgs e)
        {
            _mainMenu.SetActive(!_mainMenu.activeSelf);
            _avatar.AllowMovement = !_mainMenu.activeSelf;
            DebugLogger(VRTK_ControllerReference.GetRealIndex(e.controllerReference), "BUTTON TWO", "pressed", e);
        }

        void DoGrab(object sender, ObjectInteractEventArgs e)
        {
            GameObject target = e.target;

            GameObject grabHand = (sender as VRTK_InteractGrab).gameObject;
            SyncAttachPoint(grabHand, target);

            _isGrabbing = true;

            if (isDebugMode)
            {
                return;
            }

            var netObj = e.target.GetComponent<NetObject>();
            if (netObj == null || !netObj.isActiveAndEnabled || netObj.IsOwnedByOthers())
            {
                _vrtkGrab.ForceRelease();
                return;
            }

            netObj.TakeOwnership();
        }
        void DoRelease(object sender, ObjectInteractEventArgs e)
        {
            GameObject target = e.target;

            _isGrabbing = false;
            if (isDebugMode)
            {
                return;
            }

            NetObject netObj = target.GetComponent<NetObject>();

            if (netObj == null || !netObj.isActiveAndEnabled)
            {
                // if NetObject component is not found,
                // or it's not enabled
                // ignore everything
                return;
            }

            netObj.ReleaseOwnership();

            // ThrowableObject check = target.GetComponent<ThrowableObject>();

            // if (check != null)
            //    check.Throw(new Vector3(0, 0.53f, 0.3f), ForceMode.VelocityChange);

            //target.GetComponent<Rigidbody>().isKinematic = false;
        }
        #endregion

        /// <summary>
        /// Sets the target menu to open when the user presses a button, and makes that menu follow the headset.
        /// </summary>
        /// <param name="menu">Menu to use</param>
        public void SetMenu(GameObject menu)
        {
            _mainMenu = menu;
            // HACK These next few lines expect the parent to be ok with a ParentConstraint
            _menuConstraint = menu.transform.parent.GetComponent<ParentConstraint>();

            if (_menuConstraint == null)
            {
                _menuConstraint = _mainMenu.AddComponent<ParentConstraint>();
                _menuConstraint.AddSource(new ConstraintSource { sourceTransform = _headsetTransform, weight = 1 });
            }

            _menuConstraint.rotationAxis = Axis.Y;
        }

        public void SetIKController(IKController controller)
        {
            _ikController = controller;
            _ikController.isLeftHandIKEnabled = false;
            _ikController.isRightHandIKEnabled = true;
            _ikController.BASE_FACTOR = 0.1f;
            _ikController.enabled = true;
        }

        /// <summary>
        /// Updates the transform of the grab hand's attach point to match the transform of the grabbed object
        /// </summary>
        /// <param name="grabHand">GameObject of hand used to grab</param>
        /// <param name="grabbedObject">GameObject of object grabbed</param>
        void SyncAttachPoint(GameObject grabHand, GameObject grabbedObject)
        {
            // Create a snap handle at the center of the grabbed object
            // used to grab and rotate about the center axis
            Transform grabTransform = grabbedObject.transform;
            VRTK_FixedJointGrabAttach grabAttach = grabbedObject.GetComponent<VRTK_FixedJointGrabAttach>();
            Transform snapHandle = (grabAttach.rightSnapHandle) ? grabAttach.rightSnapHandle : (new GameObject("SnapHandle")).transform;
            snapHandle.parent = grabTransform;
            snapHandle.position = grabbedObject.GetComponent<Renderer>().bounds.center;
            snapHandle.rotation = grabTransform.rotation;
            grabAttach.rightSnapHandle = snapHandle;
            grabAttach.leftSnapHandle = snapHandle;
            grabAttach.precisionGrab = false;

            // Update the attach point transform to match the grabbed game 
            // objects snap handle, translated from object's space to attach point's space
            Transform parent = _attachPoint.transform.parent;
            Vector3 translatedPos = parent.InverseTransformPoint(snapHandle.position);
            _attachPoint.transform.localPosition = translatedPos;
            _attachPoint.transform.rotation = snapHandle.rotation;
        }

        /// <summary>
        /// Shared by all controller callbacks to output the event type
        /// </summary>
        /// <param name="index"></param>
        /// <param name="button"></param>
        /// <param name="action"></param>
        /// <param name="e"></param>
        void DebugLogger(uint index, string button, string action, ControllerInteractionEventArgs e)
        {
            if (isDebugMode)
            {
                string debugString = "Controller on index '" + index + "' " + button + " has been " + action
                    + " with a pressure of " + e.buttonPressure + " / Primary Touchpad axis at: " + e.touchpadAxis + " (" + e.touchpadAngle + " degrees)" + " at " + Time.frameCount;
                VRTK_Logger.Info(debugString);
            }
        }
    }
}