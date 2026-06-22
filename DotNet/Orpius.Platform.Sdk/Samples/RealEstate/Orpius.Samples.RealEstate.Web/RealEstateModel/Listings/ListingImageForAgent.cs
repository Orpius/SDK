using System.Text.Json;

namespace Orpius.Samples.RealEstate
{
	public static class ListingImageForAgent
	{
		const long maxImageBytes = 5 * 1024 * 1024;

		public static async Task<string?> CreateJsonAsync(
			IFormFile? imageFile,
			CancellationToken token)
		{
			if (imageFile is null || imageFile.Length == 0)
			{
				return null;
			}

			if (imageFile.Length > maxImageBytes)
			{
				throw new InvalidOperationException(
					"The selected image is too large. Please select an image smaller than 5 MB.");
			}

			if (!imageFile.ContentType.StartsWith(
					"image/",
					StringComparison.OrdinalIgnoreCase))
			{
				throw new InvalidOperationException(
					"The selected file must be an image.");
			}

			await using Stream stream = imageFile.OpenReadStream();

			using MemoryStream memoryStream = new();

			await stream.CopyToAsync(memoryStream, token);

			string base64 = Convert.ToBase64String(memoryStream.ToArray());

			AgentVisibleListingImage image = new()
			{
				FileName    = imageFile.FileName,
				ContentType = imageFile.ContentType,
				DataUri     = $"data:{imageFile.ContentType};base64,{base64}"
			};

			AgentVisibleListingImageContainer container = new()
			{
				ListingImage = image
			};

			JsonSerializerOptions options = new(JsonSerializerDefaults.Web)
			{
				WriteIndented = false
			};

			return JsonSerializer.Serialize(container, options);
		}
	}

	public class AgentVisibleListingImageContainer
	{
		public required AgentVisibleListingImage ListingImage { get; set; }
	}

	public class AgentVisibleListingImage
	{
		public required string FileName { get; set; }

		public required string ContentType { get; set; }

		public required string DataUri { get; set; }
	}
}