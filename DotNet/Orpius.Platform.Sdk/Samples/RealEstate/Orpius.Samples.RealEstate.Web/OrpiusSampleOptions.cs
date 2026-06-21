namespace Orpius.Samples
{
	public class OrpiusSampleOptions
	{
		public const string SectionName = "OrpiusSample";

		public Uri? OrpiusServerUrl { get; set; }

		public OperationCredentialOptions Operations { get; set; } = new();

		public ToolRegistrationCredentialOptions ToolsRegistration { get; set; } = new();

		public void Validate()
		{
			ValidateUri(
				OrpiusServerUrl,
				$"{SectionName}:{nameof(OrpiusServerUrl)}");

			Operations.Validate($"{SectionName}:{nameof(Operations)}");

			ToolsRegistration.Validate($"{SectionName}:{nameof(ToolsRegistration)}");
		}

		static void ValidateUri(Uri? uri, string settingName)
		{
			if (uri == null)
			{
				throw new InvalidOperationException(
					$"The '{settingName}' setting is required.");
			}

			if (!uri.IsAbsoluteUri)
			{
				throw new InvalidOperationException(
					$"The '{settingName}' setting must be an absolute URI.");
			}
		}
	}

	public class ToolRegistrationCredentialOptions
	{
		public Guid ExternalId { get; set; }

		public Guid AccessKey { get; set; }

		public Uri? IncomingUrl { get; set; }

		public void Validate(string sectionName)
		{
			ValidateGuid(ExternalId, $"{sectionName}:{nameof(ExternalId)}");
			ValidateGuid(AccessKey,  $"{sectionName}:{nameof(AccessKey)}");
			ValidateUri(IncomingUrl, $"{sectionName}:{nameof(IncomingUrl)}");
		}

		static void ValidateGuid(Guid value, string settingName)
		{
			if (value == Guid.Empty)
			{
				throw new InvalidOperationException(
					$"The '{settingName}' setting is required.");
			}
		}

		static void ValidateUri(Uri? uri, string settingName)
		{
			if (uri == null)
			{
				throw new InvalidOperationException(
					$"The '{settingName}' setting is required.");
			}

			if (!uri.IsAbsoluteUri)
			{
				throw new InvalidOperationException(
					$"The '{settingName}' setting must be an absolute URI.");
			}
		}
	}

	public class OperationCredentialOptions
	{
		public Guid ExternalId { get; set; }

		public Guid ApiKey { get; set; }

		public void Validate(string sectionName)
		{
			ValidateGuid(ExternalId, $"{sectionName}:{nameof(ExternalId)}");

			ValidateGuid(ApiKey, $"{sectionName}:{nameof(ApiKey)}");
		}

		static void ValidateGuid(Guid value, string settingName)
		{
			if (value == Guid.Empty)
			{
				throw new InvalidOperationException(
					$"The '{settingName}' setting is required.");
			}
		}
	}

	public class SampleOptionsRetriever
	{
		public static OrpiusSampleOptions GetOptions(IConfiguration configuration)
		{
			IConfigurationSection section = configuration.GetRequiredSection(
				OrpiusSampleOptions.SectionName);

			OrpiusSampleOptions options
				= section.Get<OrpiusSampleOptions>()
				  ?? throw new InvalidOperationException(
					  $"The '{OrpiusSampleOptions.SectionName}' configuration section is missing.");

			options.Validate();

			return options;
		}
	}
}