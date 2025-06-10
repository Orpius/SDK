using Orpius.Platform.Tooling.RpcToolsRegistrationService;
using Orpius.Platform.Tooling.ToolRegistration;

namespace Sample_AspNetCore_ProtobufNet.RpcServiceModel
{
	public class RegistrationHeaderHandler : DelegatingHandler
	{
		readonly IToolRegistrationParameters parameters;

		public RegistrationHeaderHandler(IToolRegistrationParameters parameters)
		{
			this.parameters = parameters
							  ?? throw new ArgumentNullException(nameof(parameters));
		}

		protected override async Task<HttpResponseMessage> SendAsync(
			HttpRequestMessage request,
			CancellationToken cancellationToken)
		{
			Guid apiKey = await parameters.GetApiKeyAsync();

			request.Headers.Add(ToolsRegistrationHeaders.ExternalId, parameters.ExternalId.ToString());
			request.Headers.Add(ToolsRegistrationHeaders.AccessKey, apiKey.ToString());

			try
			{
				return await base.SendAsync(request, cancellationToken);
			}
			finally
			{
				request.Headers.Clear();
			}
		}
	}
}