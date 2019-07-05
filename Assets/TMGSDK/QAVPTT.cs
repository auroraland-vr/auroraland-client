using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AOT;

namespace TencentMobileGaming
{
	public class QAVPTT : ITMGPTT
	{
		#region Interface
		public static new QAVPTT GetInstance()
		{
			if (sInstance == null) {
				lock (sLock) {
					if (sInstance == null) {
						sInstance = new QAVPTT();
						sInstance.mNativeObj = QAVNative.QAVSDK_PTT_GetInstance();
					}
				}
			}
			return sInstance;
		}

		public override event QAVRecordFileCompleteCallback OnRecordFileComplete;
		public override event QAVUploadFileCompleteCallback OnUploadFileComplete;
		public override event QAVDownloadFileCompleteCallback OnDownloadFileComplete;
		public override event QAVPlayFileCompleteCallback OnPlayFileComplete;
		public override event QAVSpeechToTextCallback OnSpeechToTextComplete;


		public override  int ApplyPTTAuthbuffer (byte[] authBuffer)
		{
			return QAVNative.QAVSDK_PTT_ApplyPTTAuthbuffer(mNativeObj, authBuffer, authBuffer.Length);
		}

		public override int SetMaxMessageLength(int msTime)
		{
			return QAVNative.QAVSDK_PTT_SetMaxMessageLength(mNativeObj, msTime);
		}
		
		public override int StartRecording(string filePath)
		{
			return QAVNative.QAVSDK_PTT_StartRecording(mNativeObj, filePath, s_OnRecordFileComplete);
		}

        public override int GetRecordingLevel()
        {
            return QAVNative.QAVSDK_PTT_GetRecordingLevel(mNativeObj);
        }

		public override int StopRecording()
		{
			return QAVNative.QAVSDK_PTT_StopRecording(mNativeObj);
		}

		public override int CancelRecording()
		{
			return QAVNative.QAVSDK_PTT_CancelRecording(mNativeObj);
		}

		public override int UploadRecordedFile(string filePath)
		{
			return QAVNative.QAVSDK_PTT_UploadRecordedFile(mNativeObj, filePath, s_OnUploadFileComplete);
		}

		public override int DownloadRecordedFile(string fileID, string downloadFilePath)
		{
			return QAVNative.QAVSDK_PTT_DownloadRecordedFile(mNativeObj, fileID, downloadFilePath, s_OnDownloadFileComplete);
		}

		public override int PlayRecordedFile(string filePath)
		{
			return QAVNative.QAVSDK_PTT_StartPlayFile(mNativeObj, filePath, s_OnPlayFileComplete);
		}

        public override int GetPlayingLevel()
        {
            return QAVNative.QAVSDK_PTT_GetPlayingLevel(mNativeObj);
        }
		
		public override int StopPlayFile()
		{
			return QAVNative.QAVSDK_PTT_StopPlayFile(mNativeObj);
		}
		
		public override int GetFileSize(string filePath)
		{
			return QAVNative.QAVSDK_PTT_GetFileSize(mNativeObj, filePath);
		}
		
		public override int GetVoiceFileDuration(string filePath)
		{
			return QAVNative.QAVSDK_PTT_GetVoiceFileDuration(mNativeObj, filePath);
		}
		
		public override int SpeechToText(string fileID)
		{
			return QAVNative.QAVSDK_PTT_SpeechToText(mNativeObj, fileID, "cmn-Hans-CN", "cmn-Hans-CN", s_OnSpeechToTextComplete);
		}
		public override int SpeechToText(string fileID, string language)
		{
			return QAVNative.QAVSDK_PTT_SpeechToText(mNativeObj, fileID, language, language, s_OnSpeechToTextComplete);
		}

		#endregion

		#region Implement
		public int SetAppInfo(string appID, string openID)
		{
			return QAVNative.QAVSDK_PTT_SetAppInfo(mNativeObj, appID, openID);
		}


		[MonoPInvokeCallback(typeof(QAVRecordFileCompleteCallback))]
		private static void s_OnRecordFileComplete(int code, string filepath)
		{
			if (QAVPTT.GetInstance().OnRecordFileComplete != null)
			{
				QAVPTT.GetInstance().OnRecordFileComplete(code, filepath);
			}
		}
		
		[MonoPInvokeCallback(typeof(QAVUploadFileCompleteCallback))]
		private static void s_OnUploadFileComplete(int code, string filepath, string fileid)
		{
			if (QAVPTT.GetInstance().OnUploadFileComplete != null)
			{
				QAVPTT.GetInstance().OnUploadFileComplete(code, filepath, fileid);
			}
		}
		
		[MonoPInvokeCallback(typeof(QAVDownloadFileCompleteCallback))]
		private static void s_OnDownloadFileComplete(int code, string filepath, string fileid)
		{
			if (QAVPTT.GetInstance().OnDownloadFileComplete != null)
			{
				QAVPTT.GetInstance().OnDownloadFileComplete(code, filepath, fileid);
			}
		}
		
		[MonoPInvokeCallback(typeof(QAVPlayFileCompleteCallback))]
		private static void s_OnPlayFileComplete(int code, string filepath)
		{
			if (QAVPTT.GetInstance().OnPlayFileComplete != null)
			{
				QAVPTT.GetInstance().OnPlayFileComplete(code, filepath);
			}
		}
		[MonoPInvokeCallback(typeof(QAVSpeechToTextCallback))]
		private static void s_OnSpeechToTextComplete(int code, string fileid, string result)
		{
			if (QAVPTT.GetInstance().OnSpeechToTextComplete != null)
			{
				QAVPTT.GetInstance().OnSpeechToTextComplete(code, fileid, result);
			}
		}
		
		#endregion
		
		#region Constructor
		
		public QAVPTT()
		{
			mNativeObj = IntPtr.Zero;
		}
		
		private static QAVPTT sInstance;
		private static readonly System.Object sLock = new System.Object();
		private IntPtr mNativeObj;
		#endregion
	}
}
