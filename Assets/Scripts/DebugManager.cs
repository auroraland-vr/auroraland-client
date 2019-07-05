using UnityEngine;

namespace Auroraland
{
    public class DebugManager : MonoBehaviour
    {
        public Camera SteamCamera;
        public Camera OculusCamera;
        public Camera NonVRCamera;
        public Camera DebugCamera;
        public Camera ThirdPersonCamera;
        private Camera currentCamera;
        private Camera firstPersonCamera;
        private readonly string currrentSDK;
        private IKController IKScript;

        // Use this for initialization
        void Awake()
        {
            ThirdPersonCamera.depth = -2;
        }

        private void OnEnable()
        {
            SDK_SetupMode.SwitchToNone += SwitchToDebugCamera;
            SDK_SetupMode.SwitchToNonVR += SwitchToNonVRCamera;
            SDK_SetupMode.SwitchToOculus += SwitchToOculusCamera;
            SDK_SetupMode.SwitchToSteamVR += SwitchToSteamVRCamera;

        }

        private void OnDisable()
        {
            SDK_SetupMode.SwitchToNone -= SwitchToDebugCamera;
            SDK_SetupMode.SwitchToNonVR -= SwitchToNonVRCamera;
            SDK_SetupMode.SwitchToOculus -= SwitchToOculusCamera;
            SDK_SetupMode.SwitchToSteamVR -= SwitchToSteamVRCamera;
        }

        public void SwitchCamera()
        {
            if (currentCamera == ThirdPersonCamera)
            {
                currentCamera = firstPersonCamera;
                if (firstPersonCamera == SteamCamera)
                {
                    SwitchToSteamVRCamera();
                }
                else if (firstPersonCamera == OculusCamera)
                {
                    SwitchToOculusCamera();
                }
                else if (firstPersonCamera == NonVRCamera)
                {
                    SwitchToNonVRCamera();
                }
                else
                {
                    SwitchToDebugCamera();
                }
            }
            else
            {
                SwitchToThirdPersonCamera();
            }
        }

        private void SwitchToThirdPersonCamera()
        {
            firstPersonCamera.depth = -2;
            ThirdPersonCamera.depth = 1;// show this camera
            currentCamera = ThirdPersonCamera;

        }

        private void SwitchToFirstPersonCamera()
        {
            currentCamera = firstPersonCamera;

            firstPersonCamera.depth = 1; // show this camera
            ThirdPersonCamera.depth = -2;

        }

        private void SwitchToSteamVRCamera()
        {
            firstPersonCamera = SteamCamera;
            if (currentCamera == ThirdPersonCamera)
            {
                return;
            }

            currentCamera = firstPersonCamera;
            SwitchToFirstPersonCamera();

            IKScript = GetComponentInChildren<IKController>();

            if (IKScript)
            {
                IKScript.enabled = true;
            }
        }

        private void SwitchToOculusCamera()
        {
            firstPersonCamera = OculusCamera;
            if (currentCamera == ThirdPersonCamera)
            {
                return;
            }

            currentCamera = firstPersonCamera;
            SwitchToFirstPersonCamera();

            IKScript = GetComponentInChildren<IKController>();

            if (IKScript)
            {
                IKScript.enabled = true;
            }
        }

        private void SwitchToDebugCamera()
        {
            firstPersonCamera = DebugCamera;
            if (currentCamera == ThirdPersonCamera)
            {
                return;
            }

            SwitchToFirstPersonCamera();
        }

        private void SwitchToNonVRCamera()
        {
            firstPersonCamera = NonVRCamera;
            if (currentCamera == ThirdPersonCamera)
            {
                return;
            }

            SwitchToFirstPersonCamera();
        }
    }
}