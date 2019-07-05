using UnityEngine;
using VRTK;

namespace Auroraland
{
    [RequireComponent(typeof(Animator))]
    public class IKController : MonoBehaviour
    {
        public float weight = 0.625f;
        public float bodyWeight = 0.0625f;
        public float headWeight = 1f;
        public float eyesWeight = 1f;

        public float weightLeftHand = 0;
        public float weightRightHand = 0;

        internal bool isIKModeEnabled = false;

        internal bool isRightHandIKEnabled = false;
        internal bool isLeftHandIKEnabled = false;

        internal int intensityOfRightHand = 0;
        internal int intensityOfLeftHand = 0;

        public Transform RightHandObj;
        public Transform LeftHandObj;
        public Transform HeadObj;
        public Vector3 LookAtPosition;

        public const float BASE_RATE = 0.175f;
        public float BASE_FACTOR = 0.1f;

        protected Animator animator;

        void OnEnable()
        {
            VRTK_SDKManager.instance.LoadedSetupChanged += VRTK_SDKManager_LoadedSetupChanged;
            InitObjects();
        }

        private void OnDisable()
        {
            VRTK_SDKManager.instance.LoadedSetupChanged -= VRTK_SDKManager_LoadedSetupChanged;
        }

        void Start()
        {
            animator = GetComponent<Animator>();
        }

        void VRTK_SDKManager_LoadedSetupChanged(VRTK_SDKManager sender, VRTK_SDKManager.LoadedSetupChangeEventArgs e)
        {
            InitObjects();
        }

        /// <summary>
        /// Initializes Head and Hand objects; called by OnEnable and when SDK setup is changed
        /// </summary>
        void InitObjects()
        {
            if (VRTK_DeviceFinder.HeadsetCamera())
                HeadObj = VRTK_DeviceFinder.HeadsetCamera();

            if (VRTK_DeviceFinder.GetControllerRightHand())
            {
                RightHandObj = VRTK_DeviceFinder.GetControllerRightHand().transform;
                isRightHandIKEnabled = true;
            }

            if (VRTK_SDKManager.instance.loadedSetup == GetNonVRSetup())
            {
                LeftHandObj = null; // Disabling left hand IK for NonVR mode
                isLeftHandIKEnabled = false;
                //TODO check why is left controller getting null reference 
                if (VRTK_DeviceFinder.GetControllerLeftHand())
                    VRTK_DeviceFinder.GetControllerLeftHand().GetComponent<AutoRaycaster>().enabled = false;
            }
            else if (VRTK_DeviceFinder.GetControllerLeftHand())
            {
                LeftHandObj = VRTK_DeviceFinder.GetControllerLeftHand().transform;
                isLeftHandIKEnabled = false;
            }
        }

        /// <summary>
        /// Returns NonVR SDK Setup from SDKManager
        /// </summary>
        /// <returns>NonVR SDK setup if found, else null</returns>
        VRTK_SDKSetup GetNonVRSetup()
        {
            foreach (VRTK_SDKSetup setup in VRTK_SDKManager.instance.setups)
            {
                if (setup.name.Contains("NonVR")) return setup;
            }
            return null;
        }

        void OnAnimatorIK()
        {
            if (animator)
            {
                if (HeadObj != null)
                {
                    animator.SetLookAtWeight(weight, bodyWeight, headWeight, eyesWeight, 1f);
                    LookAtPosition = HeadObj.position + HeadObj.forward;
                    animator.SetLookAtPosition(LookAtPosition);
                }

                if (RightHandObj != null && VRTK_DeviceFinder.GetControllerRightHand())
                {
                    if ((VRTK_DeviceFinder.GetControllerRightHand().activeInHierarchy && isRightHandIKEnabled)
                        || isIKModeEnabled)
                    {
                        if (weightRightHand < 0.99f)
                            weightRightHand = Mathf.Lerp(weightRightHand, 1, BASE_RATE * BASE_FACTOR);
                        else
                            weightRightHand = 1;
                    }
                    else
                    {
                        if (weightRightHand > 0.01f)
                            weightRightHand = Mathf.Lerp(weightRightHand, 0, BASE_RATE);
                        else
                            weightRightHand = 0.0f;
                    }

                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, weightRightHand);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, weightRightHand);
                    animator.SetIKPosition(AvatarIKGoal.RightHand, RightHandObj.position);
                    animator.SetIKRotation(AvatarIKGoal.RightHand, RightHandObj.rotation);

                    animator.SetInteger("intensityOfRightHand", intensityOfRightHand);
                }

                if (LeftHandObj != null && VRTK_DeviceFinder.GetControllerLeftHand())
                {
                    if ((VRTK_DeviceFinder.GetControllerLeftHand().activeInHierarchy && isLeftHandIKEnabled)
                        || isIKModeEnabled)
                    {
                        if (weightLeftHand < 0.99f)
                            weightLeftHand = Mathf.Lerp(weightLeftHand, 1.0f, BASE_RATE * BASE_FACTOR);
                        else
                            weightLeftHand = 1.0f;
                    }
                    else
                    {
                        if (weightLeftHand > 0.01f)
                            weightLeftHand = Mathf.Lerp(weightLeftHand, 0, BASE_RATE);
                        else
                            weightLeftHand = 0;
                    }

                    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, weightLeftHand);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, weightLeftHand);
                    animator.SetIKPosition(AvatarIKGoal.LeftHand, LeftHandObj.position);
                    animator.SetIKRotation(AvatarIKGoal.LeftHand, LeftHandObj.rotation);

                    animator.SetInteger("intensityOfLeftHand", intensityOfLeftHand);
                }
            }
        }
    }
}
