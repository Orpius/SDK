using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Orpius.Platform.Tooling.ToolRegistration;

namespace Sample_AspNetCore_ProtobufNet.RpcServiceModel
{
	public class RegistrationHostedService : IHostedService
	{
		readonly IToolRegistry toolRegistry;
		readonly ILogger<RegistrationHostedService> logger;
		bool connected;

		public RegistrationHostedService(IToolRegistry toolRegistry,
										 ILogger<RegistrationHostedService> logger,
										 IHostApplicationLifetime lifetime)
		{
			this.toolRegistry = toolRegistry ?? throw new ArgumentNullException(nameof(toolRegistry));
			this.logger       = logger ?? throw new ArgumentNullException(nameof(logger));

			lifetime.ApplicationStarted.Register(
				() => { RegisterAsync().ConfigureAwait(false); });
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}

		async Task RegisterAsync()
		{
			try
			{
				await toolRegistry.RegisterWithServerAsync();

				logger.LogInformation("Successfully registered tools with Orpius server.");

				connected = true;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Unable to register as tool provider with Orpius server.");
			}
		}

		public async Task StopAsync(CancellationToken cancellationToken)
		{
			if (!connected)
			{
				return;
			}

			try
			{
				await toolRegistry.DeregisterWithServerAsync(cancellationToken);

				connected = false;
			}
			catch (Exception ex)
			{
				logger.LogWarning(ex, "Unable to deregister as tool provider with Orpius server.");
			}
		}
	}
}