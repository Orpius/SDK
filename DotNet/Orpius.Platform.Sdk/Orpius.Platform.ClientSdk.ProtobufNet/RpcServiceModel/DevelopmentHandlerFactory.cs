using System.Net.Http;

namespace Orpius.Platform.RpcServiceModel
{
	class DevelopmentHandlerFactory
	{
		internal static HttpClientHandler CreateHandler() => new HttpClientHandler
		{
			/* For use with self-signed certificate. Not for production. */
			ServerCertificateCustomValidationCallback
				= HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
		};
	}
}
