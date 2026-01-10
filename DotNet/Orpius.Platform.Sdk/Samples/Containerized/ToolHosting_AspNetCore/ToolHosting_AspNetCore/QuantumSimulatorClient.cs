namespace ToolHosting_AspNetCore
{
	public sealed class QuantumSimulatorClient
	{
		readonly HttpClient httpClient;

		public QuantumSimulatorClient(HttpClient httpClient)
		{
			this.httpClient = httpClient;
		}

		public async Task<Dictionary<string, int>> ExecuteAsync(string openQasm3Program,
																int shots,
																CancellationToken token)
		{
			var response = await httpClient.PostAsJsonAsync(
							   "/execute",
							   new ExecuteRequest
							   {
								   Qasm  = openQasm3Program,
								   Shots = shots
							   },
							   cancellationToken: token);

			if (!response.IsSuccessStatusCode)
			{
				var errorText = await response.Content.ReadAsStringAsync(token);
				throw new InvalidOperationException(
					$"Quantum simulator call failed: {(int)response.StatusCode} {errorText}");
			}

			var payload = await response.Content.ReadFromJsonAsync<ExecuteResponse>(cancellationToken: token);

			if (payload?.Counts is null)
			{
				throw new InvalidOperationException(
					"Quantum simulator response was empty or invalid.");
			}

			return payload.Counts;
		}

		sealed class ExecuteRequest
		{
			public required string Qasm  { get; init; }
			public          int    Shots { get; init; } = 1024;
		}

		sealed class ExecuteResponse
		{
			public Dictionary<string, int>? Counts { get; init; }
		}
	}
}
