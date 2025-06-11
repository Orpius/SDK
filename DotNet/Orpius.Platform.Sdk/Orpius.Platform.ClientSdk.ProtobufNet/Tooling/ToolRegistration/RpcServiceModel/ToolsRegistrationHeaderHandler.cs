using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

using Orpius.Platform.Tooling.RpcToolsRegistrationService;

namespace Orpius.Platform.Tooling.ToolRegistration
{
	//public class ToolsRegistrationHeaderHandler : DelegatingHandler
	//{
	//	readonly IToolRegistrationParameters parameters;

	//	public ToolsRegistrationHeaderHandler(IToolRegistrationParameters parameters)
	//	{
	//		this.parameters = parameters
	//						  ?? throw new ArgumentNullException(nameof(parameters));
	//	}

	//	protected override async Task<HttpResponseMessage> SendAsync(
	//		HttpRequestMessage request,
	//		CancellationToken cancellationToken)
	//	{
	//		Guid apiKey = parameters.ApiKey;

	//		request.Headers.Add(
	//			ToolsRegistrationHeaders.ExternalId,
	//			parameters.ExternalId.ToString());

	//		request.Headers.Authorization 
	//			= new AuthenticationHeaderValue(
	//				ToolsRegistrationHeaders.ApiKey, 
	//				apiKey.ToString());
	//		try
	//		{
	//			return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
	//		}
	//		finally
	//		{
	//			request.Headers.Clear();
	//		}
	//	}
	//}

}