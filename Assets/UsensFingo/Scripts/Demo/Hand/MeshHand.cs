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

namespace Fingo
{
    /// <summary>
    /// Mesh hand is a class to drive a hand model by data from Fingo Client. 
    /// </summary>
    public class MeshHand : MonoBehaviour
    {
        private Hand hand;

        [Tooltip("The hand type of the hand.")]
        public HandType handType; //!< The hand type of the hand.

        [Tooltip("The move scale of joint position from original hand data.")]
        //[SerializeField]
        private float moveScale = 1.0f; //!< The scale of joint position from original hand data.
        [Tooltip("The render scale of the collision hand.")]
        //[SerializeField]
        private float renderScale = 1.0f; //!< The render scale of the collision hand.

        [HideInInspector]
        public bool isDetected = false; //!< Whether or not this hand is detected.

        [Tooltip("Enable this switch to set hand in stable mode. (Comming soon...)")]
        public bool enableStabilizer = false;           //!< Enable this switch to set hand in stable mode.

        [Header("Hand Joint Mapping")]
        //public Transform root;                         //!< The Transform of root of this hand.
        public Transform wrist;                        //!< The Transform of wrist of this hand.
        public Transform thumbProximal;                //!< The Transform of thumb proximal of this hand.        
        public Transform thumbIntermediate;            //!< The Transform of thumb intermediate of this hand.            
        public Transform thumbDistal;                  //!< The Transform of thumb distal of this hand.      
        public Transform thumbTip;                     //!< The Transform of thumb tip of this hand.   
        public Transform indexProximal;                //!< The Transform of index proximal of this hand.        
        public Transform indexIntermediate;            //!< The Transform of index intermediate of this hand.            
        public Transform indexDistal;                  //!< The Transform of index distal of this hand.      
        public Transform indexTip;                     //!< The Transform of index tip of this hand.   
        public Transform middleProximal;               //!< The Transform of middle proximal of this hand.         
        public Transform middleIntermediate;           //!< The Transform of middle intermediate of this hand.             
        public Transform middleDistal;                 //!< The Transform of middle distal of this hand.       
        public Transform middleTip;                    //!< The Transform of middle tip of this hand.    
        public Transform ringProximal;                 //!< The Transform of ring proximal of this hand.       
        public Transform ringIntermediate;             //!< The Transform of ring intermediate of this hand.           
        public Transform ringDistal;                   //!< The Transform of ring distal of this hand.     
        public Transform ringTip;                      //!< The Transform of ring tip of this hand.  
        public Transform pinkyProximal;                //!< The Transform of pinky proximal of this hand.        
        public Transform pinkyIntermediate;            //!< The Transform of pinky intermediate of this hand.            
        public Transform pinkyDistal;                  //!< The Transform of pinky distal of this hand.      
        public Transform pinkyTip;                     //!< The Transform of pinky tip of this hand.  

        private void Start()
        {
			//disable TWP will make fingo2.0 20180122fw working with 1.2.6j 20180206
            FingoMain.Instance.SetHandTimeWarping(false);
        }

        void Update()
        {
            hand = FingoMain.Instance.GetHand(handType);

            isDetected = hand.IsDetected();

            if (isDetected)
            {
                transform.localScale = Vector3.one * renderScale;
            }
            else
            {
                transform.localScale = Vector3.zero;
            }

            UpdateMeshHand();
        }

        /// <summary>
        /// Update mesh hand data.
        /// </summary>
        void UpdateMeshHand()
        {
            if (isDetected)
            {
                SetMeshHandPosition();
                SetMeshHandRotation();
            }
        }

        /// <summary>
        /// Set Mesh hand position.
        /// </summary>
        void SetMeshHandPosition()
        {
			wrist.localPosition = hand.GetWristPosition () * moveScale / renderScale;
        }

        /// <summary>
        /// Set Mesh hand joint rotation.
        /// </summary>
        void SetMeshHandRotation()
        {
            if (hand.IsDetected())
            {
                //root.localRotation = hand.GetJointLocalRotation(JointIndex.RootJoint);
                wrist.localRotation = hand.GetJointLocalRotation(JointIndex.WristJoint);
                thumbProximal.localRotation = hand.GetJointLocalRotation(JointIndex.ThumbProximal);
                thumbIntermediate.localRotation = hand.GetJointLocalRotation(JointIndex.ThumbIntermediate);
                thumbDistal.localRotation = hand.GetJointLocalRotation(JointIndex.ThumbDistal);
                thumbTip.localRotation = hand.GetTipLocalRotation(TipIndex.ThumbTip);
                indexProximal.localRotation = hand.GetJointLocalRotation(JointIndex.IndexProximal);
                indexIntermediate.localRotation = hand.GetJointLocalRotation(JointIndex.IndexIntermediate);
                indexDistal.localRotation = hand.GetJointLocalRotation(JointIndex.IndexDistal);
                indexTip.localRotation = hand.GetTipLocalRotation(TipIndex.IndexTip);
                middleProximal.localRotation = hand.GetJointLocalRotation(JointIndex.MiddleProximal);
                middleIntermediate.localRotation = hand.GetJointLocalRotation(JointIndex.MiddleIntermediate);
                middleDistal.localRotation = hand.GetJointLocalRotation(JointIndex.MiddleDistal);
                middleTip.localRotation = hand.GetTipLocalRotation(TipIndex.MiddleTip);
                ringProximal.localRotation = hand.GetJointLocalRotation(JointIndex.RingProximal);
                ringIntermediate.localRotation = hand.GetJointLocalRotation(JointIndex.RingIntermediate);
                ringDistal.localRotation = hand.GetJointLocalRotation(JointIndex.RingDistal);
                ringTip.localRotation = hand.GetTipLocalRotation(TipIndex.RingTip);
                pinkyProximal.localRotation = hand.GetJointLocalRotation(JointIndex.PinkyProximal);
                pinkyIntermediate.localRotation = hand.GetJointLocalRotation(JointIndex.PinkyIntermediate);
                pinkyDistal.localRotation = hand.GetJointLocalRotation(JointIndex.PinkyDistal);
                pinkyTip.localRotation = hand.GetTipLocalRotation(TipIndex.PinkyTip);
            }
        }
    }
}