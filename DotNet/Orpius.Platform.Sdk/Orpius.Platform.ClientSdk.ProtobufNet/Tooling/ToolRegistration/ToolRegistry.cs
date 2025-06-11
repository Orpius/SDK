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

		Task<IEnumerable<RegistrationResult>> RegisterWithServerAsync(
			CancellationToken token = default);

		Task<IEnumerable<DeregistrationResult>> DeregisterWithServerAsync(
			CancellationToken token = default);
	}

	public class RegistrationResult
	{
		public Guid                      ExternalId { get; set; }
		public RegisterAsProviderResponse? Response   { get; set; }
		public Exception?                  Error      { get; set; }
	}

	public class DeregistrationResult
	{
		public Guid                        ExternalId { get; set; }
		public DeregisterAsProviderResponse? Response   { get; set; }
		public Exception?                  Error      { get; set; }
	}

	public interface IToolCaller
	{
		Task<UseToolResponse> UseToolAsync(UseToolRequest request, object? nativeContext);
	}

	public class ToolRegistry : IToolRegistry, IToolCaller
	{
		readonly IRegistrationMediator mediator;
		readonly IEnumerable<IToolRegistrationParameters> registrationParameters;
		public IJsonSerializer JsonSerializer { get; set; } = new JsonSerializer();

		readonly ConcurrentDictionary<string, IToolRegistryItem> registryItems
			= new ConcurrentDictionary<string, IToolRegistryItem>(StringComparer.OrdinalIgnoreCase);

		public IToolResolver ToolResolver { get; set; }

		public ToolRegistry(IToolResolver toolResolver,
							IRegistrationMediator mediator,
							IEnumerable<IToolRegistrationParameters> registrationParameters)
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

		public async Task<IEnumerable<RegistrationResult>> RegisterWithServerAsync(
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

			List<RegistrationResult> results = new List<RegistrationResult>();
			
			foreach (IToolRegistrationParameters parameters in registrationParameters)
			{
				var externalId = parameters.ToolsetExternalId;

				RegisterAsProviderRequest request
					= new RegisterAsProviderRequest(
						toolsetExternalId: externalId,
						providerUrl: parameters.LocalUrl.ToString(),
						tools.Values.ToList(),
						contracts.Values.ToList(),
						ProgrammingLanguageId.CSharp);
				try
				{
					var response = await mediator.RegisterAsProviderAsync(request, token);
					results.Add(new RegistrationResult { ExternalId = externalId, Response = response });
				}
				catch (Exception ex)
				{
					results.Add(new RegistrationResult { ExternalId = externalId, Error = ex });
				}
			}

			return results;
		}

		public async Task<IEnumerable<DeregistrationResult>> DeregisterWithServerAsync(
			CancellationToken token)
		{
			List<DeregistrationResult> results = new List<DeregistrationResult>();

			foreach (IToolRegistrationParameters parameters in registrationParameters)
			{
				var externalId = parameters.ToolsetExternalId;
				DeregisterAsProviderRequest request
					= new DeregisterAsProviderRequest(
						providerUrl: parameters.LocalUrl.ToString());

				try
				{
					var response = await mediator.DeregisterAsProviderAsync(request, token);
					results.Add(new DeregistrationResult { ExternalId = externalId, Response = response });
				}
				catch (Exception ex)
				{
					results.Add(new DeregistrationResult { ExternalId = externalId, Error = ex });
				}
			}

			return results;
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
