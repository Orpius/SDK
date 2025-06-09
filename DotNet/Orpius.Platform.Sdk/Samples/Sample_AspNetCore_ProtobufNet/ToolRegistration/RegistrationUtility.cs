using Orpius.Platform.RpcServices;
using Orpius.Platform.Tooling.RpcToolsRegistrationService;
using Sample_AspNetCore_ProtobufNet.DevelopmentOnly;
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
			var toolsAndContracts = new GeneratedToolInfo().ToolsMetadata;

			var localUrl = new ApplicationUrlResolver().GetApplicationUrl(serviceProvider);

			RegisterAsProviderRequest request
				= new(providerUrl: localUrl, ProgrammingLanguageId.CSharp)
				{
					Tools     = toolsAndContracts.Tools.ToList(),
					Contracts = toolsAndContracts.Contracts.ToList()
				};

			await registrationService.RegisterAsProvider(request);
		}
	}
}
