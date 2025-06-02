using Orpius.Platform.RpcServices;
using Orpius.Platform.ToolsModel.RpcToolsRegistrationService;

namespace Sample_AspNetCore_ProtobufNet.ToolRegistration
{
	class RegistrationUtility
	{
		readonly IToolsRegistrationService registrationService;

		public RegistrationUtility(IToolsRegistrationService registrationService)
		{
			this.registrationService = registrationService 
									   ?? throw new ArgumentNullException(nameof(registrationService));
		}

		internal async Task RegisterAsProviderAsync()
		{
			var toolsAndContracts = new GeneratedToolInfo().GetToolsAndContracts();

			RegisterAsProviderRequest request
				= new()
				{
					Tools     = toolsAndContracts.Tools,
					Contracts = toolsAndContracts.Contracts,
					ProviderUrl = ApplicationState.ServerUrl
				};

			await registrationService.RegisterAsProvider(request);
		}
	}
}
