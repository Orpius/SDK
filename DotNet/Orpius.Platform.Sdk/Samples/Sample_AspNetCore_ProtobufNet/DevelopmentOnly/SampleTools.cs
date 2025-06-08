using Orpius.Platform.Generators;
using Orpius.Platform.Text.Json;
using Orpius.Platform.Tooling;
using Orpius.Platform.Tooling.ToolRegistration;
using Orpius.Platform.Tooling.Utilities;
using Orpius.Platform.ToolsModel.RpcToolProviderService;
using Orpius.Platform.ToolsModel.RpcToolsRegistrationService;

using Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius;
// ReSharper disable RedundantNameQualifier
// ReSharper disable InconsistentNaming
// ReSharper disable RedundantTypeArgumentsOfMethod

[assembly: GenerateToolRegistryItem("Sample_AspNetCore_ProtobufNet.ToolRegistration.SampleTools")]

namespace Orpius.Platform.Generators
{
	[AttributeUsage(AttributeTargets.Assembly)]
	public class GenerateToolRegistryItemAttribute : Attribute
	{
		public GenerateToolRegistryItemAttribute(string fullClassName)
		{
			FullClassName = fullClassName;
		}

		public string FullClassName { get; set; }
	}
}

namespace Sample_AspNetCore_ProtobufNet.ToolRegistration
{
	partial class SampleTools : IToolRegistryItem
	{
		public SampleTools(IToolRegistry toolRegistry)
		{
			toolsMetadata = new Lazy<IToolsMetadata>(GetMetadata);
			toolInvoker = new Lazy<GeneratedToolInvoker>(() => new GeneratedToolInvoker());

			toolRegistry.AddItem(this);
		}

		readonly Lazy<GeneratedToolInvoker> toolInvoker;
		public IToolInvoker ToolInvoker => toolInvoker.Value;

		readonly Lazy<IToolsMetadata> toolsMetadata;
		public IToolsMetadata ToolsMetadata => toolsMetadata.Value;

		IToolsMetadata GetMetadata()
		{
			ToolMessage Sample_AspNetCore_ProtobufNet_ToolsThatIProvideToOrpius_FlightStatusCheckerTool 
				= new ToolMessage(
					nameof(global::Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius.FlightStatusChecker), 
					typeof(global::Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius.FlightStatusChecker).FullName!)
			{
				Methods = new List<ToolMethodMessage>
				{
					new ToolMethodMessage(
							methodName: nameof(global::Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius.FlightStatusChecker.GetStatus),
							parameterContractTypeName: typeof(global::Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius.GetStatusRequest).FullName!,
							returnsContractTypeName: typeof(global::Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius.GetStatusResponse).FullName!,
							description:"Returns the flight status for the specified flight.")
				}
			};

			ToolMessage Sample_AspNetCore_ProtobufNet_ToolsThatIProvideToOrpius_WeatherForecaster 
				= new ToolMessage(
					toolName: "WeatherForecast",
					typeName: typeof(global::Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius.WeatherForecaster).FullName!)
			{
				Methods = new List<ToolMethodMessage>
				{
					new ToolMethodMessage(
							methodName: "GetForecast",
							parameterContractTypeName: typeof(global::Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius.GetForecastRequest).FullName!,
							returnsContractTypeName:   typeof(global::Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius.GetForecastResponse).FullName!,
							description: "Retrieve the forecast for the specified date."),
					new ToolMethodMessage(
							methodName: nameof(global::Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius.WeatherForecaster.GetClothingRecommendation),
							parameterContractTypeName: typeof(global::Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius.GetClothingRecommendationRequest).FullName!,
							returnsContractTypeName:   typeof(global::Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius.GetClothingRecommendationResponse).FullName!,
							description: "Suggests clothing based on the forecast for the specified date.")
				}
			};

			var enumConverter = new EnumToDictionaryConverter();

			var contracts
				= new List<ContractMessage>()
				{
					new ContractMessage
					{
						SimpleContract = new SimpleContractMessage(typeof(global::System.Int32).FullName!)
					},
					new ContractMessage
					{
						SimpleContract = new SimpleContractMessage(typeof(global::System.DateTime).FullName!)
					},
					new ContractMessage
					{
						SimpleContract = new SimpleContractMessage(typeof(global::System.String).FullName!)
					},
					new ContractMessage
					{
						EnumContract = new EnumContractMessage(
							typeof(global::Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius.FlightStatus).FullName!, 
							enumConverter.GetEnumDictionary<global::Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius.FlightStatus>())
					},
					new ContractMessage
					{
						ComplexContract = new ComplexContractMessage(
							typeName: typeof(global::Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius.GetStatusRequest).FullName!)
						{
							Properties = new List<ContractPropertyMessage>
							{
								new ContractPropertyMessage(
										propertyName: nameof(global::Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius.GetStatusRequest.FlightNumber),
										typeName: typeof(global::System.Int32).FullName!)
								{
									Required = true
								}
							}
						},
					},
					new ContractMessage
					{
						ComplexContract = new ComplexContractMessage(
							typeName: typeof(global::Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius.GetStatusResponse).FullName!)
						{
							Properties = new List<ContractPropertyMessage>
							{
								new ContractPropertyMessage(
										propertyName: nameof(global::Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius.GetStatusResponse.DepartureTime),
										typeName: typeof(global::System.DateTime).FullName!)
								{
									Required = false
								},
								new ContractPropertyMessage(
										propertyName: nameof(global::Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius.GetStatusResponse.FlightStatus),
										typeName: typeof(global::Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius.FlightStatus).FullName!)
								{
									Required = false
								},
								new ContractPropertyMessage(
										propertyName: nameof(global::Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius.GetStatusResponse.ExtraInformation),
										typeName: typeof(global::Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius.FlightStatus).FullName!)
								{
									Required = false
								}
							}
						}
					},
					new ContractMessage
					{
						ComplexContract = new ComplexContractMessage(
							typeof(global::Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius.GetForecastRequest).FullName!)
						{
							Properties = new List<ContractPropertyMessage>
							{
								new ContractPropertyMessage(
										nameof(global::Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius.GetForecastRequest.ForecastDate), 
										typeof(global::System.DateTime).FullName!)
								{
									Required = true,
									Description = "The date and time for which the weather forecast is requested.",
									OpenApiFormat = "date-time",
									RepresentAs = typeof(global::System.String).FullName!
								}
							}
						}
					},
					new ContractMessage
					{
						EnumContract = new EnumContractMessage(
							typeof(global::Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius.WeatherConditions).FullName!, 
							enumConverter.GetEnumDictionary<global::Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius.WeatherConditions>())
					},
					new ContractMessage
					{
						ComplexContract = new ComplexContractMessage(
							typeof(global::Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius.GetForecastResponse).FullName!)
						{
							Properties = new List<ContractPropertyMessage>
							{
								new ContractPropertyMessage(
										"WeatherConditions", 
										typeof(global::Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius.WeatherConditions).FullName!),
								new ContractPropertyMessage(
										nameof(global::Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius.GetForecastResponse.TemperatureInCelsius), 
										typeof(global::System.Int32).FullName!)
							}
						}
					},
					new ContractMessage
					{
						ComplexContract = new ComplexContractMessage(
							typeof(global::Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius.GetClothingRecommendationRequest).FullName!)
						{
							Properties = new List<ContractPropertyMessage>
							{
								new ContractPropertyMessage(
										nameof(global::Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius.GetClothingRecommendationRequest.DateTime), typeof(global::System.DateTime).FullName!)
								{
									Required = true,
									Description = "The date and time for which the clothing recommendation is requested.",
									OpenApiFormat = "date-time",
									RepresentAs = typeof(global::System.String).FullName!
								}
							}
						}
					},
					new ContractMessage
					{
						ComplexContract = new ComplexContractMessage(
							typeof(global::Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius.ClothingRecommendation).FullName!)
						{
							Properties = new List<ContractPropertyMessage>
							{
								new ContractPropertyMessage(
										nameof(global::Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius.ClothingRecommendation.Outerwear), 
										typeof(global::System.String).FullName!),
								new ContractPropertyMessage(
										nameof(global::Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius.ClothingRecommendation.Accessories), 
										typeof(global::System.String).FullName!)
							}
						}
					},
					new ContractMessage
					{
						ComplexContract = new ComplexContractMessage(typeof(GetClothingRecommendationResponse).FullName!)
							{
								Properties = new List<ContractPropertyMessage>
								{
									new ContractPropertyMessage(
											nameof(global::Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius.GetClothingRecommendationResponse.ForecastSummary), 
											typeof(global::System.String).FullName!),
									new ContractPropertyMessage(
											nameof(global::Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius.GetClothingRecommendationResponse.Recommendation), 
											typeof(global::Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius.ClothingRecommendation).FullName!)
								}
							}
					}
				};

			return new ToolsMetadata(
				[
					Sample_AspNetCore_ProtobufNet_ToolsThatIProvideToOrpius_FlightStatusCheckerTool,
					Sample_AspNetCore_ProtobufNet_ToolsThatIProvideToOrpius_WeatherForecaster
				], 
				contracts);
		}

		public class GeneratedToolInvoker : IToolInvoker
		{
			readonly HashSet<string> toolNames = new HashSet<string>()
			{
				nameof(global::Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius.FlightStatusChecker),
				"WeatherForecast"
			};

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
					case nameof(global::Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius.FlightStatusChecker):
					{
						if (!resolver.TryGetTool<global::Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius.FlightStatusChecker>(out global::Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius.FlightStatusChecker? tool))
						{
							throw new ArgumentOutOfRangeException(
								$"No tool registered with name '{nameof(global::Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius.FlightStatusChecker)}'.");
						}

						switch (request.ToolMember)
						{
							case nameof(global::Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius.FlightStatusChecker.GetStatus):
								{
									var parameter = serializer.Deserialize<global::Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius.GetStatusRequest>(requestBody);
									var response = await tool.GetStatus(parameter, context);
									string resultAsJson = serializer.Serialize(response);
									return new UseToolResponse { ResultAsJson = resultAsJson };
								}
							default:
								throw new ArgumentException(
									$"Member '{request.ToolMember}' not found for tool '{request.ToolName}'.");
						}
					}
					case "WeatherForecast":
					{
						if (!resolver.TryGetTool<global::Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius.WeatherForecaster>(out global::Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius.WeatherForecaster? tool))
						{
							throw new ArgumentOutOfRangeException(
								$"No tool registered with name 'WeatherForecast'.");
						}

						switch (request.ToolMember)
						{
							case "GetForecast":
								{
									var parameter = serializer.Deserialize<global::Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius.GetForecastRequest>(requestBody);
									var response = await tool.GetForecastAsync(parameter, context);
									string resultAsJson = serializer.Serialize(response);
									return new UseToolResponse { ResultAsJson = resultAsJson };
								}
							case nameof(global::Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius.WeatherForecaster.GetClothingRecommendation):
								{
									var parameter = serializer.Deserialize<global::Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius.GetClothingRecommendationRequest>(requestBody);
									var response = await tool.GetClothingRecommendation(parameter, context);
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
}
