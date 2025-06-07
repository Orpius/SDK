using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

using Orpius.Platform.Collections;
using Orpius.Platform.Text.Json;
using Orpius.Platform.ToolsModel.RpcToolProviderService;

namespace Orpius.Platform.Tooling.ToolRegistration
{
	public interface IToolRegistry
	{
		void RegisterToolInvoker(IToolInvoker toolInvoker);
	}

	public interface IToolCaller
	{
		Task<UseToolResponse> UseToolAsync(UseToolRequest request, object? nativeContext);
	}

	public class ToolRegistry : IToolRegistry, IToolCaller
	{
		public IJsonSerializer JsonSerializer { get; set; } = new JsonSerializer();

		readonly ConcurrentDictionary<string, IToolInvoker> invokerDictionary
			= new ConcurrentDictionary<string, IToolInvoker>(StringComparer.OrdinalIgnoreCase);

		public IToolResolver ToolResolver { get; set; }

		public ToolRegistry(IToolResolver toolResolver)
		{
			ToolResolver = toolResolver ?? throw new ArgumentNullException(nameof(toolResolver));
		}

		public void RegisterToolInvoker(IToolInvoker toolInvoker)
		{
			AssertArg.IsNotNull(toolInvoker, nameof(toolInvoker));

			foreach (string toolName in toolInvoker.ToolNames)
			{
				invokerDictionary[toolName] = toolInvoker;
			}
		}

		public async Task<UseToolResponse> UseToolAsync(UseToolRequest request,
														object? nativeContext)
		{
			string toolName = request.ToolName;

			if (!invokerDictionary.TryGetValue(toolName, out IToolInvoker? invoker))
			{
				throw new ArgumentOutOfRangeException($"Tool with name '{toolName}' was not found.");
			}

			const string requestBodyParam = nameof(UseToolRequest) + "." + nameof(UseToolRequest.ParameterAsJson);

			if (string.IsNullOrWhiteSpace(request.ParameterAsJson))
			{
				throw new ArgumentException($"{requestBodyParam} must not be null or whitespace.", requestBodyParam);
			}

			var trackedDictionary = new TrackedDictionary(request.Context);
			var combinedContext = new CombinedContext(trackedDictionary, nativeContext);

			return await invoker.InvokeToolAsync(request, combinedContext, JsonSerializer, ToolResolver);
		}
	}
}
