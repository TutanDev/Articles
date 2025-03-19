using System;
using System.Buffers;
using System.IO;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

namespace TutanDev.Json
{

	public class NewtonsoftDeserializer 
    {
		public T Deserialize<T>(string jsonText)
		{
			try
			{
				T result = JsonConvert.DeserializeObject<T>(jsonText);
				return result;
			}
			catch (Exception ex)
			{
				Debug.LogError($"Could not parse respondse {jsonText}. {ex.Message}");
				return default;
			}
		}

		public T Deserialize2<T>(string jsonText)
		{
			try
			{
				using (var stringReader = new StringReader(jsonText))
				using (var jsonReader = new JsonTextReader(stringReader))
				{
					return new JsonSerializer().Deserialize<T>(jsonReader);
				}
			}
			catch (Exception ex)
			{
				Debug.LogError($"Could not parse respondse {jsonText}. {ex.Message}");
				return default;
			}
		}

		public T Deserialize3<T>(string jsonText)
		{
			try
			{
				T result = JsonConvert.DeserializeObject<T>(jsonText, JsonString.OptimizedSettings);
				return result;
			}
			catch (Exception ex)
			{
				Debug.LogError($"Could not parse respondse {jsonText}. {ex.Message}");
				return default;
			}
		}

		public T Deserialize4<T>(string jsonText)
		{
			var pool = ArrayPool<char>.Shared;
			char[] buffer = pool.Rent(jsonText.Length);
			jsonText.CopyTo(0, buffer, 0, jsonText.Length);

			try
			{
				using (var stringReader = new StringReader(new string(buffer, 0, jsonText.Length)))
				using (var jsonReader = new JsonTextReader(stringReader))
				{
					return new JsonSerializer().Deserialize<T>(jsonReader);
				}
			}
			finally
			{
				pool.Return(buffer);
			}
		}
	}

	
}
