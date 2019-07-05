using System.Collections;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Auroraland{
	public class DebugLogger{
		public static void Log(LogType entryType, string message)
		{
			var logMessage = GetLogMessage(entryType, message);
			Debug.LogFormat("[{0} {1}]:{2}", DateTime.Now.ToShortDateString(),  DateTime.Now.ToLongTimeString(), logMessage);
		}

		public static void Log(object message)
		{
			Debug.LogFormat("[{0} {1}]:{2}", DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString(), message);
		}
		public static void LogFormat(string message, params object[] args)
		{
			var logMessage = String.Format (message, args);
			logMessage = String.Format ("[{0} {1}]:{2}", DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString(),logMessage);
			Debug.Log (logMessage);
		}

		public static void LogErrorFormat(string message, params object[] args)
		{
			var logMessage = String.Format (message, args);
			logMessage = String.Format ("[{0} {1}]:{2}", DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString(),logMessage);
			Debug.LogError (logMessage);
		}

		public static void LogError(object message)
		{
			var logMessage = String.Format ("[{0} {1}]:{2}", DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString(), GetLogMessage(LogType.Exception, message.ToString()));
			Debug.LogError (logMessage);
		}


		public static void LogError(object message, UnityEngine.Object context)
		{
			var logMessage = String.Format ("[{0} {1}]:{2}", DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString(), GetLogMessage(LogType.Exception, message.ToString()));

			Debug.LogError (logMessage, context);
		}

		public static void LogException(Exception ex)
		{
			var logMessage = GetLogMessage(LogType.Exception, ex.ToString());
			Debug.LogErrorFormat("[{0} {1}]:{2}", DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString(), logMessage);
		}

		public static void LogException(Exception ex, UnityEngine.Object context)
		{
			var logMessage = GetLogMessage(LogType.Exception, ex.ToString());
			Debug.LogErrorFormat("[{0} {1}]:{2}", DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString(), logMessage);
			Debug.LogException (ex, context);
		}

		private static string GetLogMessage(LogType entryType, string message)
		{
			var messageBuilder = new StringBuilder();
			messageBuilder.Append(Enum.GetName(typeof(LogType), entryType));
			messageBuilder.Append(" ");
			messageBuilder.Append(message);
			return messageBuilder.ToString();
		}
	}
}
