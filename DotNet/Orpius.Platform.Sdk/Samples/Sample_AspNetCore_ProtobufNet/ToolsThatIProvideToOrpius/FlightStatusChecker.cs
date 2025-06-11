using Grpc.Core;

using Orpius.Platform.Tooling;

namespace Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius
{
	[Tool]
	public class FlightStatusChecker
	{
		[ToolMethod(Description = "Returns the flight status for the specified flight.")]
		public async Task<GetStatusResponse> GetStatus(GetStatusRequest request, ICombinedContext context)
		{
			switch (request.FlightNumber)
			{
				case 123:
					return new GetStatusResponse
					{
						DepartureTime = DateTime.UtcNow + TimeSpan.FromHours(1),
						FlightStatus = FlightStatus.Delayed,
						ExtraInformation = "Undergoing mechanical repairs."
					};
				case 456:
					return new GetStatusResponse
					{
						DepartureTime = DateTime.UtcNow + TimeSpan.FromHours(3),
						FlightStatus  = FlightStatus.OnTime,
						ExtraInformation = string.Empty
					};
				default:
					throw new RpcException(new Status(StatusCode.NotFound, 
						$"Flight {request.FlightNumber} not found."));
			}
		}
	}

	public class GetStatusRequest
	{
		[ToolProperty(Required = true)]
		public required int FlightNumber { get; set; }
	}

	public class GetStatusResponse
	{
		[ToolProperty]
		public required DateTime DepartureTime { get; set; }

		[ToolProperty]
		public required FlightStatus FlightStatus { get; set; }

		[ToolProperty(Description = "A field for the extra information.")]
		public required string ExtraInformation { get; set; }
	}

	public enum FlightStatus
	{
		OnTime = 0,
		Delayed = 1
	}
}
