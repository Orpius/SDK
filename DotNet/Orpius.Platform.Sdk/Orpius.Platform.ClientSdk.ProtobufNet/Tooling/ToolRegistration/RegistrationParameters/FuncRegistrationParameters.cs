using System;
using System.Threading.Tasks;

namespace Orpius.Platform.Tooling.ToolRegistration
{
	public class FuncRegistrationParameters : IToolRegistrationParameters
	{
		readonly Func<Uri> getLocalUrl;
		readonly Func<Guid> getExternalId;
		readonly Func<Task<Guid>> getApiKey;

		public FuncRegistrationParameters(Func<Uri> getLocalUrl,
										  Func<Guid> getExternalId,
										  Func<Task<Guid>> getAccessToken)
		{
			this.getLocalUrl = getLocalUrl
							   ?? throw new ArgumentNullException(nameof(getLocalUrl));
			this.getExternalId = getExternalId
								 ?? throw new ArgumentNullException(nameof(getExternalId));
			this.getApiKey = getAccessToken
								  ?? throw new ArgumentNullException(nameof(getAccessToken));
		}

		public Task<Guid> GetApiKeyAsync()
		{
			return getApiKey();
		}

		public Uri  LocalUrl   => getLocalUrl();
		public Guid ExternalId => getExternalId();
	}
}