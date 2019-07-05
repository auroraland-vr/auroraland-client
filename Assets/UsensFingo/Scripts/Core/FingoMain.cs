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
    /// FingoMain is used to get or set all kinds of the data from Fingo Devices.
    /// </summary>
    public class FingoMain : Singleton <FingoMain>
    {
        [SerializeField]
        private bool enableHandTracking = true;
        [SerializeField]
        private bool enableControllerTracking = false;
        [SerializeField]
        private bool enableGestureDetection = true;
        [SerializeField]
        private bool enableInfraredImage = true;
        [SerializeField]
        private bool enableColorImage = false;
        [SerializeField]
        private bool enableHeadTracking = false;

        void Awake()
        {
            FingoManager.Instance.Initialize(this, enableHandTracking, enableControllerTracking, enableGestureDetection,
                enableInfraredImage, enableColorImage, enableHeadTracking);
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Test the capability of Fingo devices.
        /// </summary>
        /// <param name="deviceCapability"> The device capability. </param>
        /// <returns> Capable of the devices. </returns>
        public bool TestDevicesCapability(FingoCapability deviceCapability)
        {
            return FingoManager.Instance.TestCapability(deviceCapability);
        }

        /// <summary>
        /// Get the enable of Fingo devices.
        /// </summary>
        /// <param name="deviceCapabilty"> The capability of Fingo devices to test. </param>
        /// <returns> Enable or disable of this capability. </returns>
        public bool GetDevicesEnable(FingoCapability deviceCapabilty)
        {
            switch(deviceCapabilty)
            {
                case FingoCapability.Hand:
                    return TestDevicesCapability(FingoCapability.Hand) && enableHandTracking;
                case FingoCapability.Controller:
                    return TestDevicesCapability(FingoCapability.Controller) && enableControllerTracking;
                case FingoCapability.Gesture:
                    return TestDevicesCapability(FingoCapability.Gesture) && enableGestureDetection;
                case FingoCapability.InfraredImage:
                    return TestDevicesCapability(FingoCapability.InfraredImage) && enableInfraredImage;
                case FingoCapability.RGBImage:
                    return TestDevicesCapability(FingoCapability.RGBImage) && enableColorImage;
                case FingoCapability.Marker:
                    return TestDevicesCapability(FingoCapability.Marker) && enableHeadTracking;
                case FingoCapability.Slam:
                    return TestDevicesCapability(FingoCapability.Slam) && enableHeadTracking;
            }
            return false;
        }

        #region hand
        /// <summary>
        /// Set the hand tracking function enable or disable.
        /// </summary>
        /// <param name="enable"> Enable or disable hand tracking. </param>
        /// <returns> Setting hand tracking enable or disable success. </returns>
        public bool SetHandTrackingEnable(bool enable)
        {
            if (FingoManager.Instance.SetHandTrackingEnable(enable))
            {
                enableHandTracking = enable;
                return true;
            }
            return false;
        }

        public bool SetControllerTrackingEnable(bool enable)
        {
            if(FingoManager.Instance.SetControllerTrackingEnable(enable))
            {
                enableControllerTracking = enable;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Set the gesture detection enable or disable.
        /// </summary>
        /// <param name="enable"> Enable or disable gesture detection or not. </param>
        /// <returns> Setting gesture detection enable or disable success. </returns>
        public bool SetGestureDetectionEnable(bool enable)
        {
            if (FingoManager.Instance.SetGestureDetectionEnable(enable))
            {
                enableGestureDetection = enable;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Set the gesture detection mode.
        /// </summary>
        /// <param name="detectionMode"> The target mode of gesture detection. </param>
        /// <returns> Setting gesture detection mode success. </returns>
        public bool SetGestureDetectionMode(GestureDetectionMode detectionMode)
        {
            return FingoManager.Instance.SetGestureDetectionMode(detectionMode);
        }

        /// <summary>
        /// Get the gesture detection mode.
        /// </summary>
        /// <returns> The gesture detection mode using right now. </returns>
        public GestureDetectionMode GetGestureDetectionMode()
        {
            return FingoManager.Instance.GetGestureDetectionMode();
        }

        /// <summary>
        /// Get the hand raw data corresponding to the particular hand type currently tracked
        /// </summary>
        /// <param name="handtype"> Hand type of the hand. </param>
        /// <returns> The hand raw data of this hand. </returns>
        public HandRawData GetRawHandData(HandType handType)
        {
            return FingoManager.Instance.GetRawHandData(handType);
        }

        /// <summary>
        /// Set enable or disable time warping to hand data.
        /// </summary>
        /// <param name="enableTimeWarping"> Enable or disable time warping. </param>
        public void SetHandTimeWarping (bool enableTimeWarping)
        {
            FingoManager.Instance.EnableHandTimeWarping(enableTimeWarping);
        }

        /// <summary>
        /// Set enable or disable stabilizer to hand data.
        /// </summary>
        /// <param name="enableStabilizer"> Enable or disable stabilizer. </param>
        public void SetHandStabilizer (bool enableStabilizer)
        {
            FingoManager.Instance.EnableHandStabilizer(enableStabilizer);
        }



#if LOG_TIME_PROFILING_FPS && !UNITY_EDITOR

        /// <summary>
        /// return the msg fps from the FingoReceivehandler.cs
        /// </summary>
        /// <returns></returns>
        public float GetMsgFPS()
        {

            MsgFPS = FingoManager.Instance.GetMsgFPS();
            if (MsgFPS != 0) return MsgFPS;
            return 0;
        }

        /// <summary>
        /// return the Service delay from the FingoReceivehandler.cs
        /// </summary>
        /// <returns></returns>
        public ulong GetServiceDelay()
        {
            ServiceDelay = FingoManager.Instance.GetServiceDelay();
            //Debug.Log("FingoMain Render Delay " + ServiceDelay);
            if (ServiceDelay != 0) return ServiceDelay;
            return 0;
        }


        /// <summary>
        /// return the render delay from the FingoReceivehandler.cs
        /// </summary>
        /// <returns></returns>
        public ulong GetZMQDelay()
        {
            ZMQDelay = FingoManager.Instance.GetZMQDelay();
            //Debug.Log("FingoMain Render Delay " + ZMQDelay);
            if (ZMQDelay != 0) return ZMQDelay;
            return 0;
        }

        /// <summary>
        /// return the render delay from the FingoReceivehandler.cs
        /// </summary>
        /// <returns></returns>
        public ulong GetServiceTimeStamp()
        {
            ServiceTimeStamp = FingoManager.Instance.GetServiceTimeStamp();

            if (ServiceTimeStamp != 0) return ServiceTimeStamp;
            return 0;
        }
#endif

        /// <summary>
        /// Get the hand data corresponding to the particular hand type currently tracked
        /// </summary>
        /// <param name="handtype"> Hand type of the hand. </param>
        /// <returns> The hand data of this hand. </returns>
        public Hand GetHand(HandType handtype)
        {
            return FingoManager.Instance.GetHand(handtype);
        }
        #endregion hand

        #region head
        /// <summary>
        /// Set the head tracking function enable or disable.
        /// </summary>
        /// <param name="enable"> Enable or disable head tracking. </param>
        /// <returns> Setting hand tracking enable or disable success. </returns>
        public bool SetHeadTrackingEnable(bool enable)
        {
            if (FingoManager.Instance.SetHeadTrackingEnable(enable))
            {
                enableHeadTracking = enable;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Get the Head instance currently tracked
        /// </summary>
        /// <returns> The head data from Fingo. </returns>
        public Head GetHead(bool enablePrediction = false)
        {
            return FingoManager.Instance.GetHead(enablePrediction);
        }

        /// <summary>
        /// Reset the positional tracking.
        /// </summary>
        public void ResetHeadTracking()
        {
            FingoManager.Instance.ResetHeadTracking();
            FingoManager.Instance.InitHeadTracking();
        }
        #endregion head

        #region image
        /// <summary>
        /// Set infrared image enable or disable.
        /// </summary>
        /// <param name="enable"> Enable or disable get infrared image. </param>
        /// <returns> Setting infrared image enable or disable success. </returns>
        public bool SetIRImageEnable(bool enable)
        {
            if(FingoManager.Instance.SetIRImageEnable(enable))
            {
                enableInfraredImage = enable;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Get infrared image data corresponding to the particular eye type currently tracked
        /// </summary>
        /// <param name="eye"> The eye type of the infrared image. </param>
        /// <returns> The infrared image of the eye. </returns>
        public Image GetInfraredImage(EyeType eye)
        {
            return FingoManager.Instance.GetInfraredImage(eye);
        }

        /// <summary>
        /// Set color image enable or disable.
        /// </summary>
        /// <param name="enable"> Enable or disable get color image. </param>
        /// <returns> Setting color image enable or disable success. </returns>
        public bool SetRGBImageEnable(bool enable)
        {
            if (FingoManager.Instance.SetRGBImageEnable(enable))
            {
                enableColorImage = enable;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Get infrared image data corresponding to the particular eye type currently tracked
        /// </summary>
        /// <param name="eye"> The eye type of the color image. </param>
        /// <returns> The color image of the eye. </returns>
        public Image GetRGBImage(EyeType eye)
        {
            return FingoManager.Instance.GetRGBImage(eye);
        }
        #endregion image

        void OnApplicationQuit()
        {
            FingoManager.Instance.ReleaseAllDevices();
        }
    }
}
