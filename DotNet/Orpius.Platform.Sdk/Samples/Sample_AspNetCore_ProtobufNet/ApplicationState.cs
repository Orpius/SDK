namespace Sample_AspNetCore_ProtobufNet
{
	public class ApplicationState
	{
		public static string ServerUrl { get; set; } = "https://localhost:32774";

		// Credentials are located on the Operation page
		// inside the Orpius client application.
		public static Guid ExternalId { get; set; } = Guid.Parse("f3842aba-4757-17d9-9a37-d9b918e33579");
		public static Guid AccessKey  { get; set; } = Guid.Parse("6bce02a0-eeb2-64f0-c17a-df23dcce0379");
	}
}
