using Orpius.Platform.ToolsModel.RpcToolsRegistrationService;

namespace Sample_AspNetCore_ProtobufNet.ToolRegistration
{
	class ToolsAndContracts
	{
		public IList<ToolMessage>         Tools     { get; }
		public IList<ToolContractMessage> Contracts { get; }

		public ToolsAndContracts(IList<ToolMessage> tools, IList<ToolContractMessage> contracts)
		{
			Tools     = tools     ?? throw new ArgumentNullException(nameof(tools));
			Contracts = contracts ?? throw new ArgumentNullException(nameof(contracts));
		}
	}
}