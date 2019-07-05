
using System;
using System.Text;
using System.Runtime.InteropServices;

namespace TencentMobileGaming
{
	public class QAVSig
	{
		public QAVSig ()
		{
		}

		public static string GenSig(int appId, string identifier, string privateKey)
		{
			StringBuilder authBuff = new StringBuilder(1024);
			for (int i = 0; i < 1024; i++) {
				authBuff.Append ("\0");
			}

			int ret = QAVSig.QAVSDK_SIG_GenSig(appId, identifier, privateKey, authBuff, 1024);
			if (ret != 0){
				return null;
			}else{
				return authBuff.ToString();
			}
		}

		#if UNITY_IPHONE && !UNITY_EDITOR
		public const string MyLibName = "__Internal";
		#else
		public const string MyLibName = "qav_tlssig";
        #endif

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX
		private static int QAVSDK_SIG_GenSig(int appId, [MarshalAs(UnmanagedType.LPStr)] string account, [MarshalAs(UnmanagedType.LPStr)] string privateKey, StringBuilder retSigBuff, int buffLenght)
		{
			return QAVError.OK; 
		}
#else
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int QAVSDK_SIG_GenSig(int appId,[MarshalAs(UnmanagedType.LPStr)] string account,[MarshalAs(UnmanagedType.LPStr)] string privateKey,StringBuilder retSigBuff,int buffLenght);
		#endif
	}
}

