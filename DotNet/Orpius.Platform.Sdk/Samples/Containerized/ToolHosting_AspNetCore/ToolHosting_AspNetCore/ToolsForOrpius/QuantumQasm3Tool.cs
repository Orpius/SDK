using Grpc.Core;

using Orpius.Platform.Tooling;

using ProtoBuf.Grpc;

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
			= "Executes an OpenQASM 3 program on the quantum simulator"
			  + " and returns measurement counts. "
			  + "'Shots' controls the number of samples taken.")]
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

			Dictionary<string, int> counts
				= await quantumSimulatorClient.ExecuteAsync(
												  request.OpenQasm3Program, request.Shots, token)
											  .ConfigureAwait(false);

			return new ExecuteQasm3ProgramResponse
			{
				Counts = counts
			};
		}

		static CancellationToken GetCancellationToken(ICombinedContext? context)
		{
			/* NativeContext is a ProtoBuf.Grpc.CallContext for tool calls. */
			if (context?.NativeContext is CallContext callContext)
			{
				return callContext.CancellationToken;
			}

			return CancellationToken.None;
		}
	}

	public class ExecuteQasm3ProgramRequest
	{
		[ToolProperty(
			Required = true,
			Description
				= """
				  OpenQASM 3 program text to execute.

				  IMPORTANT (strict parser rules):
				  - Always include: include "stdgates.inc";
				  - Put ALL declarations at the top of the program, before any gates/measurements.
				  - For arrays, use OpenQASM 3 syntax: qubit[2] q; bit[2] c; (do NOT use qubit q[2];).
				  - Declare one bit per line (or use bit[N]); avoid comma-separated declarations like: bit c0, c1;
				  - Prefer explicit qubit names (q0, q1, ...) or array form (q[i]).
				  - Comments are allowed.

				  Good examples:

				  Example A (single qubit):
				  OPENQASM 3;
				  include "stdgates.inc";

				  qubit q0;
				  bit c0;

				  h q0;
				  c0 = measure q0;

				  Example B (Bell pair, array form):
				  OPENQASM 3;
				  include "stdgates.inc";

				  qubit[2] q;
				  bit[2] c;

				  h q[0];
				  cx q[0], q[1];
				  measure q -> c;

				  Avoid examples (will fail in this runtime):
				  - qubit q[2];
				  - bit c0, c1;
				  - Declaring bit/qubit after gate operations.
				  """)]
		public required string OpenQasm3Program { get; set; }

		[ToolProperty(
			Description = "Number of shots (samples) to run. Defaults to 1024.")]
		public int Shots { get; set; } = 1024;
	}

	public class ExecuteQasm3ProgramResponse
	{
		[ToolProperty(
			Description = "Measurement counts keyed by bitstring"
						  + " outcome (for example, \"00\", \"01\", \"10\", \"11\").")]
		public Dictionary<string, int>? Counts { get; init; }
	}
}