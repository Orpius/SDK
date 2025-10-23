using System;
using System.Collections.Generic;
using Orpius.Platform.Tooling.RpcToolsRegistrationService;

namespace Orpius.Platform.Tooling.ToolRegistration
{
	public class FuncRegistrationParameters : IToolRegistrationParameters
	{
		readonly Func<Uri> getLocalUrl;
		readonly Func<Guid> getExternalId;
		readonly Func<Guid> getApiKey;

		public FuncRegistrationParameters(Func<Uri> getLocalUrl,
										  Func<Guid> getExternalId,
										  Func<Guid> getApiKey)
		{
			this.getLocalUrl = getLocalUrl
							   ?? throw new ArgumentNullException(nameof(getLocalUrl));
			this.getExternalId = getExternalId
								 ?? throw new ArgumentNullException(nameof(getExternalId));
			this.getApiKey = getApiKey
							 ?? throw new ArgumentNullException(nameof(getApiKey));
		}

		public Guid ApiKey     => getApiKey();
		public Uri  LocalUrl   => getLocalUrl();
		public Guid ToolsetExternalId => getExternalId();

		public IList<HeaderMessage> CallBackHeaders { get; set; } 
			= new List<HeaderMessage>();
	}
}