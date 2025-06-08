using Orpius.Platform.Tooling.ToolRegistration;
using Orpius.Platform.ToolsModel.RpcToolsRegistrationService;
using Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius;

namespace Sample_AspNetCore_ProtobufNet.DevelopmentOnly
{
	public class GeneratedToolInfo
	{
		internal static ToolsMetadata GetToolsAndContracts()
		{
			ToolMessage tool = new(nameof(FlightStatusChecker), typeof(FlightStatusChecker).FullName!)
			{
				Methods = new List<ToolMethodMessage>
				{
					new(methodName: nameof(FlightStatusChecker.GetStatus),
						parameterContractTypeName: typeof(GetStatusRequest).FullName!,
						returnsContractTypeName: typeof(GetStatusResponse).FullName!,
						description:"Returns the flight status for the specified flight.")
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
								} /*,
								new(propertyName: nameof(GetStatusResponse.ExtraInformation),
									typeName: typeof(FlightStatus).FullName!)
								{
									Required = false
								}*/
							}
						}
					}
				};
					
				

			return new ToolsMetadata([tool], contracts);
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