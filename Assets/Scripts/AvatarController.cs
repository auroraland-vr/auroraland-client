using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Assertions;
using VRTK;

namespace Auroraland
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class AvatarController : MonoBehaviour
    {
        const float LERP_RATE = 0.175f;

        /// <summary>
        /// Inspector reference, as the best way so far to cache the headset game object.
        /// </summary>
        [SerializeField]
        [Tooltip("The headset game object under VRTK_ScriptsAction.")]
        GameObject _headset;

        /// <summary>
        /// Inspector reference, as the best way so far to cache the right controller game object.
        /// </summary>
        [SerializeField]
        [Tooltip("The right controller game object under VRTK_ScriptsAction.")]
        GameObject _rightController;

        /// <summary>
        /// Inspector reference to the camera setup root game object.
        /// </summary>
        [SerializeField]
        [Tooltip("The VRTK camera setup game object.")]
        Transform _vrtkCameraSetup;
        /// <summary>
        /// Inspector field - toggle to disable movement.
        /// </summary>
        [Tooltip("Toggle to allow/prevent movement.")]
        public bool AllowMovement = true;

        /// <summary>
        /// The avatar's movement speed. Default is 1.5.
        /// </summary>
        [Tooltip("The avatar's movement speed. Default is 1.5.")]
        public float Speed = 1.5f;

        Animator _modelAnimator;
        CharacterController _characterController;
        public HandController RightHandController { get; private set; }
        VRTK_Pointer _rightPointer;

        [Header("Test Exposures")]
        [SerializeField]
        GameObject _avatarModel;
        // HIDDEN FROM INSPECTOR
        /// <summary>
        /// A movement control based on input. At 1, moves forward. At -1, moves backward.
        /// </summary>
        //[HideInInspector]
        public float Direction;
        //[HideInInspector]
        /// <summary>
        /// A rotation control based on input, overriden by current animation. At -1, rotates counterclockwise. At 1, rotates clockwise.
        /// </summary>
        public float AngularDirection;
        //[HideInInspector]
        public float LazyTurn;

        GameObject _rightLaserOrigin;

        /// <summary>
        /// A movement factor that stores the avatar's current speed based on the current animation.
        /// </summary>
        float playerSpeed;
        /// <summary>
        /// A movement factor based on current animation. At -1, moves backward. At 1, moves forward.
        /// </summary>
        int playerDirection;
        /// <summary>
        /// A rotation control that tells it when to stop.
        /// </summary>
        float totalRotation;

        /// <summary>
        /// The current "state" of the animator component.
        /// </summary>
        int _currentAnimationState;
        /// <summary>
        /// An animator parameter that determines if it CAN stop rotating. If not passed to the animator, it will not stop turning.
        /// </summary>
        bool _isMinimumTurnReached = false;

        public void InitializeAvatar(GameObject newAvatar)
        {
            _avatarModel = newAvatar;
            _avatarModel.transform.eulerAngles = new Vector3(0, Camera.main.transform.eulerAngles.y, 0);
            _modelAnimator = newAvatar.GetComponent<Animator>();

            RightHandController.SetIKController(newAvatar.GetComponent<IKController>());

            AuroralandAvatarSettings settings = newAvatar.GetComponent<AuroralandAvatarSettings>();
            PositionConstraint cameraEyeConstraint = _vrtkCameraSetup.GetComponent<PositionConstraint>();
            ConstraintSource cs = new ConstraintSource { sourceTransform = settings.EyePosition, weight = 1 };

            if (cameraEyeConstraint.sourceCount > 0)
            {
                cameraEyeConstraint.RemoveSource(0);
            }

            cameraEyeConstraint.AddSource(cs);

            // Setup the laser to always originate from the avatar hand
            AimAssist rightFollow = settings.RightHandLaserOrigin.GetComponent<AimAssist>();

            if (rightFollow == null)
                rightFollow = settings.RightHandLaserOrigin.gameObject.AddComponent<AimAssist>();

            rightFollow.Controller = _rightController.transform;
            _rightPointer.customOrigin = rightFollow.transform;

            // HACK Hardcoded to find this game object name
            if (_rightLaserOrigin == null)
            {
                _rightLaserOrigin = GameObject.Find("[VRTK][AUTOGEN][" + _rightController.name + "][BasePointerRenderer_Origin_Smoothed]");
            }

            // Invalidate the lasers so that the origins update
            if (_rightLaserOrigin != null)
                _rightLaserOrigin.SetActive(false);
        }

        public bool IsAvatarInitialized()
        {
            return _avatarModel != null;
        }

        void Awake()
        {
            RightHandController = _rightController.GetComponent<HandController>();
            _rightPointer = _rightController.GetComponent<VRTK_Pointer>();
            _characterController = GetComponent<CharacterController>();

            Assert.IsNotNull(_characterController);
            Assert.IsNotNull(RightHandController);
            Assert.IsNotNull(_rightPointer);
        }

        void Start()
        {
            // Called here instead of Awake because _avatarModel is lazy-loaded and not pre-referenced
            _modelAnimator = _avatarModel.GetComponent<Animator>();
        }

        void Update()
        {
            if (_modelAnimator != null)
            {
                _currentAnimationState = _modelAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash;
            }

            _avatarModel.transform.eulerAngles = new Vector3(0, Camera.main.transform.eulerAngles.y, 0);

            if (AllowMovement)
            {
                UpdatePlayerMovement();
            }
            else if (_modelAnimator != null)
            {
                _modelAnimator.SetFloat("userInputMove", 0);
                _modelAnimator.SetFloat("userInputTurn", 0);
                _modelAnimator.SetBool("wasSitKeyPressed", false);
                _modelAnimator.SetBool("wasKickKeyPressed", false);
                _modelAnimator.SetBool("isRunKeyPressed", false);
                _modelAnimator.SetBool("isMinimumTurnReached", true);
            }

            if (_modelAnimator != null)
                _modelAnimator.SetBool("isGrounded", _characterController.isGrounded);
        }

        readonly Vector3 gravity = new Vector3(0, Physics.gravity.y, 0);

        public void UpdatePlayerMovement()
        {
            _isMinimumTurnReached = false;

            ProcessAnimation();

            //Vector3 playerVector = (_avatarModel.transform.forward * playerDirection * playerSpeed + new Vector3(0, Physics.gravity.y,0)) * Time.deltaTime;
            Vector3 playerVector = _avatarModel.transform.forward * playerSpeed * Direction;

            playerVector += gravity;

            _characterController.Move(playerVector * Time.deltaTime);

           

            //_characterController.Move(mainCamera.forward + new Vector3(0, Physics.gravity.y, 0) * Input.GetAxis("Vertical"));//WOW
            // pad/stick character rotation
            transform.Rotate(0, LazyTurn * Time.deltaTime, 0, Space.Self);//kind of independent
        }

        void ProcessAnimation()
        {
            /**Killed this animation code to stop animator controlling the avatar movement. -Bryan **/

           // DoTurning();
            //DoWalkCycle();
            //DoJogCycle();

            //if (_currentAnimationState == CharacterState.IDLE)
            //{
            //    playerSpeed = 0;
            //    playerDirection = 0;
            //}

            playerSpeed = Speed ;

            _modelAnimator.SetFloat("userInputMove", Direction);
            _modelAnimator.SetFloat("userInputTurn", AngularDirection);
            _modelAnimator.SetBool("isMinimumTurnReached", _isMinimumTurnReached);
            _modelAnimator.SetBool("wasSitKeyPressed", Input.GetButton("Sit"));
            _modelAnimator.SetBool("wasKickKeyPressed", Input.GetButton("Kick"));
            _modelAnimator.SetBool("isRunKeyPressed", true);
        }

        // ANIMATIONS
        void DoTurning()
        {
            if (_currentAnimationState != CharacterState.TURN_LEFT &&
                _currentAnimationState != CharacterState.TURN_RIGHT)
                return;

            playerDirection = 0;
            playerSpeed = 0;
            totalRotation += 180 * Time.deltaTime;
            Quaternion fromRotation = _avatarModel.transform.rotation;
            Quaternion toRotation = Quaternion.Euler(0,_avatarModel.transform.eulerAngles.y + (_currentAnimationState == CharacterState.TURN_LEFT ? -90 : 90), 0);
            _avatarModel.transform.rotation = Quaternion.RotateTowards(fromRotation,toRotation, 180 * Time.deltaTime);
           
            if (totalRotation < 90)
                return;

            AngularDirection = 0;
            _isMinimumTurnReached = true;
            totalRotation = 0;
        }

        void DoWalkCycle()
        {
            if (_currentAnimationState == CharacterState.WALK_START)
            {
                playerDirection = 1;
                playerSpeed = Mathf.Lerp(playerSpeed, Speed, LERP_RATE );
            }
            else if (_currentAnimationState == CharacterState.WALK_LEFT_START
                  || _currentAnimationState == CharacterState.WALK_LEFT_PASS
                  || _currentAnimationState == CharacterState.WALK_RIGHT_START
                  || _currentAnimationState == CharacterState.WALK_RIGHT_PASS)
            {
                playerDirection = 1;
                playerSpeed = Speed;
            }
            else if (_currentAnimationState == CharacterState.WALKBACK_START)
            {
                playerDirection = -1;
                playerSpeed = Mathf.Lerp(playerSpeed, Speed , LERP_RATE );
            }
            else if (_currentAnimationState == CharacterState.WALKBACK_LEFT_START
                  || _currentAnimationState == CharacterState.WALKBACK_LEFT_PASS
                  || _currentAnimationState == CharacterState.WALKBACK_RIGHT_START
                  || _currentAnimationState == CharacterState.WALKBACK_RIGHT_PASS)
            {
                playerDirection = -1;
                playerSpeed = Speed ;
            }
        }

        void DoJogCycle()
        {
            if (_currentAnimationState == CharacterState.JOG_START)
            {
                playerDirection = 1;
                playerSpeed = Mathf.Lerp(playerSpeed, Speed * 1.5f, LERP_RATE);
            }
            else if (_currentAnimationState == CharacterState.JOG_LEFT_START
                  || _currentAnimationState == CharacterState.JOG_LEFT_PASS
                  || _currentAnimationState == CharacterState.JOG_RIGHT_START
                  || _currentAnimationState == CharacterState.JOG_RIGHT_PASS)
            {
                playerDirection = 1;
                playerSpeed = Speed * 1.5f;
            }
            else if (_currentAnimationState == CharacterState.JOG_LEFT_END
                 || _currentAnimationState == CharacterState.JOG_RIGHT_END)
            {
                playerDirection = 1;
                playerSpeed = Mathf.Lerp(playerSpeed, 0, LERP_RATE);
            }
        }


        void OnDrawGizmos()
        {
            if (_avatarModel == null || _headset == null || Camera.main.transform == null) return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(_avatarModel.transform.position, _avatarModel.transform.forward*2);

            Gizmos.color = Color.blue;
            Gizmos.DrawRay(_headset.transform.position, _headset.transform.forward * 2);

            Gizmos.color = Color.red;
            Gizmos.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * 2);
        }
    }
}