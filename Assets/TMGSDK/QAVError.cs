using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AOT;


namespace TencentMobileGaming
{
	public class QAVError
	{
		#region Interface
		public static int OK = 0x0;
		
		public static int ERR_FAIL = 0x1; //DEC 1 一般错误
		
		public static int ERR_REPETITIVE_OPERATION = 0x3E9;     //DEC 1001 重复操作。已经在进行某种操作，再次去做同样的操作，则返回这个错误
		public static int ERR_EXCLUSIVE_OPERATION = 0x3EA;      //DEC 1002 互斥操作。已经在进行某种操作，再次去做同类型的其他操作，则返回这个错误
		public static int ERR_HAS_IN_THE_STATE = 0x3EB;         //DEC 1003 已经处于所要状态，无需再操作。如设备已经打开，再次去打开，就返回这个错误码
		public static int ERR_INVALID_ARGUMENT = 0x3EC;         //DEC 1004 错误参数
		public static int ERR_TIMEOUT = 0x3ED;                  //DEC 1005 操作超时
		public static int ERR_NOT_IMPLEMENTED = 0x3EE;          //DEC 1006 功能未实现
		public static int ERR_NOT_IN_MAIN_THREAD = 0x3EF;       //DEC 1007 不在主线程中执行操作
		
		public static int ERR_CONTEXT_NOT_START = 0x44D;    //DEC 1101 AVContext没有启动
		public static int ERR_ROOM_NOT_EXIST = 0x4B1;       //DEC 1201 房间不存在
		
		public static int ERR_DEVICE_NOT_EXIST = 0x515; //DEC 1301 设备不存在

		public static int ERR_SERVER_FAILED = 0x2711;                        //DEC 10001 服务器内部错误
		public static int ERR_SERVER_NO_PERMISSION = 0x2713;                 //DEC 10003 没有权限
		public static int ERR_SERVER_REQUEST_ROOM_ADDRESS_FAIL = 0x2714;     //DEC 10004 进房间获取房间地址失败
		public static int ERR_SERVER_CONNECT_ROOM_FAIL = 0x2715;             //DEC 10005 进房间连接房间失败
		public static int ERR_SERVER_FREE_FLOW_AUTH_FAIL = 0x2716;           //DEC 10006 免流情况下，免流签名校验失败，导致进房获取房间地址失败
        public static int ERR_SERVER_ROOM_DISSOLVED = 0x2717;                //DEC 10007 游戏应用房间超过90分钟，强制下线
		
		
		public static int ERR_IMSDK_UNKNOWN = 0x1B57;           //DEC 6999  IMSDK内部错误
		public static int ERR_IMSDK_TIMEOUT = 0x1B58;           //DEC 7000  IMSDK内部错误
        public static int ERR_HTTP_REQ_FAIL = 0x1B59;           //DEC 7001  Http请求错误
		public static int ERR_UNKNOWN = 0x10000;                //DEC 65536 IMSDK内部错误


		public static int ERR_ACC_OPENFILE_FAILED = 0x7D1;         			//DEC 4001 伴奏打开文件失败
		public static int ERR_ACC_FILE_FORAMT_NOTSUPPORT = 0x7D2;           //DEC 4002 伴奏不支持的文件格式
		public static int ERR_ACC_DECODER_FAILED = 0x7D3;     			 	//DEC 4003 伴奏解码失败
		public static int ERR_ACC_BAD_PARAM = 0x7D4;         				//DEC 4004 伴奏参数错误
		public static int ERR_ACC_MEMORY_ALLOC_FAILED = 0x7D5;       		//DEC 4005 伴奏内存分配失败
		public static int ERR_ACC_CREATE_THREAD_FAILED = 0x7D6; 			//DEC 4006 伴奏创建线程失败
		public static int ERR_ACC_NOT_STARTED = 0x7D7;    					//DEC 4007 伴奏未启动

		public static int ERR_VOICE_RECORD_PARAM_NULL = 0x1001;         //DEC 4097  param null
		public static int ERR_VOICE_RECORD_INIT_ERR = 0x1002;           //DEC 4098 Init Error
		public static int ERR_VOICE_RECORD_RECORDING_ERR = 0x1003;      //DEC 4099 now is recording, can't do other operator
		public static int ERR_VOICE_RECORD_NODATA_ERR = 0x1004;         //DEC 4100 nodata
		public static int ERR_VOICE_RECORD_OPENFILE_ERR = 0x1005;       //DEC 4101 open a file err
		public static int ERR_VOICE_RECORD_PERMISSION_MIC_ERR = 0x1006; //DEC 4102 you have not right to access micphone in android
		public static int ERR_VOICE_RECORD_AUDIO_TOO_SHORT = 0x1007;    //DEC 4103 less than 1000 ms
		
		public static int ERR_VOICE_UPLOAD_FILE_ACCESSERROR = 0x2001;       //DEC 8193 Read File Failed
		public static int ERR_VOICE_UPLOAD_SIGN_CHECK_FAIL = 0x2002;        //DEC 8194 Sign Check Failed maybe network or appid mismatch
		public static int ERR_VOICE_UPLOAD_NETWORK_FAIL = 0x2003;           //DEC 8195 Network return error, watch log
		public static int ERR_VOICE_UPLOAD_GET_SIGN_NETWORK_FAIL = 0x2004;  //DEC 8196 connect server failed
		public static int ERR_VOICE_UPLOAD_GET_SIGN_RSP_NULL = 0x2005;      //DEC 8197 the response data is null
		public static int ERR_VOICE_UPLOAD_GET_SIGN_RSP_INVALID = 0x2006;   //DEC 8198 decode response data failed
		public static int ERR_VOICE_UPLOAD_SIGN_CHECK_EXPIRED = 0x2007;     //DEC 8199 TLS sign is out of date
		public static int ERR_VOICE_UPLOAD_APPINFO_UNSET = 0x2008;          //DEC 8200 app info has not set
		
		public static int ERR_VOICE_DOWNLOAD_FILE_ACCESSERROR = 0x3001;         //DEC 12289 Write File Failed
		public static int ERR_VOICE_DOWNLOAD_SIGN_CHECK_FAIL = 0x3002;          //DEC 12290 Sign Check Failed maybe network or appid mismatch
		public static int ERR_VOICE_DOWNLOAD_NETWORK_FAIL = 0x3003;             //DEC 12291 Network return error, watch log
		public static int ERR_VOICE_DOWNLOAD_REMOTEFILE_ACCESSERROR = 0x3004;   //DEC 12292 Remote File Not Exist
		public static int ERR_VOICE_DOWNLOAD_GET_SIGN_NETWORK_FAIL = 0x3005;    //DEC 12293 connect server failed
		public static int ERR_VOICE_DOWNLOAD_GET_SIGN_RSP_NULL = 0x3006;        //DEC 12294 the response data is null
		public static int ERR_VOICE_DOWNLOAD_GET_SIGN_RSP_INVALID = 0x3007;     //DEC 12295 decode response data failed
		public static int ERR_VOICE_DOWNLOAD_SIGN_CHECK_EXPIRED = 0x3008;       //DEC 12296 TLS sign is out of date
		public static int ERR_VOICE_DOWNLOAD_APPINFO_UNSET = 0x3009;            //DEC 12297 app info has not set
		
		public static int ERR_VOICE_PLAYER_INIT_ERR = 0x5001;           //DEC 20481 Init Error
		public static int ERR_VOICE_PLAYER_PLAYING_ERR = 0x5002;        //DEC 20482 now is playing, can't do other operator
		public static int ERR_VOICE_PLAYER_PARAM_NULL = 0x5003;         //DEC 20483 param null
		public static int ERR_VOICE_PLAYER_OPENFILE_ERR = 0x5004;       //DEC 20484 open a file err

		public static int ERR_VOICE_S2T_INTERNAL_ERROR = 0x8001;        // DEC 32769 internal error
		public static int ERR_VOICE_S2T_NETWORK_FAIL = 0x8002;          // DEC 32770 connect server failed
		public static int ERR_VOICE_S2T_RSP_DATA_NULL = 0x8003;         // DEC 32771 the response data is null
		public static int ERR_VOICE_S2T_RSP_DATA_DECODE_FAIL = 0x8004;  // DEC 32772 decode response data failed
		public static int ERR_VOICE_S2T_SIGN_CHECK_EXPIRED = 0x8005;    // DEC 32773 TLS sign is out of date
		public static int ERR_VOICE_S2T_APPINFO_UNSET = 0x8006;         // DEC 32774 app info has not set
		
		#endregion
	}
}