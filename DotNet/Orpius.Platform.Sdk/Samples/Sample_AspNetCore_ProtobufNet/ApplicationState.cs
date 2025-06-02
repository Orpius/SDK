namespace Sample_AspNetCore_ProtobufNet
{
	public class ApplicationState
	{
		public static string OrpiusServerUrl { get; set; } = "https://localhost:32774";

		public static class OperationsSettings
		{
			// Credentials are located on the Operation page
			// inside the Orpius client application.
			public static Guid ExternalId { get; set; } = Guid.Parse("f3842aba-4757-17d9-9a37-d9b918e33579");
			public static Guid AccessKey  { get; set; } = Guid.Parse("6bce02a0-eeb2-64f0-c17a-df23dcce0379");
		}

		public static class ToolsRegistrationSettings
		{
			// Credentials for tool registrations are located in the Agent Tools tab
			// inside the Orpius client application.
			public static Guid ExternalId { get; set; } = Guid.Parse("ee2b90ff-a4c6-44bf-93a7-a25b7e3271b0");
			public static Guid AccessKey  { get; set; } = Guid.Parse("72e1b1f1-414b-46d9-bcb1-1a736d7e6027");
		}

		//public static string LocalUrl { get; set; }
	}
}
