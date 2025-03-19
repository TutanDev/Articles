
using System.Diagnostics;

namespace TutanDev.ConditionalLogging
{
	internal static class ConditionalLogger
    {
		const string INFO =		"DEBUG_INFO";
		const string WARNING =  "DEBUG_WARNING";
		const string ERROR =	"DEBUG_ERROR";

		[Conditional(INFO)]
		public static void LogInfo(object message)
		{
			UnityEngine.Debug.Log(message);
		}

		[Conditional(WARNING)]
		public static void LogWarning(object message)
		{
			UnityEngine.Debug.LogWarning(message);
		}

		[Conditional(ERROR)]
		public static void LogError(object message)
		{
			UnityEngine.Debug.LogError(message);
		}
	}
}
