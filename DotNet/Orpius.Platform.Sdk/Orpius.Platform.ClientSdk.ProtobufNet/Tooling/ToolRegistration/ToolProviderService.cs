using System.Threading.Tasks;

using Orpius.Platform.RpcServices;
using Orpius.Platform.Tooling.RpcToolProviderService;

using ProtoBuf.Grpc;

namespace Orpius.Platform.Tooling.ToolRegistration
{
	public class ToolProviderService : IToolProviderService
	{
		readonly IToolCaller toolCaller;

		public ToolProviderService(IToolCaller toolCaller)
		{
			this.toolCaller = AssertArg.IsNotNull(toolCaller, nameof(toolCaller));
		}

		public Task<UseToolResponse> UseTool(UseToolRequest request, CallContext context = default)
		{
			return toolCaller.UseToolAsync(request, context);
		}
	}
}
