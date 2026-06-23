using System.Diagnostics;

namespace Orpius.Samples.RealEstate
{
	public class NotificationService
	{
		readonly ILogger<NotificationService> logger;

		public NotificationService(ILogger<NotificationService> logger)
		{
			this.logger = logger
						  ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task SendAsync(NotificationMessage message,
									CancellationToken token = default)
		{
			ArgumentNullException.ThrowIfNull(message);

			string debugMessage =
				$"""
				 Real estate agent notification
				 RealEstateAgentId: {message.RealEstateAgentId}
				 Subject: {message.Subject}
				 Body:
				 {message.Body}
				 """;

			logger.LogInformation(
				"Real estate agent notification. RealEstateAgentId: {RealEstateAgentId}. Subject: {Subject}. Body: {Body}",
				message.RealEstateAgentId,
				message.Subject,
				message.Body);

			// TODO: Add an SMTP mailer here.
			Debug.WriteLine(debugMessage);

			await Task.CompletedTask;
		}
	}

	public class NotificationMessage
	{
		public Guid RealEstateAgentId { get; set; }

		public required string Subject { get; set; }

		public required string Body { get; set; }
	}
}