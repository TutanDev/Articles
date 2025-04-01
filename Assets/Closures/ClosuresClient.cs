using System;
using Unity.Profiling;
using UnityEngine;


namespace TutanDev.Closures
{
	public class ClosuresClient : MonoBehaviour
	{
		static readonly ProfilerMarker k_marker = new ProfilerMarker("CustomMarker");
		delegate object LogIteration();

		void Update()
		{
			LogIteration myDelegate = default;
			
			for (int i = 0; i < 3; i++)
			{
				myDelegate += () => $"iteration number: {i}";
			}

			foreach (LogIteration logIteration in myDelegate.GetInvocationList())
			{
				print(logIteration());
			}
		}
	}
}

