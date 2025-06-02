using Orpius.Platform.ToolsModel.RpcToolsRegistrationService;

using Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius;

namespace Sample_AspNetCore_ProtobufNet.ToolRegistration
{
	public class GeneratedToolInfo
	{
		internal ToolsAndContracts GetToolsAndContracts()
		{
			ToolMessage tool = new(nameof(FlightStatusChecker), typeof(FlightStatusChecker).FullName!)
			{
				Methods = new List<ToolMethodMessage>
				{
					new(methodName: nameof(FlightStatusChecker.GetStatus),
						parameterContractTypeName: typeof(GetStatusRequest).FullName!,
						returnsContractTypeName: typeof(GetStatusResponse).FullName!)
				}
			};

			List<ToolContractMessage> contracts
				= new()
				{
					new ToolContractMessage(typeName: typeof(GetStatusRequest).FullName!)
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
					new ToolContractMessage(typeName: typeof(GetStatusResponse).FullName!)
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
							}
						}
					}
				};

			return new ToolsAndContracts([tool], contracts);
		}
	}
}