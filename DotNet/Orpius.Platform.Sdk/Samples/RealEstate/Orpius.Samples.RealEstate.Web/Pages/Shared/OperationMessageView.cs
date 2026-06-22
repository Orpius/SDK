namespace Orpius.Samples.RealEstate
{
	public enum OperationMessageRole
	{
		User,
		Assistant,
		System
	}

	public class OperationMessageView
	{
		public OperationMessageRole Role { get; set; }

		public required string Text { get; set; }

		public string? ToolName { get; set; }

		public bool? Success { get; set; }

		public Guid? ConversationId { get; set; }

		public string? Html { get; set; }
	}
}
