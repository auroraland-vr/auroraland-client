/*************************************************************************\
*                           USENS CONFIDENTIAL                            *
* _______________________________________________________________________ *
*                                                                         *
* [2015] - [2017] USENS Incorporated                                      *
* All Rights Reserved.                                                    *
*                                                                         *
* NOTICE:  All information contained herein is, and remains               *
* the property of uSens Incorporated and its suppliers,                   *
* if any.  The intellectual and technical concepts contained              *
* herein are proprietary to uSens Incorporated                            *
* and its suppliers and may be covered by U.S. and Foreign Patents,       *
* patents in process, and are protected by trade secret or copyright law. *
* Dissemination of this information or reproduction of this material      *
* is strictly forbidden unless prior written permission is obtained       *
* from uSens Incorporated.                                                *
*                                                                         *
\*************************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fingo
{
    /// <summary>
    /// Stick hand is a class to drive a hand model by data from Fingo Client. 
    /// </summary>
    public class StickHand : MonoBehaviour
    {
        private Hand hand;

        [Tooltip("The hand type of the hand.")]
        public HandType HandType; //!< The hand type of the skeleton hand.

        [HideInInspector]
        public bool isDetected = false; //!< Whether or not this hand is detected.

        [Tooltip("Joint Prefab of stick hand.")]
        [SerializeField]
        private Transform jointPrefab; //!< Joint Prefab of stick hand.
        [Tooltip("Finger Tip Prefab of stick hand.")]
        [SerializeField]
        private Transform fingerTipPrefab; //!< Finger Tip Prefab of stick hand.
        [Tooltip("Bone Prefab of stick hand.")]
        [SerializeField]
        private Transform bonePrefab; //!< Bone Prefab of stick hand.
        [Tooltip("The stick hand is visible or not.")]
        public bool isVisible; //!< The stick hand is visible or not.

        private float moveScale = 1.0f; //!< The scale of joint position from original hand data.
        private float renderScale = 1.0f; //!< The render scale of the collision hand.

        private static int fingerTipNum = 5; //!< The number of all fingers.
		private static int fingerTipBonesNum = 4;	//!< The number of one finger's bones.
		private static int fingerTipJointsNum = 3;	//!< The number of one finger's joints.
		private static int palmJointsNum = 2;	//!< The number of palm joints. Notice there is one original root joint included here. This joint doesn't belong to anything.

		private Transform[] fingerTips = new Transform[fingerTipNum]; //!< the Transform of five fingers.
		private Transform[] joints = new Transform[fingerTipNum * fingerTipJointsNum + palmJointsNum]; //!< the Transform of seventeen joints of a hand.
		private Transform[] bones = new Transform[fingerTipNum * fingerTipBonesNum + 1]; //!< the Transform of twenty bones of a hand.

        private Vector3 bonePrefabScale = new Vector3(0.8f, 1, 0.8f); //!< the original scale of the bone prefab.
        private Vector3 jointPrefabScale = new Vector3(1.1f, 1.1f, 1.1f); //!< the original scale of the joint prefab.
        private Vector3 tipPrefabScale = new Vector3(1.1f, 1.1f, 1.1f); //!< the original scale of the finger tip prefab.

        void Awake()
        {
            initStickHand();
        }

        void Update()
        {
            hand = FingoMain.Instance.GetHand(HandType);
            isDetected = hand.IsDetected();
            if (isDetected)
            {
				transform.localScale = Vector3.one * moveScale;
                setJoints();
                setFingerBones();
                setPalmBones();
            }
            else
            {
                transform.localScale = Vector3.zero;
            }
        }

        /// <summary>
        /// Initialize joint, finger tip and bone gameobject.
        /// </summary>
        void initStickHand()
        {
			for (int i = 0; i < joints.Length; ++i)
            {
                if (joints[i] == null)
                {
                    joints[i] = GameObject.Instantiate(jointPrefab).transform;
                }
                joints[i].GetComponent<Renderer>().enabled = isVisible;
                joints[i].name = "Joint" + i.ToString();
                joints[i].parent = this.transform;
            }
            joints[0].GetComponent<Renderer>().enabled = false;   // Hide Root Node
			for (int i = 0; i < fingerTips.Length; ++i)
            {
                if (fingerTips[i] == null)
                {
                    fingerTips[i] = GameObject.Instantiate(fingerTipPrefab).transform;
                }
                fingerTips[i].GetComponent<Renderer>().enabled = isVisible;
                fingerTips[i].name = "Tip" + i.ToString();
                fingerTips[i].parent = this.transform;
            }
			for (int i = 0; i < bones.Length; ++i)
            {
                if (bones[i] == null)
                {
                    bones[i] = GameObject.Instantiate(bonePrefab).transform;
                }
                bones[i].GetComponent<Renderer>().enabled = isVisible;
                bones[i].name = "Bone" + i.ToString();
                bones[i].parent = this.transform;
            }
        }

        /// <summary>
        /// Set Joints and Fingertips position and rotation from Fingo Device.
        /// </summary>
        void setJoints()
        {
			for (int i = 0; i < joints.Length; ++i)
            {
                joints[i].localScale = jointPrefabScale * 0.01f / moveScale * renderScale;
                joints[i].localPosition = (hand.GetJointPosition((JointIndex)i) - hand.GetJointPosition(JointIndex.WristJoint))
                    / moveScale * renderScale + hand.GetJointPosition(JointIndex.WristJoint);
                joints[i].localRotation = hand.GetJointLocalRotation((JointIndex)i);
                joints[i].GetComponent<Renderer>().enabled = isVisible;
            }
			joints[0].GetComponent<Renderer>().enabled = false;   // Hide Root Node
			for (int i = 0; i < fingerTips.Length; ++i)
            {
                fingerTips[i].localScale = tipPrefabScale * 0.01f / moveScale * renderScale;
                fingerTips[i].localPosition = (hand.GetTipPosition((TipIndex)i) - hand.GetJointPosition(JointIndex.WristJoint))
                    / moveScale * renderScale + hand.GetJointPosition(JointIndex.WristJoint);
                fingerTips[i].GetComponent<Renderer>().enabled = isVisible;
            }
        }

        /// <summary>
        /// Set bones of a stick hand.
        /// </summary>
        void setFingerBones()
        {
			for (int i = 0; i < fingerTips.Length; ++i)
            {
                Finger finger = hand.GetFinger((FingerIndex)i);
				for (int j = 1; j < fingerTipBonesNum; ++j)
                {
                    Bone bone = finger.GetBone((BoneIndex)j);
                    Vector3 startPoint = transform.TransformPoint((bone.GetStartJointPosition() - hand.GetWristPosition()) 
                        / moveScale * renderScale + hand.GetWristPosition());
                    Vector3 endPoint = transform.TransformPoint((bone.GetEndJointPosition() - hand.GetWristPosition()) 
                        / moveScale * renderScale + hand.GetWristPosition());
                    float dis = bone.GetLength() / moveScale * renderScale;
                    // scale
					bones[fingerTipBonesNum * i + j].localScale = new Vector3(
                        0.01f * bonePrefabScale.x / moveScale * renderScale,
                        dis * .5f * bonePrefabScale.y,
                        0.01f * bonePrefabScale.z / moveScale * renderScale);
                    // position
					bones[fingerTipBonesNum * i + j].position = (startPoint + endPoint) * .5f;
                    // rotation
					bones[fingerTipBonesNum * i + j].localRotation = Quaternion.LookRotation(-bone.GetNormalDirection(), bone.GetUpDirection());
                    // visibility
					bones[fingerTipBonesNum * i + j].GetComponent<Renderer>().enabled = isVisible;
                }
            }
        }

        void setPalmBones()
        {
			// These joints are palm and fingers' root joints.
            JointIndex[] jointIndex = new JointIndex[6] {
                JointIndex.WristJoint,
                JointIndex.ThumbProximal,
                JointIndex.IndexProximal,
                JointIndex.MiddleProximal,
                JointIndex.RingProximal,
                JointIndex.PinkyProximal
            };

			for (int i = 0; i < jointIndex.Length; ++i)
            {
                Vector3 startJointPos = hand.GetJointPosition(jointIndex[i]);
                Vector3 endJointPos = hand.GetJointPosition(jointIndex[(i + 1) % 6]);

                Vector3 startPoint = transform.TransformPoint((startJointPos - hand.GetWristPosition()) 
                    / moveScale * renderScale + hand.GetWristPosition());
                Vector3 endPoint = transform.TransformPoint((endJointPos - hand.GetWristPosition()) 
                    / moveScale * renderScale + hand.GetWristPosition());
                
                float dis = Vector3.Distance(startJointPos, endJointPos);

                Vector3 dir = endJointPos - startJointPos;
                dir = Camera.main.transform.TransformDirection(dir);
                
                // scale
                bones[fingerTipBonesNum * i].localScale = new Vector3(
                    0.01f * bonePrefabScale.x / moveScale * renderScale,
                    dis * .5f * bonePrefabScale.y,
                    0.01f * bonePrefabScale.z / moveScale * renderScale);
                // position
				bones[fingerTipBonesNum * i].position = (startPoint + endPoint) * .5f;
                // rotation
                bones[fingerTipBonesNum * i].rotation = Quaternion.LookRotation(hand.GetPalmNormal(), dir);
                // visibility
                bones[fingerTipBonesNum * i].GetComponent<Renderer>().enabled = isVisible;
            }
        }
    }
}