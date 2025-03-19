using System;
using Unity.Profiling;
using UnityEngine;

namespace TutanDev.Json
{
	public class TestClient : MonoBehaviour
	{
		static readonly ProfilerMarker k_marker = new ProfilerMarker("Deserialize NEwtonsoft");

		NewtonsoftDeserializer serializer;
		RoomCanvas room;



		private void Start()
		{
			serializer = new();
		}

		private void Update()
		{
			k_marker.Begin();
			room = serializer.Deserialize3<RoomCanvas>(JsonString.Text);
			k_marker.End();
		}
	}
}
