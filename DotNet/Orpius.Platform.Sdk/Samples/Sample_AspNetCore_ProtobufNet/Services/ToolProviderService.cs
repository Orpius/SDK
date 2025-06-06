using Orpius.Platform.RpcServices;
using Orpius.Platform.ToolsModel.RpcToolProviderService;

using ProtoBuf.Grpc;

namespace Sample_AspNetCore_ProtobufNet.Services
{
	public class ToolProviderService : IToolProviderService
	{
		public async Task<UseToolResponse> UseTool(UseToolRequest request, CallContext context = default)
		{
			/*
				ToolProviderService will be implemented in the SDK.
				First we 
			 */
			return new UseToolResponse();
		}
	}
}
