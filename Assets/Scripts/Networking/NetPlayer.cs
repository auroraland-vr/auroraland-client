using UnityEngine;
using Nakama;
using System;
using System.Collections.Generic;

// TODO Peer review with Simon
namespace Auroraland
{
    /*
	 *  Avatar controls entity sync, animation, hands position of the local player
	*/
    public class NetPlayer : NetObject
    {
        public GameObject AvatarModel;
        public Transform AvatarRoot;

        private Animator animator;
        private IKController ik;

        public void SetModel(GameObject model)
        {
            AvatarModel = model;

            animator = AvatarModel.GetComponent<Animator>();
            ik = AvatarModel.GetComponent<IKController>();
        }

       

        public override NControlData GetControlData()
        {
            // Setting up all values for the control data packet
            List<bool> values = new List<bool>
            {
                animator.GetBool("isRunKeyPressed"),
                animator.GetComponent("wasKickKeyPressed"),
                animator.GetComponent("wasSitKeyPressed"),
                animator.GetComponent("isFrontAgainstWall"),
                animator.GetComponent("isBackAgainstWall"),
                animator.GetComponent("isGrounded"),
                animator.GetComponent("isSitting"),
                animator.GetComponent("isMinimumTurnReached")
            };

            int packedBoolean = 0;

            foreach (bool val in values)
            {
                packedBoolean += (0x1 & Convert.ToInt32(val));
                packedBoolean <<= 1;
            }

            var lookAtPosition = (ik.LookAtPosition != null) ? ik.LookAtPosition : Vector3.zero;
            var leftHandPosition = (ik.LeftHandObj) ? ik.LeftHandObj.position : Vector3.zero;
            var leftHandRotation = (ik.LeftHandObj) ? ik.LeftHandObj.rotation : Quaternion.identity;
            var rightHandPosition = (ik.RightHandObj) ? ik.RightHandObj.position : Vector3.zero;
            var rightHandRotation = (ik.RightHandObj) ? ik.RightHandObj.rotation : Quaternion.identity;
            NVector3 pos = NakamaTypeConverter.Vector3ToNVector3(AvatarRoot.position);
            NVector3 rot = NakamaTypeConverter.Vector3ToNVector3(AvatarModel.transform.eulerAngles);

            // Pack all the prepared data into the packet
            NControlData controlData = new NControlData
            {
                EntityId = nEntity.Id,
                UserId = NKController.Instance.GetLocalUserId(),
                Position = pos,
                Rotation = rot,
                LookAt = NakamaTypeConverter.Vector3ToNVector3(lookAtPosition),
                LeftHandPosition = NakamaTypeConverter.Vector3ToNVector3(leftHandPosition),
                LeftHandRotation = NakamaTypeConverter.QuaternionToNVector4(leftHandRotation),
                RightHandPosition = NakamaTypeConverter.Vector3ToNVector3(rightHandPosition),
                RightHandRotation = NakamaTypeConverter.QuaternionToNVector4(rightHandRotation),
                InputMove = animator.GetFloat("userInputMove"),
                InputTurn = animator.GetFloat("userInputTurn"),
                InputStrafe = animator.GetFloat("userInputStrafe"),
                IntensityOfHeadTurn = animator.GetInteger("intensityOfHeadTurn"),
                IntensityOfLeftHand = animator.GetInteger("intensityOfLeftHand"),
                IntensityOfRightHand = animator.GetInteger("intensityOfRightHand"),
                IkModeEnabled = ik.isIKModeEnabled,
                PackedBoolean = packedBoolean
            };

            return controlData;
        }

        public override INEntity GetNEntity()
        {
            NVector3 pos = NakamaTypeConverter.Vector3ToNVector3(AvatarRoot.position);
            NVector3 rot = NakamaTypeConverter.Vector3ToNVector3(AvatarModel.transform.eulerAngles);
            NVector3 scale = NakamaTypeConverter.Vector3ToNVector3(AvatarRoot.localScale);

            nEntity.Position = pos;
            nEntity.Rotation = rot;
            nEntity.Scale = scale;
            return nEntity;
        }
    }
}
