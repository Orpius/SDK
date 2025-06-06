using System;

namespace Orpius.Platform.Text.Json
{
	public class JsonSerializationException : Exception
	{
		public JsonSerializationException(string message, Exception? innerException = null) 
			: base(message, innerException)
		{
		}
	}

	class JsonSerializer : IJsonSerializer
	{
		public string Serialize<T>(T item)
		{
			try
			{
				return System.Text.Json.JsonSerializer.Serialize(item);
			}
			catch (Exception ex)
			{
				throw new JsonSerializationException($"Unable to serialize '{typeof(T)}'.", ex);
			}
			
		}

		public T Deserialize<T>(string json)
		{
			T result;

			try
			{
				result = System.Text.Json.JsonSerializer.Deserialize<T>(json);
			}
			catch (Exception ex)
			{
				throw new JsonSerializationException($"Unable to deserialize to type '{typeof(T)}'.", ex);
			}
			
			return result ?? throw new JsonSerializationException("Deserializing JSON resulted in null.");
		}
	}
}