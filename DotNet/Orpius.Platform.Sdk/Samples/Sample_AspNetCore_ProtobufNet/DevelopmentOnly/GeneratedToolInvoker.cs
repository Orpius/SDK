using Orpius.Platform.Text.Json;
using Orpius.Platform.Tooling;
using Orpius.Platform.Tooling.ToolRegistration;
using Orpius.Platform.ToolsModel.RpcToolProviderService;

using Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius;


namespace Sample_AspNetCore_ProtobufNet.DevelopmentOnly
{
	partial class GeneratedToolInvoker : IToolInvoker
	{
		readonly HashSet<string> toolNames = new HashSet<string>() { nameof(FlightStatusChecker) };

		public bool IsInvokerForTool(string toolName)
		{
			return toolNames.Contains(toolName);
		}

		public bool RemoveTool(string toolName)
		{
			return toolNames.Remove(toolName);
		}

		public IReadOnlyCollection<string> ToolNames => toolNames;

		public async Task<UseToolResponse> InvokeToolAsync(UseToolRequest request,
														   ICombinedContext context,
														   IJsonSerializer serializer,
														   IToolResolver resolver)
		{
			const string requestBodyParam
				= nameof(UseToolRequest) + "." + nameof(UseToolRequest.ParameterAsJson);

			if (string.IsNullOrWhiteSpace(request.ParameterAsJson))
			{
				throw new ArgumentException(
					$"{requestBodyParam} must not be null or whitespace.",
					requestBodyParam);
			}

			string requestBody = request.ParameterAsJson;

			switch (request.ToolName)
			{
				case nameof(FlightStatusChecker):
				{
					if (!resolver.TryGetTool<FlightStatusChecker>(out FlightStatusChecker tool))
					{
						throw new ArgumentOutOfRangeException(
							$"No tool registered with name '{nameof(FlightStatusChecker)}'.");
					}

					switch (request.ToolMember)
					{
						case nameof(FlightStatusChecker.GetStatus):
						{
							GetStatusRequest parameter = serializer.Deserialize<GetStatusRequest>(requestBody);
							GetStatusResponse response = await tool.GetStatus(parameter, context);
							string resultAsJson = serializer.Serialize(response);
							return new UseToolResponse { ResultAsJson = resultAsJson };
						}
						default:
							throw new ArgumentException(
								$"Member '{request.ToolMember}' not found for tool '{request.ToolName}'.");
					}
				}
				default:
					throw new ArgumentException($"Tool not found '{request.ToolName}'.");
			}
		}
	}
}