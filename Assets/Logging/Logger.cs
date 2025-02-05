
using System;

namespace TutanDev.Logging
{
    internal enum LogLevel { Info, Error }
    
	internal static class Logger 
    {
		static LogLevel _level = LogLevel.Error;

  //      public static void LogInfo(object message)
  //      {
		//	if (_level > LogLevel.Info) return;
		//	UnityEngine.Debug.Log(message);
  //      }

		//public static void LogError(object message)
		//{
		//	if (_level > LogLevel.Error) return;
		//	UnityEngine.Debug.LogError(message);
		//}

		public static void LogInfo(Func<object> message)
		{
			if (_level > LogLevel.Info) return;
			UnityEngine.Debug.Log(message());
		}

		public static void LogError(Func<object> message)
		{
			if (_level > LogLevel.Error) return;
			UnityEngine.Debug.LogError(message());
		}
	}
}
