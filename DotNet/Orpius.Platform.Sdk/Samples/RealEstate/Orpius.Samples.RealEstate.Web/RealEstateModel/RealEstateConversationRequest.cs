using Orpius.Platform.OperationsModel.RpcOperationsService;

namespace Orpius.Samples.RealEstate
{
	public class RealEstateConversationRequest
	{
		public required string MessageText { get; set; }

		public Guid? ConversationId { get; set; }

		public required List<Tool> Tools { get; set; }

		public Dictionary<string, string> SharedContext { get; set; } = new();

		public bool ShowMessageAsUserMessage { get; set; }
	}
}