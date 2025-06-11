//using System;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using System.Threading;
//using System.Threading.Tasks;

//using Orpius.Platform.OperationsModel;
//using Orpius.Platform.OperationsModel.RpcOperationsService;
//using Orpius.Platform.Tooling.RpcToolsRegistrationService;

//namespace Orpius.Platform.Tooling.ToolRegistration
//{
//	public class OperationsHeaderHandler : DelegatingHandler
//	{
//		readonly IOperationsServiceParameters parameters;

//		public OperationsHeaderHandler(IOperationsServiceParameters parameters)
//		{
//			this.parameters = parameters
//							  ?? throw new ArgumentNullException(nameof(parameters));
//		}

//		protected override async Task<HttpResponseMessage> SendAsync(
//			HttpRequestMessage request,
//			CancellationToken cancellationToken)
//		{
//			Guid apiKey = await parameters.GetApiKeyAsync();

//			request.Headers.Add(
//				OperationHeaders.ExternalId,
//				parameters.ExternalId.ToString());

//			request.Headers.Authorization = new AuthenticationHeaderValue(
//				OperationHeaders.ApiKey, 
//				apiKey.ToString());

//			try
//			{
//				return await base.SendAsync(request, cancellationToken);
//			}
//			finally
//			{
//				request.Headers.Clear();
//			}
//		}
//	}
//}