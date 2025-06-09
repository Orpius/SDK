using System;
using System.Linq;
using System.Threading.Tasks;

using Orpius.Platform.RpcServices;
using Orpius.Platform.Tooling.RpcToolsRegistrationService;

namespace Orpius.Platform.Tooling.ToolRegistration
{
	public interface IRegistrationMediator
	{
		Task<RegisterAsProviderResponse> RegisterAsProviderAsync(
			string localUrl,
			IToolsMetadata toolsMetadata);
	}

	public class RegistrationMediator : IRegistrationMediator
	{
		readonly IToolsRegistrationService registrationService;

		public RegistrationMediator(IToolsRegistrationService registrationService)
		{
			this.registrationService = AssertArg.IsNotNull(registrationService, nameof(registrationService));
		}

		public async Task<RegisterAsProviderResponse> RegisterAsProviderAsync(
			string localUrl, 
			IToolsMetadata toolsMetadata)
		{
			var toolsAndContracts = toolsMetadata;
			
			RegisterAsProviderRequest request
				= new RegisterAsProviderRequest(providerUrl: localUrl, ProgrammingLanguageId.CSharp)
				{
					Tools     = toolsAndContracts.Tools.ToList(),
					Contracts = toolsAndContracts.Contracts.ToList()
				};

			return await registrationService.RegisterAsProvider(request);
		}
	}
}
