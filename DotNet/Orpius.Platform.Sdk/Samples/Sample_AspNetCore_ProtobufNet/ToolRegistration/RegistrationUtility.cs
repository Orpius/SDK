using Orpius.Platform.RpcServices;
using Orpius.Platform.ToolsModel.RpcToolsRegistrationService;

using Sample_AspNetCore_ProtobufNet.RpcServiceModel;

namespace Sample_AspNetCore_ProtobufNet.ToolRegistration
{
	class RegistrationUtility
	{
		readonly IToolsRegistrationService registrationService;
		readonly IServiceProvider serviceProvider;

		public RegistrationUtility(IToolsRegistrationService registrationService, 
								   IServiceProvider serviceProvider)
		{
			this.registrationService = registrationService 
									   ?? throw new ArgumentNullException(nameof(registrationService));
			this.serviceProvider = serviceProvider;
		}

		internal async Task RegisterAsProviderAsync()
		{
			var toolsAndContracts = new GeneratedToolInfo().GetToolsAndContracts();

			var localUrl = new ApplicationUrlResolver().GetApplicationUrl(serviceProvider);

			RegisterAsProviderRequest request
				= new()
				{
					Tools     = toolsAndContracts.Tools,
					Contracts = toolsAndContracts.Contracts,
					ProviderUrl = localUrl
				};

			await registrationService.RegisterAsProvider(request);
		}
	}
}
