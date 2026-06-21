using System.Text.Json;
using System.Text.Json.Serialization;

namespace Orpius.Samples.RealEstate.Pages.Shared
{
	public static class OperationMessageStreamWriter
	{
		static readonly JsonSerializerOptions jsonSerializerOptions = new(JsonSerializerDefaults.Web)
		{
			Converters =
			{
				new JsonStringEnumConverter()
			}
		};

		public static async Task PrepareResponseAsync(
			HttpResponse response,
			CancellationToken token)
		{
			response.ContentType          = "application/x-ndjson";
			response.Headers.CacheControl = "no-cache";

			await response.StartAsync(token);
		}

		public static async Task WriteAsync(
			HttpResponse response,
			OperationMessageView message,
			CancellationToken token)
		{
			await JsonSerializer.SerializeAsync(
				response.Body,
				message,
				jsonSerializerOptions,
				token);

			await response.WriteAsync("\n", token);
			await response.Body.FlushAsync(token);
		}
	}
}
