namespace Sample_AspNetCore_ProtobufNet
{
	public class ApplicationState
	{
		public static string OrpiusServerUrl { get; set; } = "https://stack1.app.orpius.com";

		public static class OperationsSettings
		{
			// Credentials are located on the Operation page
			// inside the Orpius client application.
			public static Guid ExternalId { get; set; } = Guid.Parse("40402bf7-53d0-6e51-df31-5725d843ef19");
			public static Guid ApiKey  { get; set; } = Guid.Parse("40402bf7-53d0-6e51-df31-5725d843ef19");
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
