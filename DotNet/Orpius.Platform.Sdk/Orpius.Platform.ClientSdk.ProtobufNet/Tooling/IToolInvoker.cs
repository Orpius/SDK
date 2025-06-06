using System.Collections.Generic;
using System.Threading.Tasks;
using Orpius.Platform.Text.Json;
using Orpius.Platform.Tooling.ToolRegistration;
using Orpius.Platform.ToolsModel.RpcToolProviderService;

namespace Orpius.Platform.Tooling
{
	public interface IToolInvoker
	{
		//bool IsInvokerForTool(string toolName);

		//bool RemoveTool(string toolName);
		IReadOnlyCollection<string> ToolNames { get; }

		Task<UseToolResponse> InvokeToolAsync(UseToolRequest request,
											  ICombinedContext context,
											  IJsonSerializer serializer,
											  IToolResolver resolver);
	}
}
