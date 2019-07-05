using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AOT;

namespace TencentMobileGaming
{
	public class QAVAudioEffectCtrl : ITMGAudioEffectCtrl
	{
		#region Interface
		[MonoPInvokeCallback(typeof(QAVAccompanyFileCompleteHandler))]
		private static void s_OnAccompanyComplete(int code,bool isfinished, string filepath)
		{
			if (QAVContext.GetInstance().GetAudioEffectCtrlInner().OnAccompanyFileCompleteHandler != null)
			{
				QAVContext.GetInstance().GetAudioEffectCtrlInner().OnAccompanyFileCompleteHandler(code, isfinished,filepath);
			}
		}

		public override event QAVAccompanyFileCompleteHandler OnAccompanyFileCompleteHandler;

		public override int StartAccompany(string filePath, bool loopBack, int loopCount, int duckerTimeMs)
		{
			return QAVNative.QAVSDK_AVAudioCtrl_StartAccompany(mNativeObj, filePath, loopBack, loopCount, duckerTimeMs, s_OnAccompanyComplete);
		}
		
		public override int StopAccompany(int duckerTimeMs)
		{
			return QAVNative.QAVSDK_AVAudioCtrl_StopAccompany(mNativeObj, duckerTimeMs);
		}

		public override bool IsAccompanyPlayEnd()
		{
			return QAVNative.QAVSDK_AVAudioCtrl_IsAccompanyPlayEnd(mNativeObj);
		}
		
		public override int PauseAccompany()
		{
			return QAVNative.QAVSDK_AVAudioCtrl_PauseAccompany(mNativeObj);
		}
		
		public override int ResumeAccompany()
		{
			return QAVNative.QAVSDK_AVAudioCtrl_ResumeAccompany(mNativeObj);
		}

		public override int EnableAccompanyPlay(bool enable)
		{
			return QAVNative.QAVSDK_AVAudioCtrl_EnableAccompanyPlay(mNativeObj, enable);
		}

		public override int EnableAccompanyLoopBack(bool enable)
		{
			return QAVNative.QAVSDK_AVAudioCtrl_EnableAccompanyLoopBack(mNativeObj, enable);
		}

		public override int SetAccompanyVolume(int vol)
		{
			return QAVNative.QAVSDK_AVAudioCtrl_SetAccompanyVolume(mNativeObj, vol);
		}

		public override int GetAccompanyVolume()
		{
			return QAVNative.QAVSDK_AVAudioCtrl_GetAccompanyVolume(mNativeObj);
		}

		public override uint GetAccompanyFileTotalTimeByMs()
		{
			return QAVNative.QAVSDK_AVAudioCtrl_GetAccompanyFileTotalTimeByMs(mNativeObj);
		}
		
		public override uint GetAccompanyFileCurrentPlayedTimeByMs()
		{
			return QAVNative.QAVSDK_AVAudioCtrl_GetAccompanyFileCurrentPlayedTimeByMs(mNativeObj);
		}
		
		public override int SetAccompanyFileCurrentPlayedTimeByMs(uint timeMs)
		{
			return QAVNative.QAVSDK_AVAudioCtrl_SetAccompanyFileCurrentPlayedTimeByMs(mNativeObj, timeMs);
		}

		public override int GetEffectsVolume()
		{
			return 0;
		}

		public override int SetEffectsVolume(int volume)
		{
			return 0;
		}

		public override int PlayEffect(int soundId, string filePath, bool loop = false, double pitch = 1.0f, double pan = 0.0f, double gain = 1.0f)
		{
			return 0;
		}

		public override int PauseEffect(int soundId)
		{
			return 0;
		}

		public override int PauseAllEffects()
		{
			return 0;
		}

		public override int ResumeEffect(int soundId)
		{
			return 0;
		}

		public override int ResumeAllEffects()
		{
			return 0;
		}

		public override int StopEffect(int soundId)
		{
			return 0;
		}

		public override int StopAllEffects()
		{
			return 0;
		}

		public override int SetVoiceType (int voiceType)
		{
			return QAVNative.QAVSDK_AVAudioCtrl_SetVoiceType(mNativeObj, voiceType);
		}


		#endregion
		
		#region Constructor
		
		public QAVAudioEffectCtrl()
		{
			mNativeObj = IntPtr.Zero;
		}
		
		public void Init(IntPtr nativeObj)
		{
			mNativeObj = nativeObj;
		}
		
		public void Uninit()
		{
			mNativeObj = IntPtr.Zero;
		}
		
		private IntPtr mNativeObj;
		
		#endregion
	}
}
