
using UnityEngine;
using Unity.Profiling;

namespace TutanDev.Logging
{
    public class LoggerClient : MonoBehaviour
    {
		static readonly ProfilerMarker k_marker = new ProfilerMarker("CustomMarker");

        delegate object LogIteration();

        private LogIteration _logIteration;

		void Update()
        {
            k_marker.Begin();
            for (int i = 0; i < 3; i++)
            {
                _logIteration = () => $"iteration number: {i}";

				Logger.LogInfo(_logIteration);
            }
            k_marker.End();
        }
    }
}
