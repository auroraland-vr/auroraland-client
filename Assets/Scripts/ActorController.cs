using UnityEngine;

namespace Auroraland
{
    public class ActorController : MonoBehaviour
    {
        GameObject avatar;

        // Base Factor Variables
        public const int BASE_FRAME = 1;
        public const float BASE_RATE = 0.175f; // higher number = faster interpolation
        public static float BASE_SPEED = 1.5f;
        public float BASE_TURN = 0.703125f;
        public float BASE_RANGE = 78.75f;
        float RUN_SPEED = BASE_SPEED * 1.5f;
        float STRAFE_SPEED = BASE_SPEED * 0.5f;

        public bool AllowMovement = true;

        // Control Variables
        float playerSpeed;
        /// <summary>
        /// At 0, player will not move. At 1 or -1, moves forward/backward respectively.
        /// </summary>
        float playerDirection;
        float playerRotation;
        float targetRotation;

        // Position Variables
        float totalRotation = 0;
        Vector3 playerVector;

        // Turning Variables
        int intensityOfHeadTurn;

        // The current y move speed
        float verticalSpeed;
        float adjustedGravity;
        const float GRAVITY = 9.8f;
        const float OVER_NINE_THOUSAND = 9001.0f;
        const float UNDER_NINE_THOUSAND = 0.001f;

        /*
         * Input Variables
         */
        float inputMove;
        float inputTurn;

        bool isRunKeyHeld = false;
        bool wasSitKeyPressed = false; //ctrl key pressed down once
        bool wasKickKeyPressed = false;  //k key pressed down once

        /*
         * Collision Variables
         */
        CharacterController controller;

        /*
         * State Variables
         */
        CharacterState status;
        internal int currentBaseState;   // a reference to the current state of the animator, used for base layer

        internal bool isMinimumTurnReached = false;
        internal bool isSitting = false;

        void Start()
        {
            controller = GetComponent<CharacterController>();
            status = avatar.GetComponent<CharacterState>();

            isRunKeyHeld = false;
            wasSitKeyPressed = false;
            wasKickKeyPressed = false;

            controller.Move(Vector3.down * OVER_NINE_THOUSAND);
            transform.localRotation = Quaternion.identity;

            totalRotation = 0;

            currentBaseState = status.GetCurrentState(0);
        }

        void Update()
        {

            controller.Move(Vector3.down * UNDER_NINE_THOUSAND);

            if (currentBaseState != status.GetCurrentState(0))
                currentBaseState = status.GetCurrentState(0);

            if (AllowMovement)
                UpdatePlayerMovement();
            else
            {
                Animator animator = avatar.GetComponent<Animator>();
                animator.SetFloat("userInputMove", 0);
                animator.SetFloat("userInputTurn", 0);
                animator.SetBool("wasSitKeyPressed", false);
                animator.SetBool("wasKickKeyPressed", false);
                animator.SetBool("isRunKeyPressed", false);
                animator.SetBool("isMinimumTurnReached", true);
                animator.SetInteger("intensityOfHeadTurn", 0);
            }

            ApplyGravity();
        }

        public void ApplyGravity()
        {
            if (controller.isGrounded)
            {
                verticalSpeed = 0.0f;
                adjustedGravity = 0.0f;
                avatar.GetComponent<Animator>().SetBool("isGrounded", true);
            }
            else
            {
                adjustedGravity = Mathf.Lerp(adjustedGravity, GRAVITY, BASE_RATE * 0.5f);
                verticalSpeed -= adjustedGravity * Time.deltaTime;
                avatar.GetComponent<Animator>().SetBool("isGrounded", false);
            }
        }

         public void UpdatePlayerMovement()
        {

            isMinimumTurnReached = false;   // assume target rotation has not been met

            isRunKeyHeld = Input.GetButton("Run");
            wasSitKeyPressed = Input.GetButtonDown("Sit");
            wasKickKeyPressed = Input.GetButtonDown("Kick");

//            intensityOfHeadTurn = DetermineAutoTurn();

            NewMethod();

            Animator animator = avatar.GetComponent<Animator>();
            animator.SetFloat("userInputMove", inputMove);
            animator.SetFloat("userInputTurn", inputTurn);
            animator.SetBool("wasSitKeyPressed", wasSitKeyPressed);
            animator.SetBool("wasKickKeyPressed", wasKickKeyPressed);
            animator.SetBool("isRunKeyPressed", isRunKeyHeld);
            animator.SetBool("isMinimumTurnReached", isMinimumTurnReached);
            animator.SetInteger("intensityOfHeadTurn", intensityOfHeadTurn);

            playerVector = (transform.forward * playerDirection * playerSpeed + new Vector3(0, verticalSpeed)) * Time.deltaTime;
            controller.Move(playerVector);
        }

        private void NewMethod()
        {
            if (currentBaseState == CharacterState.IDLE || currentBaseState == CharacterState.SIT_IDLE)
            {
                targetRotation = 0;
                playerDirection = 0;
                playerSpeed = 0;
                totalRotation = 0;
            }
            else if (currentBaseState == CharacterState.TURN_LEFT
                  || currentBaseState == CharacterState.TURN_RIGHT)
            {
                targetRotation = BASE_TURN / 2;
                playerDirection = 0;
                playerSpeed = 0;

                totalRotation += playerRotation;

                if (currentBaseState == CharacterState.TURN_LEFT)
                {
                    if (totalRotation <= -BASE_TURN * 5)
                        isMinimumTurnReached = true;
                    else
                        inputTurn = -1;
                }
                else if (currentBaseState == CharacterState.TURN_RIGHT)
                {
                    if (totalRotation >= BASE_TURN * 5)
                        isMinimumTurnReached = true;
                    else
                        inputTurn = 1;
                }
            }
            else if (currentBaseState == CharacterState.WALK_START)
            {
                playerSpeed = Mathf.Lerp(playerSpeed, BASE_SPEED, BASE_RATE * 0.5f);
                targetRotation = BASE_TURN * 0.75f;
                playerDirection = 1.0f;
            }
            else if (currentBaseState == CharacterState.WALK_LEFT_START
                  || currentBaseState == CharacterState.WALK_LEFT_PASS
                  || currentBaseState == CharacterState.WALK_RIGHT_START
                  || currentBaseState == CharacterState.WALK_RIGHT_PASS)
            {
                targetRotation = BASE_TURN * 0.75f;
                playerDirection = 1.0f;
                playerSpeed = BASE_SPEED;
            }
            else if (currentBaseState == CharacterState.WALK_LEFT_END
                || currentBaseState == CharacterState.WALK_RIGHT_END
                || currentBaseState == CharacterState.IDLE_KICK)
            {
                playerDirection = 1.0f;
                playerSpeed = Mathf.Lerp(playerSpeed, 0, BASE_RATE * 0.5f);
            }
            else if (currentBaseState == CharacterState.WALKBACK_START)
            {
                targetRotation = BASE_TURN;
                playerDirection = -1.0f;
                playerSpeed = Mathf.Lerp(playerSpeed, BASE_SPEED * 0.625f, BASE_RATE * 0.5f);
            }
            else if (currentBaseState == CharacterState.WALKBACK_LEFT_START
                  || currentBaseState == CharacterState.WALKBACK_LEFT_PASS
                  || currentBaseState == CharacterState.WALKBACK_RIGHT_START
                  || currentBaseState == CharacterState.WALKBACK_RIGHT_PASS)
            {
                targetRotation = BASE_TURN;
                playerDirection = -1.0f;
                playerSpeed = BASE_SPEED * 0.625f;
            }
            else if (currentBaseState == CharacterState.WALKBACK_LEFT_END
                  || currentBaseState == CharacterState.WALKBACK_RIGHT_END)
            {
                playerDirection = -1.0f;
                playerSpeed = Mathf.Lerp(playerSpeed, 0, BASE_RATE * 0.5f);
            }
            else if (currentBaseState == CharacterState.JOG_START)
            {
                targetRotation = BASE_TURN * 0.375f;
                playerDirection = 1.0f;
                playerSpeed = Mathf.Lerp(playerSpeed, RUN_SPEED, BASE_RATE);

            }
            else if (currentBaseState == CharacterState.JOG_LEFT_START
                  || currentBaseState == CharacterState.JOG_LEFT_PASS
                  || currentBaseState == CharacterState.JOG_RIGHT_START
                  || currentBaseState == CharacterState.JOG_RIGHT_PASS)
            {
                targetRotation = BASE_TURN * 0.375f;
                playerDirection = 1.0f;
                playerSpeed = RUN_SPEED;
            }
            else if (currentBaseState == CharacterState.JOG_KICK)
            {
                targetRotation = 0;
                playerDirection = 1.0f;
                playerSpeed = RUN_SPEED;
            }
            else if (currentBaseState == CharacterState.JOG_LEFT_END
                 || currentBaseState == CharacterState.JOG_RIGHT_END)
            {
                targetRotation = 0;
                playerDirection = 1.0f;
                playerSpeed = Mathf.Lerp(playerSpeed, 0, BASE_RATE);
            }
            else //not a controllable state
            {
                playerRotation = 0;
                playerSpeed = 0;
                playerDirection = 0;
            }

            if (inputTurn == 0)
                playerRotation = Mathf.Lerp(playerRotation, 0, BASE_RATE * 0.75f);
            else
                playerRotation = Mathf.Lerp(playerRotation, targetRotation * inputTurn, BASE_RATE * 0.75f);

            transform.Rotate(new Vector3(0, playerRotation, 0));
        }

        public void SetMovementParameters(float move, float turn)
        {
            inputMove = move;
            inputTurn = turn;
        }

        public void ChangeAvatar(GameObject newAvatar)
        {
            avatar = newAvatar;
            status = newAvatar.GetComponent<CharacterState>();
        }
    }
}