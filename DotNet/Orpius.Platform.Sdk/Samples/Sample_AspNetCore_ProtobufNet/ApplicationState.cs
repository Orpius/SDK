namespace Sample_AspNetCore_ProtobufNet
{
	public class ApplicationState
	{
		public static string OrpiusServerUrl { get; set; }
			= "https://localhost:32774";//"https://nyfqg7nhtfak2vj6kjslml4lrk.app.orpius.com";

		public static class OperationsSettings
		{
			// Credentials are located on the Operation page
			// inside the Orpius client application.
			public static Guid ExternalId { get; set; } = Guid.Parse(
				"f3842aba-4757-17d9-9a37-d9b918e33579"
				//"40402bf7-53d0-6e51-df31-5725d843ef19"
				);
			public static Guid ApiKey  { get; set; } = Guid.Parse(
				"6bce02a0-eeb2-64f0-c17a-df23dcce0379"
				//"af314bb7-8e92-6dd2-8dfa-c489dd178ead"
				);
		}

		public static class ToolsRegistrationSettings
		{
			// Credentials for tool registrations are located in the Agent Tools tab
			// inside the Orpius client application.
			public static Guid   ExternalId  { get; set; } = Guid.Parse(
				"ee2b90ff-a4c6-44bf-93a7-a25b7e3271b0"
				//"25639ffa-6184-465f-8f53-d68f944acaf3"
				);
			public static Guid   AccessKey      { get; set; } = Guid.Parse(
				"72e1b1f1-414b-46d9-bcb1-1a736d7e6027"
				//"8e5558f2-f813-4f3f-8a6e-cda7b94a7ee4"
				);

			public static Uri IncomingUrl { get; set; }
				= new("https://alpine-remarkable-grown-possible.trycloudflare.com");
		}
	}
}
