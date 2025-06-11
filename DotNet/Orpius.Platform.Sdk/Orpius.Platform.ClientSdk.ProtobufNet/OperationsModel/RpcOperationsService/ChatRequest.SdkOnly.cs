using System;

namespace Orpius.Platform.OperationsModel.RpcOperationsService
{
	public interface IOperationExternalIdProvider
	{
		/// <summary>
		/// The external ID of the Operation.
		/// </summary>
		Guid OperationExternalId { get; }
	}

	partial class ChatRequest : IOperationExternalIdProvider
	{
	}
}
