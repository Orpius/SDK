using Orpius.Platform.Tooling;

namespace Orpius.Samples.RealEstate
{
	[Tool]
	public class RealEstateAgentMessenger
	{
		readonly NotificationService notificationService;

		public RealEstateAgentMessenger(NotificationService notificationService)
		{
			this.notificationService = notificationService
									   ?? throw new ArgumentNullException(nameof(notificationService));
		}

		[ToolMethod(Description
			= """
			  Send a notification to a real estate agent in the third-party real estate application.
			  Use this tool for real estate agents.
			  Do not use the built-in Notifier for real estate agents, because real estate agents
			  are identified by RealEstateAgentId values controlled by the third-party application,
			  not by Orpius user IDs.
			  """)]
		public async Task<SendRealEstateAgentNotificationResponse> SendNotification(
			SendRealEstateAgentNotificationRequest request,
			ICombinedContext context)
		{
			NotificationMessage message = new()
			{
				RealEstateAgentId = request.RealEstateAgentId,
				Subject = request.Subject,
				Body = request.Body
			};

			await notificationService.SendAsync(message);

			SendRealEstateAgentNotificationResponse response = new()
			{
				RealEstateAgentId = request.RealEstateAgentId,
				Sent = true
			};

			return response;
		}
	}

	public class SendRealEstateAgentNotificationRequest
	{
		[ToolProperty(
			Required = true,
			Description
				= """
				  The identifier of the real estate agent to notify.
				  This is a third-party application identity, not an Orpius user ID.
				  """)]
		public Guid RealEstateAgentId { get; set; }

		[ToolProperty(
			Required = true,
			Description = "The subject or title of the notification.")]
		public required string Subject { get; set; }

		[ToolProperty(
			Required = true,
			Description = "The main textual content of the notification.")]
		public required string Body { get; set; }
	}

	public class SendRealEstateAgentNotificationResponse
	{
		[ToolProperty(
			Required = true,
			Description = "The real estate agent identifier that the notification was sent to.")]
		public Guid RealEstateAgentId { get; set; }

		[ToolProperty(
			Required = true,
			Description = "Indicates whether the notification was sent.")]
		public bool Sent { get; set; }
	}
}