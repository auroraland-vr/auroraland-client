using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using AOT;


namespace TencentMobileGaming
{
	public class QAVRoom : ITMGRoom
	{
		#region Interface

		public override string GetQualityTips ()
		{
			return Marshal.PtrToStringAnsi(QAVNative.QAVSDK_AVRoom_GetQualityTips (mNativeObj));
		}

		[MonoPInvokeCallback(typeof(QAVCallback))]
		private static void s_ChangeRoomtypeCallback(int result, string error_info)
		{
			Debug.Log("s_ChangeRoomtypeCallback, result=" + result + "err:" + error_info);
			if (QAVContext.GetInstance().GetRoomInner().OnChangeRoomtypeCallback != null) {
				QAVContext.GetInstance().GetRoomInner().OnChangeRoomtypeCallback(result, error_info);
			}
		}

		public override event QAVCallback OnChangeRoomtypeCallback;

		public override int ChangeRoomType(ITMGRoomType roomType)
		{
			
			return QAVNative.QAVSDK_AVRoom_ChangeRoomType(mNativeObj, (int)roomType, QAVRoom.s_ChangeRoomtypeCallback);
		}

		public override int GetRoomType()
		{
			int roomtype = QAVNative.QAVSDK_AVRoom_GetRoomType (mNativeObj);
			if (roomtype > 0 && roomtype < 4) {
				return  roomtype;
			} else
			{
				return 0;
			}
		}
		
        public override int ChangeGameAudioMode(int audioMode)
		{
			return QAVNative.QAVSDK_AVRoom_ChangeGameAudioMode(mNativeObj, audioMode);
        }

        public override int UpdateAudioRecvRange(int range)
        {
            return QAVNative.QAVSDK_AVRoom_UpdateAudioRecvRange(mNativeObj,range);
        }

        public override int UpdateSelfPosition(int[] position, float[] axisForward, float[] axisRight, float[] axisUp)
        {
            return QAVNative.QAVSDK_AVRoom_UpdateSelfPosition(mNativeObj, position, axisForward, axisRight, axisUp, 3);
        }
		#endregion
		
		#region Constructor
		public QAVRoom()
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

