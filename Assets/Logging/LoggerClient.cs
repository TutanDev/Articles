
using UnityEngine;

namespace TutanDev.Logging
{
    public class LoggerClient : MonoBehaviour
    {
        [SerializeField] int _logCountPerFrame = 10;

        void Update()
        {
            for (int i = 0; i < _logCountPerFrame; i++)
            {
                Logger.LogInfo(() => $"Test Log from {name} [{i}]");
            }
        }
    }
}
