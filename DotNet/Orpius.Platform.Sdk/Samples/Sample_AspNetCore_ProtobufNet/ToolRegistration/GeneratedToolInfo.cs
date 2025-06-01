using Orpius.Platform.ToolsModel.RpcToolsRegistrationService;

using Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius;

namespace Sample_AspNetCore_ProtobufNet.ToolRegistration
{
	public class GeneratedToolInfo
	{
		internal ToolsAndContracts GetToolsAndContracts()
		{
			ToolMessage tool = new()
			{
				ToolName = nameof(FlightStatusChecker),
				TypeName = typeof(FlightStatusChecker).FullName,
				//Version = GetTimeBasedInt(),
				Methods = new List<ToolMethodMessage>()
				{
					new ToolMethodMessage
					{
						Name             = nameof(FlightStatusChecker.GetStatus),
						ContractTypeName = typeof(GetStatusRequest).FullName
					}
				}
			};

			List<ToolContractMessage> contracts
				= new()
				{
					new ToolContractMessage
					{
						Name = typeof(GetStatusRequest).FullName,
						Properties = new List<ContractPropertyMessage>()
						{
							new ContractPropertyMessage()
							{
								Name     = nameof(GetStatusRequest.FlightNumber),
								TypeName = typeof(int).FullName,
								Required = true
							}
						}
					},
					new ToolContractMessage
					{
						Name = typeof(GetStatusResponse).FullName,
						Properties = new List<ContractPropertyMessage>()
						{
							new ContractPropertyMessage
							{
								Name     = nameof(GetStatusResponse.DepartureTime),
								TypeName = typeof(DateTime).FullName,
								Required = false,
							},
							new ContractPropertyMessage
							{
								Name     = nameof(GetStatusResponse.FlightStatus),
								TypeName = typeof(FlightStatus).FullName,
								Required = false,
							}
						}
					},
				};

			return new ToolsAndContracts([tool], contracts);
		}

		int GetTimeBasedInt()
		{
			var unixTimeMilliseconds = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
			return (int)(unixTimeMilliseconds & int.MaxValue); // mask to keep within positive int range
		}
	}
}