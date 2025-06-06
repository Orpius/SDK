using Orpius.Platform.RpcServices;
using Orpius.Platform.Tooling.ToolRegistration;
using Orpius.Platform.ToolsModel.RpcToolProviderService;

using ProtoBuf.Grpc;
using Sample_AspNetCore_ProtobufNet.DevelopmentOnly;

namespace Sample_AspNetCore_ProtobufNet.Services
{
	public class ToolProviderService : IToolProviderService
	{
		ToolRegistry? registry;
		public async Task<UseToolResponse> UseTool(UseToolRequest request, CallContext context = default)
		{
			/*
			ToolProviderService will be implemented in the SDK.
			First we resolve the tool registry.
			We resolve the tool using its name from the request.
			We resolve the tool method using the endpoint name.
			We rehydrate the 
			 */

			registry ??= new ToolRegistryForDevelopment().GetRegistry();

			return await registry.UseToolAsync(request, context);

			//return new UseToolResponse();
		}
	}
}
