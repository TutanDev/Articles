
using Unity.Profiling;
using UnityEngine;

namespace TutanDev.ConditionalLogging
{
	public class ConditionalLoggerClient : MonoBehaviour
    {
		static readonly ProfilerMarker s_Marker = new ProfilerMarker("ConditionalLoggerClient.Update");

		[SerializeField] int _logCountPerFrame = 100;

		void Update()
		{
			using (s_Marker.Auto())
			{
				for (int i = 0; i < _logCountPerFrame; i++)
				{
					ConditionalLogger.LogInfo($"Test Log from {name} [{i}]");
				}
			}
		}
	}
}
