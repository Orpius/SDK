using System;
using System.Collections.Generic;

using Orpius.Platform.ToolsModel.RpcToolsRegistrationService;

namespace Orpius.Platform.Tooling.ToolRegistration
{
	public interface IToolsMetadata
	{
		IReadOnlyCollection<ToolMessage>     Tools     { get; }
		IReadOnlyCollection<ContractMessage> Contracts { get; }
	}

	public class ToolsMetadata : IToolsMetadata
	{
		public IReadOnlyCollection<ToolMessage>     Tools     { get; }
		public IReadOnlyCollection<ContractMessage> Contracts { get; }

		public ToolsMetadata(IReadOnlyCollection<ToolMessage> tools, 
							 IReadOnlyCollection<ContractMessage> contracts)
		{
			Tools     = tools     ?? throw new ArgumentNullException(nameof(tools));
			Contracts = contracts ?? throw new ArgumentNullException(nameof(contracts));
		}
	}
}