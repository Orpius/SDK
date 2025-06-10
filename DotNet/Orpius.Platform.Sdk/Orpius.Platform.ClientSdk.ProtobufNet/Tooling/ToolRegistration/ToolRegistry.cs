using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Orpius.Platform.Collections;
using Orpius.Platform.Text.Json;
using Orpius.Platform.Tooling.RpcToolProviderService;
using Orpius.Platform.Tooling.RpcToolsRegistrationService;

namespace Orpius.Platform.Tooling.ToolRegistration
{
	public interface IToolRegistry
	{
		void AddItem(IToolRegistryItem item);

		Task<RegisterAsProviderResponse> RegisterWithServerAsync(
			CancellationToken token = default);

		Task<DeregisterAsProviderResponse> DeregisterWithServerAsync(
			CancellationToken token = default);
	}

	public interface IToolCaller
	{
		Task<UseToolResponse> UseToolAsync(UseToolRequest request, object? nativeContext);
	}

	public class ToolRegistry : IToolRegistry, IToolCaller
	{
		readonly IRegistrationMediator mediator;
		readonly IToolRegistrationParameters registrationParameters;
		public IJsonSerializer JsonSerializer { get; set; } = new JsonSerializer();

		readonly ConcurrentDictionary<string, IToolRegistryItem> registryItems
			= new ConcurrentDictionary<string, IToolRegistryItem>(StringComparer.OrdinalIgnoreCase);

		public IToolResolver ToolResolver { get; set; }

		public ToolRegistry(IToolResolver toolResolver,
							IRegistrationMediator mediator,
							IToolRegistrationParameters registrationParameters)
		{
			ToolResolver  = toolResolver ?? throw new ArgumentNullException(nameof(toolResolver));
			this.mediator = mediator     ?? throw new ArgumentNullException(nameof(mediator));
			this.registrationParameters = registrationParameters
										  ?? throw new ArgumentNullException(nameof(registrationParameters));
		}

		public void AddItem(IToolRegistryItem item)
		{
			AssertArg.IsNotNull(item, nameof(item));

			foreach (string toolName in item.ToolInvoker.ToolNames)
			{
				registryItems[toolName] = item;
			}
		}

		public async Task<RegisterAsProviderResponse> RegisterWithServerAsync(
			CancellationToken token)
		{
			var contracts = new Dictionary<string, ContractMessage>();
			var tools = new Dictionary<string, ToolMessage>();

			/* Flatten items. */
			foreach (IToolRegistryItem item in registryItems.Values)
			{
				foreach (ContractMessage? contractMessage in item.ToolsMetadata.Contracts)
				{
					var contract = contractMessage.Contract;
					contracts[contract.TypeName] = contractMessage;
				}

				foreach (ToolMessage toolMessage in item.ToolsMetadata.Tools)
				{
					/* A type could be reused as a different tool,
					   therefore we use the ToolName rather than the TypeName. */
					tools[toolMessage.ToolName] = toolMessage;
				}
			}

			RegisterAsProviderRequest request
				= new RegisterAsProviderRequest(
					providerUrl: registrationParameters.LocalUrl.ToString(),
					tools.Values.ToList(),
					contracts.Values.ToList(),
					ProgrammingLanguageId.CSharp);

			//ToolsMetadata toolsMetadata = new ToolsMetadata(tools.Values, contracts.Values);

			return await mediator.RegisterAsProviderAsync(request, token);
		}

		public async Task<DeregisterAsProviderResponse> DeregisterWithServerAsync(
			CancellationToken token)
		{
			DeregisterAsProviderRequest request
				= new DeregisterAsProviderRequest(
					providerUrl: registrationParameters.LocalUrl.ToString());

			return await mediator.DeregisterAsProviderAsync(request, token);
		}

		public async Task<UseToolResponse> UseToolAsync(UseToolRequest request,
														object? nativeContext)
		{
			string toolName = request.ToolName;

			if (!registryItems.TryGetValue(toolName, out IToolRegistryItem? registryItem))
			{
				throw new ArgumentOutOfRangeException(
					$"Tool with name '{toolName}' was not found.");
			}

			const string requestBodyParam = nameof(UseToolRequest) + "." + nameof(UseToolRequest.ParameterAsJson);

			if (string.IsNullOrWhiteSpace(request.ParameterAsJson))
			{
				throw new ArgumentException(
					$"{requestBodyParam} must not be null or whitespace.", 
					requestBodyParam);
			}

			var trackedDictionary = new TrackedDictionary(request.Context);
			var combinedContext = new CombinedContext(trackedDictionary, nativeContext);

			return await registryItem.ToolInvoker.InvokeToolAsync(
					   request,
					   combinedContext, 
					   JsonSerializer, 
					   ToolResolver);
		}
	}
}
