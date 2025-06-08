//using Orpius.Platform.Text.Json;
//using Orpius.Platform.Tooling;
//using Orpius.Platform.Tooling.ToolRegistration;
//using Orpius.Platform.ToolsModel.RpcToolProviderService;

//using Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius;
//// ReSharper disable RedundantTypeArgumentsOfMethod
//// ReSharper disable PartialTypeWithSinglePart


//namespace Sample_AspNetCore_ProtobufNet.DevelopmentOnly
//{
//	partial class GeneratedToolInvoker : IToolInvoker
//	{
//		readonly HashSet<string> toolNames = new HashSet<string>()
//		{
//			nameof(global::Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius.FlightStatusChecker),
//			"WeatherForecast"
//		};

//		public bool IsInvokerForTool(string toolName)
//		{
//			return toolNames.Contains(toolName);
//		}

//		public bool RemoveTool(string toolName)
//		{
//			return toolNames.Remove(toolName);
//		}

//		public IReadOnlyCollection<string> ToolNames => toolNames;

//		public async Task<UseToolResponse> InvokeToolAsync(UseToolRequest request,
//														   ICombinedContext context,
//														   IJsonSerializer serializer,
//														   IToolResolver resolver)
//		{
//			const string requestBodyParam
//				= nameof(UseToolRequest) + "." + nameof(UseToolRequest.ParameterAsJson);

//			if (string.IsNullOrWhiteSpace(request.ParameterAsJson))
//			{
//				throw new ArgumentException(
//					$"{requestBodyParam} must not be null or whitespace.",
//					requestBodyParam);
//			}

//			string requestBody = request.ParameterAsJson;

//			switch (request.ToolName)
//			{
//				case nameof(global::Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius.FlightStatusChecker):
//				{
//					if (!resolver.TryGetTool<global::Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius.FlightStatusChecker>(out global::Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius.FlightStatusChecker? tool))
//					{
//						throw new ArgumentOutOfRangeException(
//							$"No tool registered with name '{nameof(global::Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius.FlightStatusChecker)}'.");
//					}

//					switch (request.ToolMember)
//					{
//						case nameof(global::Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius.FlightStatusChecker.GetStatus):
//						{
//							var parameter = serializer.Deserialize<GetStatusRequest>(requestBody);
//							var response = await tool.GetStatus(parameter, context);
//							string resultAsJson = serializer.Serialize(response);
//							return new UseToolResponse { ResultAsJson = resultAsJson };
//						}
//						default:
//							throw new ArgumentException(
//								$"Member '{request.ToolMember}' not found for tool '{request.ToolName}'.");
//					}
//				}
//				case "WeatherForecast":
//				{
//					if (!resolver.TryGetTool<global::Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius.WeatherForecaster>(out global::Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius.WeatherForecaster? tool))
//					{
//						throw new ArgumentOutOfRangeException(
//							$"No tool registered with name 'WeatherForecast'.");
//					}

//					switch (request.ToolMember)
//					{
//						case "GetForecast":
//						{
//							var parameter = serializer.Deserialize<GetForecastRequest>(requestBody);
//							var response = await tool.GetForecastAsync(parameter, context);
//							string resultAsJson = serializer.Serialize(response);
//							return new UseToolResponse { ResultAsJson = resultAsJson };
//						}
//						case nameof(global::Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius.WeatherForecaster.GetClothingRecommendation):
//						{
//							var parameter = serializer.Deserialize<GetClothingRecommendationRequest>(requestBody);
//							var response = await tool.GetClothingRecommendation(parameter, context);
//							string resultAsJson = serializer.Serialize(response);
//							return new UseToolResponse { ResultAsJson = resultAsJson };
//						}
//						default:
//							throw new ArgumentException(
//								$"Member '{request.ToolMember}' not found for tool '{request.ToolName}'.");
//					}
//				}
//				default:
//					throw new ArgumentException($"Tool not found '{request.ToolName}'.");
//			}
//		}
//	}
//}