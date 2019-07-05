using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;



namespace TencentMobileGaming
{
	public class QAVNative
	{
		static QAVNative ()
        {
			
		}
        
		#region DllImport
		
		#if UNITY_IPHONE && !UNITY_EDITOR
		public const string MyLibName = "__Internal";
        #else
        public const string MyLibName = "gmesdk";
        #endif
        #endregion

        #region NativeEntry
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_Poll();
		
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr QAVSDK_GetSDKVersion();
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void QAVSDK_SetAppVersion([MarshalAs(UnmanagedType.LPStr)] string sAppVersion);
		
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVContext_SetTestEnv(IntPtr appchannel, bool test);
		
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr QAVSDK_AVContext_GetInstance();

        [DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void QAVSDK_AVContext_SetLogLevel(IntPtr avcontext, int logLevel, bool enableWrite, bool enablePrint);
        [DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void QAVSDK_AVContext_SetLogPath(IntPtr avcontext, [MarshalAs(UnmanagedType.LPStr)] string logDir);

        [DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern bool QAVSDK_AVContext_IsContextStarted(IntPtr avcontext);
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVContext_Start(IntPtr avcontext, [MarshalAs(UnmanagedType.LPStr)] string sdkAppId,
		                                                 [MarshalAs(UnmanagedType.LPStr)] string openId, QAVCallback callback);
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVContext_Stop(IntPtr avcontext);
        [DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int QAVSDK_AVContext_Destroy(IntPtr avcontext);

        [DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern bool QAVSDK_AVContext_IsRoomEntered(IntPtr avcontext);
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVContext_EnterRoom(IntPtr avcontext, [MarshalAs(UnmanagedType.LPStr)] string roomID, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] byte[] authBuffer, int authBufferLen,
			int roomtype, int teamId, int audioMode,
			QAVEnterRoomComplete enterRoomComplete, QAVExitRoomComplete exitRoomComplete, QAVRoomDisconnect roomDisconnect, QAVEndpointsUpdateInfo endpointsUpdateInfo, 
			QAVOnRoomTypeChangedEvent onRoomtypeChangeEvent, QAVOnDeviceStateChangedEvent onDeviceStateChangedEvent);
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVContext_ExitRoom(IntPtr avcontext);

		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr QAVSDK_AVContext_GetRoom(IntPtr avcontext);
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr QAVSDK_AVRoom_GetQualityTips(IntPtr avroom);
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVRoom_ChangeRoomType(IntPtr avroom, int roomtype, QAVCallback callback);
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVRoom_GetRoomType(IntPtr avroom);

        [DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int QAVSDK_AVRoom_ChangeGameAudioMode(IntPtr avroom, int audioMode);
        [DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int QAVSDK_AVRoom_UpdateAudioRecvRange(IntPtr avroom, int range);

        [DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int QAVSDK_AVRoom_UpdateSelfPosition(IntPtr avroom,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 5)] int[] position,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 5)] float[] axisForward, 
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 5)] float[] axisRight, 
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 5)] float[] axisUp, int len);

		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr QAVSDK_AVContext_GetAudioCtrl(IntPtr avcontext);

        [DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int QAVSDK_AVContext_SetRecvMixStreamCount(IntPtr context, int nMixCount);

		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_SetVoiceType(IntPtr context, int voiceType);
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_EnableAudioCaptureDevice(IntPtr avaudioctrl, bool enabled);
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_EnableAudioPlayDevice(IntPtr avaudioctrl, bool enabled);
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_IsAudioCaptureDeviceEnabled(IntPtr avaudioctrl);
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_IsAudioPlayDeviceEnabled(IntPtr avaudioctrl);
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_EnableAudioSend(IntPtr avaudioctrl, bool enabled, QAVAudioCallback callback);
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_EnableAudioRecv(IntPtr avaudioctrl, bool enabled, QAVAudioCallback callback);
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_IsAudioSendEnabled(IntPtr avaudioctrl);
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_IsAudioRecvEnabled(IntPtr avaudioctrl);
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_PauseAudio(IntPtr avaudioctrl);
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_ResumeAudio(IntPtr avaudioctrl);
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_GetMicLevel(IntPtr avaudioctrl);
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_SetMicVolume(IntPtr avaudioctrl, int volume);
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_GetMicVolume(IntPtr avaudioctrl);
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_GetSpeakerLevel(IntPtr avaudioctrl);
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_SetSpeakerVolume(IntPtr avaudioctrl, int volume);
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_GetSpeakerVolume(IntPtr avaudioctrl);
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_SetAudioRouteChangeCallback(IntPtr avaudioctrl, QAVAudioRouteChangeCallback callback);
        [DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int QAVSDK_AVAudioCtrl_GetVolumeByUin(IntPtr avaudioctrl, [MarshalAs(UnmanagedType.LPStr)] string openId);

        [DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int QAVSDK_AVAudioCtrl_EnableLoopBack(IntPtr avaudioctrl, bool enabled);

        [DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int QAVSDK_AVAudioCtrl_AddAudioBlackList(IntPtr avaudioctrl, [MarshalAs(UnmanagedType.LPStr)] string openId);
        [DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_RemoveAudioBlackList(IntPtr avaudioctrl, [MarshalAs(UnmanagedType.LPStr)] string openId);

		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_InitSpatializer(IntPtr avaudioctrl);
        [DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int QAVSDK_AVAudioCtrl_EnableSpatializer(IntPtr avaudioctrl, bool enabled, bool applyTeam);
        [DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool QAVSDK_AVAudioCtrl_IsEnableSpatializer(IntPtr avaudioctrl);

        [DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_StartAccompany(IntPtr avaudioctrl, string filePath, bool loopBack, int loopCount, int duckerTimeMs, QAVAccompanyFileCompleteHandler callback);
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_StopAccompany(IntPtr avaudioctrl, int duckerTimeMs);
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern bool QAVSDK_AVAudioCtrl_IsAccompanyPlayEnd(IntPtr avaudioctrl);
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_PauseAccompany(IntPtr avaudioctrl);
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_ResumeAccompany(IntPtr avaudioctrl);
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_EnableAccompanyPlay(IntPtr avaudioctrl, bool enable);
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_EnableAccompanyLoopBack(IntPtr avaudioctrl, bool enable);
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_SetAccompanyVolume(IntPtr avaudioctrl, int vol);
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_GetAccompanyVolume(IntPtr avaudioctrl);
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern uint QAVSDK_AVAudioCtrl_GetAccompanyFileTotalTimeByMs(IntPtr avaudioctrl);
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_SetAccompanyFileCurrentPlayedTimeByMs(IntPtr avaudioctrl, uint timeMs);
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern uint QAVSDK_AVAudioCtrl_GetAccompanyFileCurrentPlayedTimeByMs(IntPtr avaudioctrl);

		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr QAVSDK_PTT_GetInstance();
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_PTT_SetAppInfo(IntPtr avptt, [MarshalAs(UnmanagedType.LPStr)] string appid, 
		                                               [MarshalAs(UnmanagedType.LPStr)] string openid);
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_PTT_SetMaxMessageLength(IntPtr avptt, int msTime);
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_PTT_StartRecording(IntPtr avptt, [MarshalAs(UnmanagedType.LPStr)] string filePath, QAVRecordFileCompleteCallback callback);
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int QAVSDK_PTT_GetRecordingLevel(IntPtr avptt);
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_PTT_StopRecording(IntPtr avptt);
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_PTT_CancelRecording(IntPtr avptt);

		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_PTT_ApplyPTTAuthbuffer(IntPtr avptt, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] authBuffer, int authBufferLen);

		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_PTT_UploadRecordedFile(IntPtr avptt, [MarshalAs(UnmanagedType.LPStr)] string filePath, QAVUploadFileCompleteCallback callback);
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_PTT_DownloadRecordedFile(IntPtr avptt, [MarshalAs(UnmanagedType.LPStr)] string fileID, [MarshalAs(UnmanagedType.LPStr)] string filePath, QAVDownloadFileCompleteCallback callback);
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_PTT_StartPlayFile(IntPtr avptt, [MarshalAs(UnmanagedType.LPStr)] string filePath, QAVPlayFileCompleteCallback callback);
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int QAVSDK_PTT_GetPlayingLevel(IntPtr avptt);
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_PTT_StopPlayFile(IntPtr avptt);
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_PTT_GetFileSize(IntPtr avptt, [MarshalAs(UnmanagedType.LPStr)] string filePath);
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_PTT_GetVoiceFileDuration(IntPtr avptt, [MarshalAs(UnmanagedType.LPStr)] string filePath);
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_PTT_SpeechToText(IntPtr avptt, [MarshalAs(UnmanagedType.LPStr)] string fileID, [MarshalAs(UnmanagedType.LPStr)] string language, [MarshalAs(UnmanagedType.LPStr)] string translateLanguage, QAVSpeechToTextCallback callback);
        #endregion
    }
}

/*
 * Platform Defines
The platform defines that Unity supports for your scripts are:
Property: Function: 
UNITY_EDITOR Define for calling Unity Editor scripts from your game code. 
UNITY_EDITOR_WIN Platform define for editor code on Windows. 
UNITY_EDITOR_OSX Platform define for editor code on Mac OSX. 
UNITY_STANDALONE_OSX Platform define for compiling/executing code specifically for Mac OS (This includes Universal, PPC and Intel architectures). 
UNITY_DASHBOARD_WIDGET Platform define when creating code for Mac OS dashboard widgets. 
UNITY_STANDALONE_WIN Use this when you want to compile/execute code for Windows stand alone applications. 
UNITY_STANDALONE_LINUX Use this when you want to compile/execute code for Linux stand alone applications. 
UNITY_STANDALONE Use this to compile/execute code for any standalone platform (Mac, Windows or Linux). 
UNITY_WEBPLAYER Platform define for web player content (this includes Windows and Mac Web player executables). 
UNITY_WII Platform define for compiling/executing code for the Wii console. 
UNITY_IPHONE Platform define for compiling/executing code for the iPhone platform. 
UNITY_ANDROID Platform define for the Android platform. 
UNITY_PS3 Platform define for running PlayStation 3 code. 
UNITY_XBOX360 Platform define for executing Xbox 360 code. 
UNITY_FLASH Platform define when compiling code for Adobe Flash. 
UNITY_BLACKBERRY Platform define for a Blackberry10 device. 
UNITY_WP8 Platform define for Windows Phone 8. 
UNITY_METRO Platform define for Windows Store Apps (additionally NETFX_CORE is defined when compiling C# files against .NET Core). 
UNITY_WINRT Equivalent to UNITY_WP8 |UNITY_METRO 
Also you can compile code selectively depending on the version of the engine you are working on. Currently the supported ones are:

UNITY_2_6 Platform define for the major version of Unity 2.6. 
UNITY_2_6_1 Platform define for specific version 2.6.1. 
UNITY_3_0 Platform define for the major version of Unity 3.0. 
UNITY_3_0_0 Platform define for specific version 3.0.0. 
UNITY_3_1 Platform define for major version of Unity 3.1. 
UNITY_3_2 Platform define for major version of Unity 3.2. 
UNITY_3_3 Platform define for major version of Unity 3.3. 
UNITY_3_4 Platform define for major version of Unity 3.4. 
UNITY_3_5 Platform define for major version of Unity 3.5. 
UNITY_4_0 Platform define for major version of Unity 4.0. 
UNITY_4_0_1 Platform define for specific version 4.0.1. 
UNITY_4_1 Platform define for major version of Unity 4.1. 
UNITY_4_2 Platform define for major version of Unity 4.2. 
UNITY_4_3 Platform define for major version of Unity 4.3. 
UNITY_4_5 Platform define for major version of Unity 4.5. 
Note: For versions before 2.6.0 there are no platform defines as this feature was first introduced in that version.
*/
