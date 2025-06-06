using System;

namespace Orpius.Platform.Text.Json
{
	public interface IJsonSerializer
	{
		/// <exception cref="ArgumentNullException">
		/// Thrown if the specified json is <c>null</c>.</exception>
		/// <exception cref="JsonSerializationException">Throw if inner serializer fails.</exception>
		string Serialize<T>(T item);

		/// <exception cref="ArgumentNullException">
		/// Thrown if the specified json is <c>null</c>.</exception>
		/// <exception cref="JsonSerializationException">Throw if inner serializer fails.</exception>
		T Deserialize<T>(string json);
	}
}