using System.Runtime.CompilerServices;

using Orpius.Platform.OperationsModel.RpcOperationsService;

namespace Orpius.Samples.RealEstate
{
	public class ApplicantConversationService
	{
		readonly RealEstateConversationService conversationService;
		readonly IRealEstateAgentIdentityService identityService;

		public ApplicantConversationService(
			RealEstateConversationService conversationService,
			IRealEstateAgentIdentityService identityService)
		{
			this.conversationService = conversationService
									   ?? throw new ArgumentNullException(nameof(conversationService));

			this.identityService = identityService
										 ?? throw new ArgumentNullException(nameof(identityService));
		}

		public async IAsyncEnumerable<OperationMessageView> AddApplicantFromTextAsync(
			string emailAddress,
			string applicantText,
			Guid? conversationId,
			[EnumeratorCancellation] CancellationToken token)
		{
			Guid realEstateAgentId = identityService.GetCurrentRealEstateAgentId();

			RealEstateConversationRequest request = new()
			{
				MessageText = CreateMessageText(applicantText, conversationId),
				ConversationId = conversationId,
				ShowMessageAsUserMessage = false,
				Tools = new List<Tool>
				{
					new(name: nameof(ApplicantRegistrar))
					{
						ToolPresence = ToolPresence.Required
					}
				},
				SharedContext = new Dictionary<string, string>
				{
					[RealEstateContextKeys.ApplicantEmailAddress] = emailAddress.Trim(),
					[RealEstateContextKeys.RealEstateAgentId] = realEstateAgentId.ToString()
				}
			};

			await foreach (OperationMessageView message
						   in conversationService.SendAsync(request, token))
			{
				yield return message;
			}
		}

		static string CreateMessageText(string applicantText,
										Guid? conversationId)
		{
			bool newConvo = conversationId    == null
							|| conversationId == Guid.Empty;

			if (newConvo)
			{
				return
					$"""
					 The real-estate agent has entered the following applicant details.

					 Register the applicant using the available applicant registration tool.
					 Extract the applicant's first name, surname, and ideal property features.

					 The applicant's email address and the current real estate agent identifier
					 have already been supplied securely to the tool context.
					 Do not ask for the email address.
					 Do not include the email address in the agent-visible conversation.

					 Applicant details:
					 {applicantText}
					 """;
			}

			return
				$"""
				 Additional applicant details:
				 {applicantText}
				 """;
		}
	}
}