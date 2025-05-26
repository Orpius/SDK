namespace Sample_AspNetCore_ProtobufNet
{
	public class ApplicationState
	{
		public static string ServerUrl { get; set; } = "https://localhost:32774";

		// Credentials are located on the Operation page
		// inside the Orpius client application.
		public static Guid ExternalId { get; set; }
		public static Guid AccessKey  { get; set; }
	}
}
