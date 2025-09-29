namespace Sample_AspNetCore_ProtobufNet
{
	public class ApplicationState
	{
		public static string OrpiusServerUrl { get; set; } = "https://stack1.app.orpius.com";

		public static class OperationsSettings
		{
			// Credentials are located on the Operation page
			// inside the Orpius client application.
			public static Guid ExternalId { get; set; } = Guid.Parse("65c7df12-9fdd-70d9-f584-b087a74cca3b");
			public static Guid ApiKey  { get; set; } = Guid.Parse("c110d87e-fd7c-592c-7514-987324867e07");
		}

		public static class ToolsRegistrationSettings
		{
			// Credentials for tool registrations are located in the Agent Tools tab
			// inside the Orpius client application.
			public static Guid ExternalId { get; set; } = Guid.Parse("02d72aa0-dfa1-4ae9-a505-c48eefb39b12");
			public static Guid ApiKey  { get; set; } = Guid.Parse("56b014bd-0d42-42f0-8177-ec3df719cc7d");
		}
	}
}
