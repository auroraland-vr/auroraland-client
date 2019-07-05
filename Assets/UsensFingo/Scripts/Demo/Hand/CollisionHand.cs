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
    /// Collision hand is a class to drive a hand collider by data from Fingo Client. 
    /// </summary>
    public class CollisionHand : MonoBehaviour
    {
        private Hand hand;

        [Tooltip("The hand type of the hand.")]
        public HandType HandType; //!< The hand type of the hand.

        [Tooltip("The move scale of joint position from original hand data.")]
        [SerializeField]
        private float moveScale = 1.0f; //!< The move scale of joint position from original hand data.
        [Tooltip("The render scale of the collision hand.")]
        [SerializeField]
        private float renderScale = 1.0f; //!< The render scale of the collision hand.

        [HideInInspector]
        public bool isDetected = false; //!< Whether or not this hand is detected.

        [Tooltip("Bone Prefab of Collision hand component.")]
        [SerializeField]
        private Transform handColliderPrefab; //!< Bone Prefab of Collision hand component.

        private Transform[] bones = new Transform[15]; //!< the Transform of fifteen finger bones. 
        private Transform palm; //!< the Transform of the palm.

        [HideInInspector]
        public bool[] EnableBone = new bool[15] { true, true, true, true, true, true,
            true, true, true, true, true, true, true, true, true };
        [HideInInspector]
        public bool EnablePalm = true;

        void Awake()
        {
            iniCollisionHand();
        }

        void Update()
        {
            hand = FingoMain.Instance.GetHand(HandType);
            isDetected = hand.IsDetected();
            if (isDetected)
            {
				transform.localScale = Vector3.one * moveScale;
                setBones();
                setPalm();
            }
            else
            {
                transform.localScale = Vector3.zero;
            }
            for(int i = 0; i < 15; ++i)
            {
                bones[i].GetComponent<Collider>().enabled = isDetected && EnableBone[i];
            }
            palm.GetComponent<Collider>().enabled = isDetected && EnablePalm;
        }

        /// <summary>
        /// Initialize joint, finger tip and bone gameobject.
        /// </summary>
        void iniCollisionHand()
        {
            for (int i = 0; i < 15; ++i)
            {
                if (bones[i] == null)
                {
                    bones[i] = Instantiate(handColliderPrefab).transform;
                }
                bones[i].name = "Bone Collider " + i.ToString();
                bones[i].parent = this.transform;
            }
            if (palm == null)
            {
                palm = Instantiate(handColliderPrefab).transform;
                palm.name = "Palm Collider";
                palm.parent = this.transform;
            }
        }

        /// <summary>
        /// Set bones of a collision hand.
        /// </summary>
        void setBones()
        {
            for (int i = 0; i < 5; ++i)
            {
                Finger finger = hand.GetFinger((FingerIndex)i);
                for(int j = 1; j < 4; ++j)
                {
                    Bone bone = finger.GetBone((BoneIndex)j);
                    Vector3 startPoint = transform.TransformPoint((bone.GetStartJointPosition() - hand.GetWristPosition()) / moveScale * renderScale + hand.GetWristPosition());
                    Vector3 endPoint = transform.TransformPoint((bone.GetEndJointPosition() - hand.GetWristPosition()) / moveScale * renderScale + hand.GetWristPosition());
                    float dis = bone.GetLength() * renderScale / moveScale;
                    bones[3 * i + j - 1].localScale = new Vector3(0.01f / moveScale * renderScale, dis, 0.01f / moveScale * renderScale);
                    bones[3 * i + j - 1].position = (startPoint + endPoint) * .5f;
                    bones[3 * i + j - 1].localRotation = Quaternion.LookRotation(-bone.GetNormalDirection(),bone.GetUpDirection());
                    bones[3 * i + j - 1].gameObject.GetComponent<Renderer>().enabled = true;
                }
            }
        }

        /// <summary>
        /// Set palm of a collision hand.
        /// </summary>
        void setPalm()
        {
            Vector3 indexProximalPosition = (hand.GetJointPosition(JointIndex.IndexProximal) - hand.GetJointPosition(JointIndex.WristJoint))
                 / moveScale * renderScale + hand.GetJointPosition(JointIndex.WristJoint);
            Vector3 pinkyProximalPosition = (hand.GetJointPosition(JointIndex.PinkyProximal) - hand.GetJointPosition(JointIndex.WristJoint))
                 / moveScale * renderScale + hand.GetJointPosition(JointIndex.WristJoint);
            Vector3 palmPos = transform.rotation * (((indexProximalPosition + pinkyProximalPosition) * .5f + hand.GetWristPosition()) * .5f);
            float scale_x = Vector3.Distance(indexProximalPosition, pinkyProximalPosition);
            float scale_y = Vector3.Distance((indexProximalPosition + pinkyProximalPosition) * .5f, hand.GetWristPosition());
            float scale_z = 0.01f / moveScale * renderScale;
            Quaternion palmWorldRotation = transform.rotation * hand.GetWristRotation() * Quaternion.Inverse(transform.localRotation);
            palm.localScale = new Vector3(scale_x, scale_y, scale_z);
            palm.GetComponent<Rigidbody>().MovePosition(new Vector3(palmPos.x * transform.lossyScale.x, palmPos.y * transform.lossyScale.y, palmPos.z * transform.lossyScale.z)
                    + transform.position);
            palm.GetComponent<Rigidbody>().MoveRotation(Quaternion.Euler(palmWorldRotation.eulerAngles));
            if (Application.platform == RuntimePlatform.Android)
            {
                palm.position = new Vector3(palmPos.x * transform.lossyScale.x, palmPos.y * transform.lossyScale.y, palmPos.z * transform.lossyScale.z) + transform.position;
                palm.rotation = Quaternion.Euler(palmWorldRotation.eulerAngles);
            }

            palm.position = new Vector3(palmPos.x * transform.lossyScale.x, palmPos.y * transform.lossyScale.y, palmPos.z * transform.lossyScale.z) + transform.position;
            palm.GetComponent<Collider>().enabled = isDetected && EnablePalm;
        }

        /// <summary>
        /// Enable all the colliders on the hand.
        /// </summary>
        public void EnableAllCollision()
        {
            for(int i = 0; i < 15; ++i)
            {
                EnableBone[i] = true;
            }
            EnablePalm = true;
        }

        /// <summary>
        /// Disable all the colliders on the hand.
        /// </summary>
        public void DisableAllCollision()
        {
            for (int i = 0; i < 15; ++i)
            {
                EnableBone[i] = false;
            }
            EnablePalm = false;
        }
    }
}