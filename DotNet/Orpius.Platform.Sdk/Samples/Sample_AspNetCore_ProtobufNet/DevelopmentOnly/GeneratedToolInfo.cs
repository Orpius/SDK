using Orpius.Platform.Tooling.ToolRegistration;
using Orpius.Platform.ToolsModel.RpcToolsRegistrationService;
using Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius;

namespace Sample_AspNetCore_ProtobufNet.DevelopmentOnly
{
	public class GeneratedToolInfo
	{
		internal static ToolsMetadata GetToolsAndContracts()
		{
			ToolMessage flightTool = new(nameof(FlightStatusChecker), typeof(FlightStatusChecker).FullName!)
			{
				Methods = new List<ToolMethodMessage>
				{
					new(methodName: nameof(FlightStatusChecker.GetStatus),
						parameterContractTypeName: typeof(GetStatusRequest).FullName!,
						returnsContractTypeName: typeof(GetStatusResponse).FullName!,
						description:"Returns the flight status for the specified flight.")
				}
			};

			ToolMessage weatherTool = new(
				toolName: nameof(WeatherForecaster),
				typeName: typeof(WeatherForecaster).FullName!)
			{
				Methods = new List<ToolMethodMessage>
				{
					new(methodName: "GetForecast",
						parameterContractTypeName: typeof(GetForecastRequest).FullName!,
						returnsContractTypeName:   typeof(GetForecastResponse).FullName!,
						description: "Retrieve the forecast for the specified date."),
					new(methodName: nameof(WeatherForecaster.GetClothingRecommendation),
						parameterContractTypeName: typeof(GetClothingRecommendationRequest).FullName!,
						returnsContractTypeName:   typeof(GetClothingRecommendationResponse).FullName!,
						description: "Suggests clothing based on the forecast for the specified date.")
				}
			};

			List<ContractMessage> contracts
				= new()
				{
					new ContractMessage
					{
						SimpleContract = new SimpleContractMessage(typeof(int).FullName!)
					},
					new ContractMessage
					{
						SimpleContract = new SimpleContractMessage(typeof(DateTime).FullName!)
					},
					new ContractMessage
					{
						SimpleContract = new SimpleContractMessage(typeof(string).FullName!)
					},
					/* Flights */
					new ContractMessage
					{
						EnumContract = new EnumContractMessage(typeof(FlightStatus).FullName!, GetEnumDictionary<FlightStatus>())
					},
					new ContractMessage
					{
						ComplexContract = new ComplexContractMessage(typeName: typeof(GetStatusRequest).FullName!)
						{
							Properties = new List<ContractPropertyMessage>
							{
								new(propertyName: nameof(GetStatusRequest.FlightNumber),
									typeName: typeof(int).FullName!)
								{
									Required = true
								}
							}
						},
					},
					new ContractMessage
					{
						ComplexContract = new ComplexContractMessage(typeName: typeof(GetStatusResponse).FullName!)
						{
							Properties = new List<ContractPropertyMessage>
							{
								new(propertyName: nameof(GetStatusResponse.DepartureTime),
									typeName: typeof(DateTime).FullName!)
								{
									Required = false
								},
								new(propertyName: nameof(GetStatusResponse.FlightStatus),
									typeName: typeof(FlightStatus).FullName!)
								{
									Required = false
								},
								new(propertyName: nameof(GetStatusResponse.ExtraInformation),
									typeName: typeof(FlightStatus).FullName!)
								{
									Required = false
								}
							}
						}
					},
					/* Weather */
					new ContractMessage
					{
						ComplexContract = new ComplexContractMessage(typeof(GetForecastRequest).FullName!)
						{
							Properties = new List<ContractPropertyMessage>
							{
								new(nameof(GetForecastRequest.ForecastDate), typeof(DateTime).FullName!)
								{
									Required    = true,
									Description = "The date and time for which the weather forecast is requested.",
									Format      = "date-time",
									RepresentAs = typeof(string).FullName!
								}
							}
						}
					},
					new ContractMessage
					{
						EnumContract = new EnumContractMessage(typeof(WeatherConditions).FullName!, GetEnumDictionary<WeatherConditions>())
					},
					new ContractMessage
					{
						ComplexContract = new ComplexContractMessage(typeof(GetForecastResponse).FullName!)
						{
							Properties = new List<ContractPropertyMessage>
							{
								new("WeatherConditions", typeof(WeatherConditions).FullName!),
								new(nameof(GetForecastResponse.TemperatureInCelsius), typeof(int).FullName!)
							}
						}
					},
					new ContractMessage
					{
						ComplexContract = new ComplexContractMessage(typeof(GetClothingRecommendationRequest).FullName!)
						{
							Properties = new List<ContractPropertyMessage>
							{
								new(nameof(GetClothingRecommendationRequest.DateTime), typeof(DateTime).FullName!)
								{
									Required = true,
									Description = "The date and time for which the clothing recommendation is requested.",
									Format      = "date-time",
									RepresentAs = typeof(string).FullName!
								}
							}
						}
					},
					new ContractMessage
					{
						ComplexContract = new ComplexContractMessage(typeof(ClothingRecommendation).FullName!)
						{
							Properties = new List<ContractPropertyMessage>
							{
								new(nameof(ClothingRecommendation.Outerwear), typeof(string).FullName!),
								new(nameof(ClothingRecommendation.Accessories), typeof(string).FullName!)
							}
						}
					},
					new ContractMessage
					{
						ComplexContract
							= new ComplexContractMessage(typeof(GetClothingRecommendationResponse).FullName!)
							{
								Properties = new List<ContractPropertyMessage>
								{
									new(nameof(GetClothingRecommendationResponse.ForecastSummary), typeof(string).FullName!),
									new(nameof(GetClothingRecommendationResponse.Recommendation), typeof(ClothingRecommendation).FullName!)
								}
							}
					}
				};

			return new ToolsMetadata([flightTool, weatherTool], contracts);
		}

		static Dictionary<string, int> GetEnumDictionary<T>() where T : struct, Enum
		{
			var result = new Dictionary<string, int>();

			foreach (var value in Enum.GetValues<T>())
			{
				result[value.ToString()] = Convert.ToInt32(value);
			}

			return result;
		}
	}
}