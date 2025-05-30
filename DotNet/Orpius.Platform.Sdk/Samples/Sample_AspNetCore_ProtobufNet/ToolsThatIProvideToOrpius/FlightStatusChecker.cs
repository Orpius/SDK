using Grpc.Core;

using Orpius.Platform.ToolsModel;

namespace Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius
{
	[Tool(1)]
	public class FlightStatusChecker
	{
		[ToolMethod]
		public async Task<GetStatusResponse> GetStatus(GetStatusRequest request, ToolContext context)
		{
			switch (request.FlightNumber)
			{
				case 123:
					return new GetStatusResponse
					{
						DepartureTime = DateTime.UtcNow + TimeSpan.FromHours(1),
						FlightStatus = FlightStatus.Delayed
					};
				case 456:
					return new GetStatusResponse
					{
						DepartureTime = DateTime.UtcNow + TimeSpan.FromHours(3),
						FlightStatus  = FlightStatus.OnTime
					};
				default:
					throw new RpcException(new Status(StatusCode.NotFound, 
						$"Flight {request.FlightNumber} not found."));
			}
		}
	}

	[ToolContract]
	public class GetStatusRequest
	{
		[ToolProperty(Required = true)]
		public required int FlightNumber { get; set; }
	}

	[ToolContract]
	public class GetStatusResponse
	{
		[ToolProperty]
		public required DateTime DepartureTime { get; set; }

		[ToolProperty]
		public required FlightStatus FlightStatus { get; set; }
	}

	public enum FlightStatus
	{
		OnTime = 0,
		Delayed = 1
	}
}
