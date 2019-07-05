using Nakama;
using UnityEngine;

namespace Auroraland
{
    /*
	 *  Actor controls entity sync, animation, hands position of the other players
	*/
    public class NetActor : NetObject
    {
        // Use this for initialization

        public GameObject ActorModel;
        private Animator animator;
        private IKSetter ik;

        const float POSITION_DIFF_THRESHOLD = 0.045f;
        const float QUATERNION_DIFF_THRESHOLD = 0.06f;

        public void SetModel(GameObject model)
        {
            ActorModel = model;
            animator = ActorModel.GetComponent<Animator>();
            ik = ActorModel.GetComponent<IKSetter>();
        }

        public override void SetControlData(INControlData controlData)
        {
            animator.SetFloat("userInputMove", controlData.InputMove);
            animator.SetFloat("userInputTurn", controlData.InputTurn);
            animator.SetFloat("userInputStrafe", controlData.InputStrafe);

            int packedBoolean = controlData.PackedBoolean;
            foreach (string key in new[] { "isMinimumTurnReached", "isSitting", "isGrounded", "isBackAgainstWall", "isFrontAgainstWall", "wasKickKeyPressed", "wasSitKeyPressed", "isRunKeyPressed" })
            {
                var val = 0x1 & (packedBoolean >> 1);
                animator.SetBool(key, val == 1);
                packedBoolean >>= 1;
            }
            /*
            animator.SetBool("isRunKeyPressed", controlData.RunKeyPressed);
            animator.SetBool("wasSitKeyPressed", controlData.SitKeyPressed);
            animator.SetBool("wasKickKeyPressed", controlData.KickKeyPressed);
            animator.SetBool("isFrontAgainstWall", controlData.FrontAgainstWall);
            animator.SetBool("isBackAgainstWall", controlData.BackAgainstWall);
            animator.SetBool("isGrounded", controlData.Grounded);
            animator.SetBool("isSitting", controlData.Sitting);
            animator.SetBool("isMinimumTurnReached", controlData.MinimumTurnReached);
            */

            //animator.SetFloat("intensityOfHeadTurn", controlData.IntensityOfHeadTurn);
            animator.SetFloat("intensityOfLeftHand", controlData.IntensityOfLeftHand);
            animator.SetFloat("intensityOfRightHand", controlData.IntensityOfRightHand);

            var lookat = NakamaTypeConverter.INVector3ToVector3(controlData.LookAt);
            var leftHandPos = NakamaTypeConverter.INVector3ToVector3(controlData.LeftHandPosition);
            var leftHandRot = NakamaTypeConverter.INVector4ToQuaternion(controlData.LeftHandRotation);
            var rightHandPos = NakamaTypeConverter.INVector3ToVector3(controlData.RightHandPosition);
            var rightHandRot = NakamaTypeConverter.INVector4ToQuaternion(controlData.RightHandRotation);

            ik.SetHands(leftHandPos, leftHandRot, rightHandPos, rightHandRot, true);

            // Reconciliation
            var position = controlData.Position;
            var rotation = controlData.Rotation;

            var parentObject = this.transform.parent.gameObject;
            var currPosition = NakamaTypeConverter.Vector3ToNVector3(parentObject.transform.position);
            var currQuaternion = NakamaTypeConverter.Vector3ToNVector3(parentObject.transform.eulerAngles);

            // if difference is big enough, snap the object to received position/rotation
            if (position.DistanceTo(currPosition) > POSITION_DIFF_THRESHOLD)
            {
                originPosition = transform.position;
                targetPosition = NakamaTypeConverter.INVector3ToVector3(position);
                needReconciliation = true;
            }
            if (rotation.DistanceTo(currQuaternion) > QUATERNION_DIFF_THRESHOLD)
            {
                originRotation = transform.eulerAngles;
                targetRotation = NakamaTypeConverter.INVector3ToVector3(rotation);
                needReconciliation = true;
            }

            var controller = GetComponent<ActorController>();
            controller.SetMovementParameters(controlData.InputMove, controlData.InputTurn);
        }
    }
}