using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using AOT;

namespace TencentMobileGaming
{
    #region Interface
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void QAVCallback(int result, string error_info);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void QAVEnterRoomComplete(int result, string error_info);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void QAVExitRoomComplete();
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void QAVRoomDisconnect(int result, string error_info);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void QAVEndpointsUpdateInfo(int eventID, int count, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)]string[] openIdList);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void QAVOnRoomTypeChangedEvent(int roomtype);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void QAVOnDeviceStateChangedEvent(int deviceType, string deviceId, bool openOrClose);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void QAVAudioCallback(bool enabled, int result, string error_info);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void QAVAudioRouteChangeCallback(int code);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void QAVAccompanyFileCompleteHandler(int code, bool isfinished, string filepath);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void QAVRecordFileCompleteCallback(int code, string filepath);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void QAVUploadFileCompleteCallback(int code, string filepath, string fileid);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void QAVDownloadFileCompleteCallback(int code, string filepath, string fileid);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void QAVPlayFileCompleteCallback(int code, string filepath);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void QAVSpeechToTextCallback(int code, string fileid, string result);


	#endregion
}

