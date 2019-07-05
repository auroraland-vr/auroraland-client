using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Auroraland{
	[RequireComponent(typeof(Animator))]
	public class IKSetter : MonoBehaviour {
        
        private bool isIKModeEnabled = false;
        private Vector3 rightHandPosition;
		private Quaternion rightHandRotation;
		private Vector3 leftHandPosition;
		private Quaternion leftHandRotation;
		private Vector3 lookAtPosition; 
		protected Animator animator;

        private float weightLeftHand = 0;
        private float weightRightHand = 0;

        private const float BASE_RATE = 0.175f;
        private const float BASE_FACTOR = 0.5f;

        // Use this for initialization
        void Start () {
			animator = GetComponent<Animator>();
		}
		// Update is called once per frame
		void OnAnimatorIK(){
			if (animator) {

                if (isIKModeEnabled)
                {
                    if (weightRightHand < 0.99f)
                        weightRightHand = Mathf.Lerp(weightRightHand, 1.0f, BASE_RATE * BASE_FACTOR);
                    else
                        weightRightHand = 1.0f;
                }
                else
                {
                    if (weightRightHand > 0.01f)
                        weightRightHand = Mathf.Lerp(weightRightHand, 0, BASE_RATE);
                    else
                        weightRightHand = 0.0f;
                }


                // animator.SetLookAtWeight (1);
				// animator.SetLookAtPosition (lookAtPosition);
				animator.SetIKPositionWeight(AvatarIKGoal.RightHand, weightRightHand);
				animator.SetIKRotationWeight(AvatarIKGoal.RightHand, weightRightHand);
				animator.SetIKPositionWeight (AvatarIKGoal.LeftHand, weightLeftHand);
				animator.SetIKRotationWeight (AvatarIKGoal.LeftHand, weightLeftHand);
				animator.SetIKPosition (AvatarIKGoal.RightHand, rightHandPosition);
				animator.SetIKRotation (AvatarIKGoal.RightHand, rightHandRotation);
				animator.SetIKPosition (AvatarIKGoal.LeftHand, leftHandPosition);
				animator.SetIKRotation (AvatarIKGoal.LeftHand, leftHandRotation);
			}
		}
		public void SetIK(Vector3 lookAtPos, Vector3 leftHandPos, Quaternion leftHandRot, Vector3 rightHandPos,Quaternion rightHandRot, bool inputIsIKModeEnabled){
			lookAtPosition = lookAtPos;
            SetHands(leftHandPos, leftHandRot, rightHandPos, rightHandRot, inputIsIKModeEnabled);
		}

        public void SetHands(Vector3 leftHandPos, Quaternion leftHandRot, Vector3 rightHandPos, Quaternion rightHandRot, bool inputIsIKModeEnabled)
        {
            isIKModeEnabled = inputIsIKModeEnabled;
			leftHandPosition = leftHandPos;
			leftHandRotation = leftHandRot;
			rightHandPosition = rightHandPos;
			rightHandRotation = rightHandRot;
        }
	}
}