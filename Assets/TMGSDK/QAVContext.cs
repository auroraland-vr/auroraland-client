using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using AOT;


namespace TencentMobileGaming
{
	public class QAVContext : ITMGContext
	{
		#region Interface
		public static new QAVContext GetInstance()
		{
			if (sInstance == null) {
				lock (sLock) {
					if (sInstance == null) {
						sInstance = new QAVContext ();
						sInstance.mNativeObj = QAVNative.QAVSDK_AVContext_GetInstance();
					}
				}
			}
			return sInstance;
		}

        ~QAVContext()
        {
            //Debug.LogFormat("~QAVContext");

            QAVNative.QAVSDK_AVContext_Destroy(mNativeObj);
        }

        public override int Poll()
        {
            return QAVNative.QAVSDK_Poll();
        }

        public override int Pause()
        {
            return GetAudioCtrl().PauseAudio();
        }

        public override int Resume()
        {
            return GetAudioCtrl().ResumeAudio();
        }

		public override int Init(string sdkAppID, string openID)
        {
			if (QAVNative.QAVSDK_AVContext_IsContextStarted(mNativeObj)
				&& string.Equals(mSdkAppID, sdkAppID) && string.Equals(mOpenID, openID)) {
				return QAVError.OK;
			}

			int ret = QAVNative.QAVSDK_AVContext_Stop(mNativeObj);
			if (ret == QAVError.OK) {
				mSdkAppID = sdkAppID;
				mOpenID = openID;
				QAVNative.QAVSDK_AVContext_Start(mNativeObj, mSdkAppID, mOpenID, s_OnStartContextCallback);
				QAVPTT.GetInstance().SetAppInfo(sdkAppID, openID);
				return QAVError.OK;
			} else {
				return ret;
			}
		}

        public override int Uninit()
        {
            mSdkAppID = "";
            mOpenID = "";
            return QAVNative.QAVSDK_AVContext_Stop(mNativeObj);
        }

		public override string GetSDKVersion()
		{
			return Marshal.PtrToStringAnsi(QAVNative.QAVSDK_GetSDKVersion());
		}
		
		public override void SetAppVersion(string sAppVersion)
		{
			QAVNative.QAVSDK_SetAppVersion(sAppVersion);
		}

        public override void SetLogLevel(int logLevel, bool enableWrite, bool enablePrint)
        {
            QAVNative.QAVSDK_AVContext_SetLogLevel(mNativeObj, logLevel, enableWrite, enablePrint);
        }

        public override void SetLogPath(string logDir)
        {
            QAVNative.QAVSDK_AVContext_SetLogPath(mNativeObj, logDir);
        }

        public override int SetRecvMixStreamCount(int nCount)
        {
            if(QAVNative.QAVSDK_AVContext_IsContextStarted(mNativeObj)){
                return QAVError.ERR_FAIL;
            }
            return QAVNative.QAVSDK_AVContext_SetRecvMixStreamCount(mNativeObj, nCount);
        }
        public override int SetTestEnv(bool test)
        {
            return QAVNative.QAVSDK_AVContext_SetTestEnv(mNativeObj, test);
        }

        public override bool IsRoomEntered()
		{
			return QAVNative.QAVSDK_AVContext_IsRoomEntered(mNativeObj);
		}
		
		[MonoPInvokeCallback(typeof(QAVCallback))]
		private static void s_OnStartContextCallback(int result, string error_info)
		{
			Debug.Log(string.Format("s_OnStartContextCallback {0}, {1}", result, error_info));
		}

		public override int EnterRoom(string roomID, ITMGRoomType roomtype, byte[] authBuffer)
        {
			return QAVNative.QAVSDK_AVContext_EnterRoom(mNativeObj, roomID, authBuffer, authBuffer.Length, (int)roomtype, 0, 0,
				QAVContext.s_OnEnterRoomComplete, QAVContext.s_OnExitRoomComplete, QAVContext.s_OnRoomDisconnect, QAVContext.s_OnEndpointsUpdateInfo,
				QAVContext.s_QAVOnRoomTypeChangedEvent, QAVContext.s_OnDeviceStateChangedEvent);
        }

		public override int EnterTeamRoom(string roomID, ITMGRoomType roomtype, byte[] authBuffer,int teamId, int audioMode)
		{
			return QAVNative.QAVSDK_AVContext_EnterRoom(mNativeObj, roomID, authBuffer, authBuffer.Length, (int)roomtype, teamId, audioMode,
				QAVContext.s_OnEnterRoomComplete, QAVContext.s_OnExitRoomComplete, QAVContext.s_OnRoomDisconnect, QAVContext.s_OnEndpointsUpdateInfo,
				QAVContext.s_QAVOnRoomTypeChangedEvent, QAVContext.s_OnDeviceStateChangedEvent);
		}
		
		public override int ExitRoom()
		{
			return QAVNative.QAVSDK_AVContext_ExitRoom(mNativeObj);
		}

		public override ITMGRoom GetRoom()
		{
			return this.GetRoomInner();
		}

		public QAVRoom GetRoomInner()
		{
			if (null == mAVRoom) {
				mAVRoom = new QAVRoom ();
			}
			mAVRoom.Init(QAVNative.QAVSDK_AVContext_GetRoom(mNativeObj));
			return mAVRoom;
		}

		public override ITMGAudioCtrl GetAudioCtrl()
		{
			return this.GetAudioCtrlInner();
		}
		public QAVAudioCtrl GetAudioCtrlInner()
		{
			if (null == mAVAudioCtrl) {
				mAVAudioCtrl = new QAVAudioCtrl ();
			}
			mAVAudioCtrl.Init(QAVNative.QAVSDK_AVContext_GetAudioCtrl(mNativeObj));
			return mAVAudioCtrl;
		}


        public override ITMGAudioEffectCtrl GetAudioEffectCtrl()
		{
			return this.GetAudioEffectCtrlInner();
        }
        public QAVAudioEffectCtrl GetAudioEffectCtrlInner()
        {
            if (null == mAVAudioEffectCtrl)
            {
                mAVAudioEffectCtrl = new QAVAudioEffectCtrl();
            }
            mAVAudioEffectCtrl.Init(QAVNative.QAVSDK_AVContext_GetAudioCtrl(mNativeObj));
            return mAVAudioEffectCtrl;
        }

        public override ITMGPTT GetPttCtrl()
		{
			return QAVPTT.GetInstance();
		}

		public override event QAVEnterRoomComplete OnEnterRoomCompleteEvent;
		public override event QAVExitRoomComplete OnExitRoomCompleteEvent;
		public override event QAVRoomDisconnect OnRoomDisconnectEvent;
		public override event QAVEndpointsUpdateInfo OnEndpointsUpdateInfoEvent;		// 成员状态变化，EventID详见EVENT_ID_ENDPOINT_XXX
		public override event QAVOnRoomTypeChangedEvent OnRoomTypeChangedEvent;
		#endregion
		
		#region Implement
		[MonoPInvokeCallback(typeof(QAVEnterRoomComplete))]
		private static void s_OnEnterRoomComplete(int result, string error_info)
		{
			Debug.LogFormat("s_OnEnterRoomComplete:code:{0},Msg:{1},{2}",result,error_info,QAVContext.GetInstance().OnEnterRoomCompleteEvent);
			if (QAVContext.GetInstance().OnEnterRoomCompleteEvent != null) {
				QAVContext.GetInstance().OnEnterRoomCompleteEvent(result, error_info);
			}
		}
		
		[MonoPInvokeCallback(typeof(QAVExitRoomComplete))]
		private static void s_OnExitRoomComplete()
		{
			Debug.LogFormat("s_OnExitRoomComplete");
			if (QAVContext.GetInstance().OnExitRoomCompleteEvent != null) {
				QAVContext.GetInstance().OnExitRoomCompleteEvent();
			}
		}
		
		[MonoPInvokeCallback(typeof(QAVRoomDisconnect))]
		private static void s_OnRoomDisconnect(int result, string error_info)
		{
			Debug.LogFormat("s_OnRoomDisconnect");
			if (QAVContext.GetInstance().OnRoomDisconnectEvent != null) {
				QAVContext.GetInstance().OnRoomDisconnectEvent(result, error_info);
			}
		}
		
		[MonoPInvokeCallback(typeof(QAVEndpointsUpdateInfo))]
		private static void s_OnEndpointsUpdateInfo(int eventID, int count, string[] openIdList)
		{
			if (QAVContext.GetInstance().OnEndpointsUpdateInfoEvent != null) {
				QAVContext.GetInstance().OnEndpointsUpdateInfoEvent(eventID, count, openIdList);
			}
		}

		[MonoPInvokeCallback(typeof(QAVOnDeviceStateChangedEvent))]
		private static void s_OnDeviceStateChangedEvent(int deviceType, string deviceId, bool openOrClose)
		{
			QAVContext.GetInstance ().GetAudioCtrlInner ().OnDeviceStateChanged (deviceType, deviceId, openOrClose);
		}

		[MonoPInvokeCallback(typeof(QAVOnRoomTypeChangedEvent))]
		private static void s_QAVOnRoomTypeChangedEvent(int roomtype)
		{
			Debug.LogFormat("s_QAVOnRoomTypeChangedEvent:coderoomtype{0}", roomtype);
			if (QAVContext.GetInstance ().OnRoomTypeChangedEvent != null) {
				QAVContext.GetInstance ().OnRoomTypeChangedEvent (roomtype);
			}
		}



		private QAVRoom mAVRoom;
		private QAVAudioCtrl mAVAudioCtrl;
		private QAVAudioEffectCtrl mAVAudioEffectCtrl;

#endregion
		
#region Constructor
		
		private QAVContext()
		{
			mNativeObj = IntPtr.Zero;
		}

		private static QAVContext sInstance;
		private static readonly System.Object sLock = new System.Object ();
		private IntPtr mNativeObj;
        
        private string mSdkAppID = "";
        public string mOpenID = "";

		#endregion
	}
}
