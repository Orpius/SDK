using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Orpius.Platform.Collections;

namespace Orpius.Platform.Tooling.ToolRegistration
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
				IEnumerable<RegistrationResult>? results = await toolRegistry.RegisterWithServerAsync().ConfigureAwait(false);
				bool hasErrors = false;

				if (results != null)
				{
					foreach (RegistrationResult registrationResult in results)
					{
						if (registrationResult.Error != null)
						{
							logger.LogWarning(registrationResult.Error, 
								"Unable to register toolset with Orpius server. ExternalId: {ExternalId}", 
								registrationResult.ExternalId);
							hasErrors = true;
						}
					}
				}

				if (hasErrors)
				{
					logger.LogWarning("Some toolsets could not be registered with Orpius server.");
				}
				else
				{
					logger.LogInformation("Successfully registered tools with Orpius server.");
				}

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
				var results = await toolRegistry.DeregisterWithServerAsync(cancellationToken).ConfigureAwait(false);

				bool hasErrors = false;
				
				if (results != null)
				{
					foreach (var result in results)
					{
						if (result.Error != null)
						{
							hasErrors = true;

							logger.LogWarning(result.Error, 
								"Unable to deregister toolset with Orpius server. ExternalId: {ExternalId}", 
								result.ExternalId);
						}
					}
				}

				if (hasErrors)
				{
					logger.LogWarning("Some toolsets could not be deregistered from Orpius server.");
				}
				else
				{
					logger.LogInformation("Successfully deregistered tools with Orpius server.");
				}

				connected = false;
			}
			catch (Exception ex)
			{
				logger.LogWarning(ex, "Unable to deregister as tool provider with Orpius server.");
			}
		}
	}
}