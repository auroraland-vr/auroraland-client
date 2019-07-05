using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AOT;

namespace TencentMobileGaming
{

	public class QAVAudioCtrl : ITMGAudioCtrl
	{
		public override int PauseAudio()
		{
			return QAVNative.QAVSDK_AVAudioCtrl_PauseAudio(mNativeObj);
		}
		public override int ResumeAudio()
		{
			return QAVNative.QAVSDK_AVAudioCtrl_ResumeAudio(mNativeObj);
		}

		public override int EnableAudioCaptureDevice(bool enabled)
		{
			return QAVNative.QAVSDK_AVAudioCtrl_EnableAudioCaptureDevice(mNativeObj, enabled);
		}
		public override int EnableAudioPlayDevice(bool enabled)
		{
			return QAVNative.QAVSDK_AVAudioCtrl_EnableAudioPlayDevice(mNativeObj, enabled);
		}
		public override bool IsAudioCaptureDeviceEnabled()
		{
			return QAVNative.QAVSDK_AVAudioCtrl_IsAudioCaptureDeviceEnabled(mNativeObj) != 0;
		}
		public override bool IsAudioPlayDeviceEnabled()
		{
			return QAVNative.QAVSDK_AVAudioCtrl_IsAudioPlayDeviceEnabled(mNativeObj) != 0;
		}
		public override int EnableAudioSend(bool isEnabled)
		{
			return QAVNative.QAVSDK_AVAudioCtrl_EnableAudioSend(mNativeObj, isEnabled, null);
		}
		public override int EnableAudioRecv(bool isEnabled)
		{
			return QAVNative.QAVSDK_AVAudioCtrl_EnableAudioRecv(mNativeObj, isEnabled, null);
		}
		public override bool IsAudioSendEnabled()
		{
			return QAVNative.QAVSDK_AVAudioCtrl_IsAudioSendEnabled(mNativeObj) != 0;
		}
		public override bool IsAudioRecvEnabled()
		{
			return QAVNative.QAVSDK_AVAudioCtrl_IsAudioRecvEnabled(mNativeObj) != 0;
		}
        public override int EnableMic(bool isEnabled)
        {
            int ret1 = EnableAudioCaptureDevice(isEnabled);
            int ret2 = EnableAudioSend(isEnabled);
            if (ret1 == QAVError.OK && ret2 == QAVError.OK)
            {
                return QAVError.OK;
            }
            else
            {
                if (ret1 != QAVError.OK)
                {
                    return ret1;
                }
                else
                {
                    return ret2;
                }
            }
        }

        public override int GetMicState()
        {
            return (IsAudioCaptureDeviceEnabled() && IsAudioSendEnabled()) ? 1 : 0;
        }

        public override int EnableSpeaker(bool isEnabled)
        {
            int ret1 = EnableAudioPlayDevice(isEnabled);
            int ret2 = EnableAudioRecv(isEnabled);
            if (ret1 == QAVError.OK && ret2 == QAVError.OK)
            {
                return QAVError.OK;
            }
            else
            {
                if (ret1 != QAVError.OK)
                {
                    return ret1;
                }
                else
                {
                    return ret2;
                }
            }
        }

        public override int GetSpeakerState()
        {
            return (IsAudioPlayDeviceEnabled() && IsAudioRecvEnabled()) ? 1 : 0;
        }

        public override int GetMicLevel()
		{
			return QAVNative.QAVSDK_AVAudioCtrl_GetMicLevel(mNativeObj);
		}
		public override int SetMicVolume(int volume)
		{
			return QAVNative.QAVSDK_AVAudioCtrl_SetMicVolume(mNativeObj, volume);
		}
		public override int GetMicVolume()
		{
			return QAVNative.QAVSDK_AVAudioCtrl_GetMicVolume(mNativeObj);
		}
		public override int GetSpeakerLevel()
		{
			return QAVNative.QAVSDK_AVAudioCtrl_GetSpeakerLevel(mNativeObj);
		}
		public override int SetSpeakerVolume(int volume)
		{
			return QAVNative.QAVSDK_AVAudioCtrl_SetSpeakerVolume(mNativeObj, volume);
		}
		public override int GetSpeakerVolume()
		{
			return QAVNative.QAVSDK_AVAudioCtrl_GetSpeakerVolume(mNativeObj);
		}

		public override int EnableLoopBack(bool enable)
		{
			return QAVNative.QAVSDK_AVAudioCtrl_EnableLoopBack(mNativeObj, enable);
		}

		public override event QAVAudioRouteChangeCallback OnAudioRouteChangeComplete;

		[MonoPInvokeCallback(typeof(QAVAudioRouteChangeCallback))]
		private static void s_OnAudioRouteChangeComplete(int code)
		{
			if (QAVContext.GetInstance().GetAudioCtrlInner().OnAudioRouteChangeComplete != null) {
				QAVContext.GetInstance().GetAudioCtrlInner().OnAudioRouteChangeComplete(code);
			}
		}

        // public override event QAVOnDeviceStateChangedEvent OnDeviceStateChangedEvent;

        public void OnDeviceStateChanged(int deviceType, string deviceId, bool openOrClose) {
// 			if (QAVContext.GetInstance().GetAudioCtrlInner().OnDeviceStateChangedEvent != null) {
// 				QAVContext.GetInstance().GetAudioCtrlInner().OnDeviceStateChangedEvent(deviceType, deviceId, openOrClose);
// 			}
		}

        public override int AddAudioBlackList(string openId)
        {
			return QAVNative.QAVSDK_AVAudioCtrl_AddAudioBlackList(mNativeObj, openId);
        }

        public override int RemoveAudioBlackList(string openId)
        {
			return QAVNative.QAVSDK_AVAudioCtrl_RemoveAudioBlackList(mNativeObj, openId);
        }

		public override int InitSpatializer()
		{
			return QAVNative.QAVSDK_AVAudioCtrl_InitSpatializer(mNativeObj);
		}

        public override int EnableSpatializer(bool enable, bool applyTeam)
        {
            return QAVNative.QAVSDK_AVAudioCtrl_EnableSpatializer(mNativeObj, enable, applyTeam);
        }

        public override bool IsEnableSpatializer()
        {
            return QAVNative.QAVSDK_AVAudioCtrl_IsEnableSpatializer(mNativeObj);
        }

        public QAVAudioCtrl()
		{
			mNativeObj = IntPtr.Zero;
		}
        ~QAVAudioCtrl()
		{
        }

		private static int s_TIMERID_TRACKING_VOLME = 8884321;
		public void Init(IntPtr nativeObj)
		{
			mNativeObj = nativeObj;
			QAVNative.QAVSDK_AVAudioCtrl_SetAudioRouteChangeCallback(mNativeObj, s_OnAudioRouteChangeComplete);
		}
		
		public void Uninit()
		{
			mNativeObj = IntPtr.Zero;
			QAVNative.QAVSDK_AVAudioCtrl_SetAudioRouteChangeCallback(mNativeObj, null);

		}
		
		private IntPtr mNativeObj;
		
	}
}
