using Grpc.Core;

using Orpius.Platform.Tooling;

namespace ToolHosting_AspNetCore.ToolsForOrpius
{
	[Tool]
	public class QuantumQasm3Tool
	{
		readonly QuantumSimulatorClient quantumSimulatorClient;

		public QuantumQasm3Tool(QuantumSimulatorClient quantumSimulatorClient)
		{
			this.quantumSimulatorClient = quantumSimulatorClient
				?? throw new ArgumentNullException(nameof(quantumSimulatorClient));
		}

		[ToolMethod(Description
			= "Executes an OpenQASM 3 program on the quantum simulator and returns measurement counts. " +
			"'Shots' controls the number of samples taken.")]
		public async Task<ExecuteQasm3ProgramResponse> ExecuteQasm3Program(
			ExecuteQasm3ProgramRequest request,
			ICombinedContext context)
		{
			if (request is null)
			{
				throw new ArgumentNullException(nameof(request));
			}

			if (string.IsNullOrWhiteSpace(request.OpenQasm3Program))
			{
				throw new RpcException(new Status(
					StatusCode.InvalidArgument,
					"OpenQasm3Program is required and cannot be empty."));
			}

			if (request.Shots <= 0)
			{
				throw new RpcException(new Status(
					StatusCode.InvalidArgument,
					"Shots must be greater than 0."));
			}

			CancellationToken token = GetCancellationToken(context);

			/* You said you already have QuantumSimulatorClient and will plug it in.
			   Pick whichever signature you have and adapt this call.

			   Example expectation:
			     Task<Dictionary<string, int>> ExecuteAsync(string program, int shots, CancellationToken token)
			*/
			Dictionary<string, int> counts =
				await quantumSimulatorClient.ExecuteAsync(request.OpenQasm3Program, request.Shots, token)
					.ConfigureAwait(false);

			return new ExecuteQasm3ProgramResponse
			{
				Counts = counts
			};
		}

		static CancellationToken GetCancellationToken(ICombinedContext context)
		{
			if (context is null)
			{
				return default;
			}

			/* The SDK guide shows NativeContext is a ProtoBuf.Grpc.CallContext for tool calls. */
			if (context.NativeContext is ProtoBuf.Grpc.CallContext callContext)
			{
				return callContext.CancellationToken;
			}

			return default;
		}
	}

	public class ExecuteQasm3ProgramRequest
	{
		[ToolProperty(Required = true, Description = "The OpenQASM 3 program text to execute.")]
		public required string OpenQasm3Program { get; set; }

		[ToolProperty(Description = "Number of shots (samples) to run. Defaults to 1024.")]
		public int Shots { get; set; } = 1024;
	}

	public class ExecuteQasm3ProgramResponse
	{
		[ToolProperty(Description =
			"Measurement counts keyed by bitstring outcome (for example, \"00\", \"01\", \"10\", \"11\").")]
		public Dictionary<string, int>? Counts { get; init; }
	}
}
