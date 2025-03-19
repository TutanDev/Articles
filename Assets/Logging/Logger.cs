
using System;

namespace TutanDev.Logging
{
    internal enum LogLevel { Verbose, Info, Warning,  Error, FatalError }
    
	internal static class Logger 
    {
		static LogLevel _level = LogLevel.FatalError;

		public static void LogInfo(object message)
		{
			if (_level > LogLevel.Info) return;
			UnityEngine.Debug.Log(message);
		}

		public static void LogError(object message)
		{
			if (_level > LogLevel.Error) return;
			UnityEngine.Debug.LogError(message);
		}

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
