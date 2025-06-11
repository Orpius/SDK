//using Orpius.Platform.Tooling.ToolRegistration;
//using Orpius.Platform.Tooling.Utilities;
//using Orpius.Platform.Tooling.RpcToolsRegistrationService;
//using Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius;

//namespace Sample_AspNetCore_ProtobufNet.DevelopmentOnly
//{
//	public class GeneratedToolInfo
//	{
//		public IToolsMetadata ToolsMetadata => toolsMetadata.Value;
		
//		public GeneratedToolInfo()
//		{
//			toolsMetadata = new Lazy<IToolsMetadata>(GetMetadata);
//		}

//		readonly Lazy<IToolsMetadata> toolsMetadata;

//		IToolsMetadata GetMetadata()
//		{
//			ToolMessage flightTool = new(nameof(FlightStatusChecker), typeof(FlightStatusChecker).FullName!)
//			{
//				Methods = new List<ToolMethodMessage>
//				{
//					new ToolMethodMessage(methodName: nameof(FlightStatusChecker.GetStatus),
//						parameterContractTypeName: typeof(GetStatusRequest).FullName!,
//						returnsContractTypeName: typeof(GetStatusResponse).FullName!,
//						description:"Returns the flight status for the specified flight.")
//				}
//			};

//			ToolMessage weatherTool = new(
//				toolName: nameof(WeatherForecaster),
//				typeName: typeof(WeatherForecaster).FullName!)
//			{
//				Methods = new List<ToolMethodMessage>
//				{
//					new ToolMethodMessage(methodName: "GetForecast",
//						parameterContractTypeName: typeof(GetForecastRequest).FullName!,
//						returnsContractTypeName:   typeof(GetForecastResponse).FullName!,
//						description: "Retrieve the forecast for the specified date."),
//					new ToolMethodMessage(methodName: nameof(WeatherForecaster.GetClothingRecommendation),
//						parameterContractTypeName: typeof(GetClothingRecommendationRequest).FullName!,
//						returnsContractTypeName:   typeof(GetClothingRecommendationResponse).FullName!,
//						description: "Suggests clothing based on the forecast for the specified date.")
//				}
//			};

//			var enumConverter = new EnumToDictionaryConverter();

//			var contracts
//				= new List<ContractMessage>()
//				{
//					new ContractMessage
//					{
//						SimpleContract = new SimpleContractMessage(typeof(int).FullName!)
//					},
//					new ContractMessage
//					{
//						SimpleContract = new SimpleContractMessage(typeof(DateTime).FullName!)
//					},
//					new ContractMessage
//					{
//						SimpleContract = new SimpleContractMessage(typeof(string).FullName!)
//					},
//					/* Flights */
//					new ContractMessage
//					{
//						EnumContract = new EnumContractMessage(typeof(FlightStatus).FullName!, enumConverter.GetEnumDictionary<FlightStatus>())
//					},
//					new ContractMessage
//					{
//						ComplexContract = new ComplexContractMessage(typeName: typeof(GetStatusRequest).FullName!)
//						{
//							Properties = new List<ContractPropertyMessage>
//							{
//								new ContractPropertyMessage(propertyName: nameof(GetStatusRequest.FlightNumber),
//									typeName: typeof(int).FullName!)
//								{
//									Required = true
//								}
//							}
//						},
//					},
//					new ContractMessage
//					{
//						ComplexContract = new ComplexContractMessage(typeName: typeof(GetStatusResponse).FullName!)
//						{
//							Properties = new List<ContractPropertyMessage>
//							{
//								new ContractPropertyMessage(propertyName: nameof(GetStatusResponse.DepartureTime),
//									typeName: typeof(DateTime).FullName!)
//								{
//									Required = false
//								},
//								new ContractPropertyMessage(propertyName: nameof(GetStatusResponse.FlightStatus),
//									typeName: typeof(FlightStatus).FullName!)
//								{
//									Required = false
//								},
//								new ContractPropertyMessage(propertyName: nameof(GetStatusResponse.ExtraInformation),
//									typeName: typeof(FlightStatus).FullName!)
//								{
//									Required = false
//								}
//							}
//						}
//					},
//					/* Weather */
//					new ContractMessage
//					{
//						ComplexContract = new ComplexContractMessage(typeof(GetForecastRequest).FullName!)
//						{
//							Properties = new List<ContractPropertyMessage>
//							{
//								new ContractPropertyMessage(nameof(GetForecastRequest.ForecastDate), typeof(DateTime).FullName!)
//								{
//									Required    = true,
//									Description = "The date and time for which the weather forecast is requested.",
//									OpenApiFormat      = "date-time",
//									RepresentAs = typeof(string).FullName!
//								}
//							}
//						}
//					},
//					new ContractMessage
//					{
//						EnumContract = new EnumContractMessage(typeof(WeatherConditions).FullName!, enumConverter.GetEnumDictionary<WeatherConditions>())
//					},
//					new ContractMessage
//					{
//						ComplexContract = new ComplexContractMessage(typeof(GetForecastResponse).FullName!)
//						{
//							Properties = new List<ContractPropertyMessage>
//							{
//								new ContractPropertyMessage("WeatherConditions", typeof(WeatherConditions).FullName!),
//								new ContractPropertyMessage(nameof(GetForecastResponse.TemperatureInCelsius), typeof(int).FullName!)
//							}
//						}
//					},
//					new ContractMessage
//					{
//						ComplexContract = new ComplexContractMessage(typeof(GetClothingRecommendationRequest).FullName!)
//						{
//							Properties = new List<ContractPropertyMessage>
//							{
//								new ContractPropertyMessage(nameof(GetClothingRecommendationRequest.DateTime), typeof(DateTime).FullName!)
//								{
//									Required = true,
//									Description = "The date and time for which the clothing recommendation is requested.",
//									OpenApiFormat      = "date-time",
//									RepresentAs = typeof(string).FullName!
//								}
//							}
//						}
//					},
//					new ContractMessage
//					{
//						ComplexContract = new ComplexContractMessage(typeof(ClothingRecommendation).FullName!)
//						{
//							Properties = new List<ContractPropertyMessage>
//							{
//								new ContractPropertyMessage(nameof(ClothingRecommendation.Outerwear), typeof(string).FullName!),
//								new ContractPropertyMessage(nameof(ClothingRecommendation.Accessories), typeof(string).FullName!)
//							}
//						}
//					},
//					new ContractMessage
//					{
//						ComplexContract = new ComplexContractMessage(typeof(GetClothingRecommendationResponse).FullName!)
//							{
//								Properties = new List<ContractPropertyMessage>
//								{
//									new ContractPropertyMessage(nameof(GetClothingRecommendationResponse.ForecastSummary), typeof(string).FullName!),
//									new ContractPropertyMessage(nameof(GetClothingRecommendationResponse.Recommendation), typeof(ClothingRecommendation).FullName!)
//								}
//							}
//					}
//				};

//			return new ToolsMetadata([flightTool, weatherTool], contracts);
//		}
//	}
//}