using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

namespace AuroraShmup {
    /// <summary>
    /// Spawns the Arcade Machine's GameUI on enable/disable
    /// </summary>
    public class ArcadeMachineManager : MonoBehaviour
    {
        /// <summary>
        /// The GameUI prefab object
        /// </summary>
        public GameObject GameUIPrefab;
        /// <summary>
        /// Whether the ArcadeMachine is available to engage
        /// </summary>
        public bool isAvailable { get; private set; }

        /// <summary>
        /// A reference to the GameUI GameObject
        /// </summary>
        private GameObject GameUI;
        /// <summary>
        /// A reference to GameUI's game manager script
        /// </summary>
        private GameManager gameManager;
        /// <summary>
        /// The AvatarController of the avatar playing the arcade game
        /// </summary>
        private Auroraland.AvatarController engagedAvatar;
        /// <summary>
        /// Whether avatar player is controlling machine via NonVR input
        /// </summary>
        private bool nonVRInputEnabled;
        /// <summary>
        /// Whether trigger is being pressed
        /// </summary>
        private bool triggerPressed;

        /// <summary>
        /// Disables movement for avatar player that is currently playing arcade 
        /// </summary>
        public void Engage(Auroraland.AvatarController avatarController)
        {
            isAvailable = false;
            engagedAvatar = avatarController;
            engagedAvatar.AllowMovement = false;
            if (string.Equals(Auroraland.SDK_SetupMode.Instance.CurrentSDKName, "NonVR"))
                nonVRInputEnabled = true;
            else {
                engagedAvatar.RightHandController._vrtkControllerEvents.TouchpadTouchStart += ArcadeEvent_TouchpadTouched;
                engagedAvatar.RightHandController._vrtkControllerEvents.TouchpadTouchEnd += ArcadeEvent_TouchpadTouched;
                engagedAvatar.RightHandController._vrtkControllerEvents.TouchpadAxisChanged += ArcadeEvent_TouchpadTouched;
                engagedAvatar.RightHandController._vrtkControllerEvents.TriggerReleased += ArcadeEvent_TriggerReleased;
            }
            // TODO: ENFORCE RULE: ONLY PLAYERS FACING THE MACHINE CAN ENGAGE
        }

        /// <summary>
        /// Returns avatar player's control scheme to original setup
        /// </summary>
        public void Disengage()
        {
            isAvailable = true;
            engagedAvatar.AllowMovement = true;
            if (nonVRInputEnabled)
                nonVRInputEnabled = false;
            else {
                engagedAvatar.RightHandController._vrtkControllerEvents.TouchpadTouchEnd -= ArcadeEvent_TouchpadTouched;
                engagedAvatar.RightHandController._vrtkControllerEvents.TouchpadTouchStart -= ArcadeEvent_TouchpadTouched;
                engagedAvatar.RightHandController._vrtkControllerEvents.TouchpadAxisChanged -= ArcadeEvent_TouchpadTouched;
                engagedAvatar.RightHandController._vrtkControllerEvents.TriggerReleased -= ArcadeEvent_TriggerReleased;
                triggerPressed = false;
            }
        }

        /// <summary>
        /// Maps the avatar player's fire input to interact with the arcade game
        /// </summary>
        /// <param name="avatarPlayer">The GameObject of the avatar player</param>
        public void EnableInteractivity(GameObject avatarPlayer)
        {
            avatarPlayer.GetComponent<Auroraland.AvatarController>().RightHandController.
                _vrtkControllerEvents.TriggerPressed += ArcadeEvent_TriggerPressed;
        }

        /// <summary>
        /// Unmaps the avatar player's fire input from interacting with arcade game
        /// </summary>
        /// <param name="avatarPlayer">The GameObject of the avatar player</param>
        public void DisableInteractivity(GameObject avatarPlayer)
        {
            avatarPlayer.GetComponent<Auroraland.AvatarController>().RightHandController.
                _vrtkControllerEvents.TriggerPressed -= ArcadeEvent_TriggerPressed;
        }

        private void Start()
        {
            isAvailable = true;
            nonVRInputEnabled = false;
            triggerPressed = false;
            CullGameUI();
            GameUI = Instantiate(GameUIPrefab, transform.position, transform.rotation, transform.parent);
            GameUI.GetComponent<VRTK_TransformFollow>().gameObjectToFollow = gameObject;
            gameManager = GameUI.transform.Find("GameCanvas").GetComponent<GameManager>();
            gameManager.arcadeManager = this;
        }

        private void OnDisable()
        {
            Destroy(GameUI);
        }

        /// <summary>
        /// Culls the arcade game UI from all cameras.
        /// Called prior to instantiating new game ui which contains its own 
        /// render camera
        /// </summary>
        private void CullGameUI() {
            Camera[] cameras = Camera.allCameras;
            foreach (var cam in cameras) {
                cam.cullingMask &= ~LayerMask.GetMask("GameUI");
            }
        }

        /// <summary>
        /// Callback event for when trigger is pressed
        /// </summary>
        private void ArcadeEvent_TriggerPressed(object sender, ControllerInteractionEventArgs e)
        {
            Debug.Log("ArcadeTrigger");
            Auroraland.AvatarController avatarController = e.controllerReference.actual.transform.Find("Right Controller").
                GetComponent<Auroraland.HandController>()._avatar;
            if (gameManager.isRunning)
            {
                if (engagedAvatar == avatarController)
                {
                    triggerPressed = true;
                }
                    
            }
            else if (!gameManager.isRestarting)
            {
                if (isAvailable)
                {
                    Engage(avatarController);
                    gameManager.StartGame();
                }
            }
        }

        /// <summary>
        /// Callback event for when trigger is released
        /// </summary>
        private void ArcadeEvent_TriggerReleased (object sender, ControllerInteractionEventArgs e)
        {
            triggerPressed = false;
        }

        /// <summary>
        /// Callback event for when touchpad is touched
        /// </summary>
        private void ArcadeEvent_TouchpadTouched(object sender, ControllerInteractionEventArgs e)
        {
            gameManager.shipPlayer.InputHorizontal(e.touchpadAxis.x);
            gameManager.shipPlayer.InputVertical(e.touchpadAxis.y);
        }

        private void Update()
        {
            if (gameManager.isRunning)
            {
                if (nonVRInputEnabled)
                    ProcessNonVRInput();
                if (triggerPressed)
                    gameManager.shipPlayer.Fire();
            }
        }

        /// <summary>
        /// Processes the user input.
        /// </summary>
        private void ProcessNonVRInput()
        {
            float HorizontalInput = Input.GetAxis("Horizontal");
            float VerticalInput = Input.GetAxis("Vertical");
            gameManager.shipPlayer.InputHorizontal(HorizontalInput);
            gameManager.shipPlayer.InputVertical(VerticalInput);
        }
    }
}
