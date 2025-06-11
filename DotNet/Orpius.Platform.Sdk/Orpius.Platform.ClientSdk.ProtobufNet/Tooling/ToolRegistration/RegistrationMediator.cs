using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Orpius.Platform.RpcServices;
using Orpius.Platform.Tooling.RpcToolsRegistrationService;

namespace Orpius.Platform.Tooling.ToolRegistration
{
	public interface IRegistrationMediator
	{
		Task<RegisterAsProviderResponse> RegisterAsProviderAsync(
			RegisterAsProviderRequest request,
			CancellationToken token = default);

		Task<DeregisterAsProviderResponse> DeregisterAsProviderAsync(
			DeregisterAsProviderRequest request,
			CancellationToken token = default);
	}

	public class RegistrationMediator : IRegistrationMediator
	{
		readonly IToolsRegistrationService registrationService;

		public RegistrationMediator(IToolsRegistrationService registrationService)
		{
			this.registrationService = AssertArg.IsNotNull(registrationService, nameof(registrationService));
		}

		public async Task<RegisterAsProviderResponse> RegisterAsProviderAsync(
			RegisterAsProviderRequest request,
			CancellationToken token = default)
		{
			return await registrationService.RegisterAsProvider(request).ConfigureAwait(false);
		}

		public async Task<DeregisterAsProviderResponse> DeregisterAsProviderAsync(
			DeregisterAsProviderRequest request,
			CancellationToken token = default)
		{
			return await registrationService.DeregisterAsProvider(request).ConfigureAwait(false);
		}
	}
}
