using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;

namespace Sample_AspNetCore_ProtobufNet.RpcServiceModel
{
	class ApplicationUrlResolver
	{
		public string GetApplicationUrl(IServiceProvider services)
		{
			var serverAddresses = services
									 .GetRequiredService<IServer>()
									 .Features
									 .Get<IServerAddressesFeature>()
									 ?.Addresses;

			return serverAddresses?.FirstOrDefault()
				   ?? throw new InvalidOperationException("Unable to determine application URL.");
		}
	}
}
