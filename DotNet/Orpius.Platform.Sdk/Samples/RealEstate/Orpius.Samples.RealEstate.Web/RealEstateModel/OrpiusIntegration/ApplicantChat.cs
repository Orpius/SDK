using System.Runtime.CompilerServices;

using Orpius.Platform.Inferencing;
using Orpius.Platform.OperationsModel.RpcOperationsService;
using Orpius.Platform.RpcServices;

namespace Orpius.Samples.RealEstate
{
	public class ApplicantChat
	{
		readonly IOperationsService operationsService;
		readonly SampleOptions options;
		readonly IRealEstateAgentIdentityService identityService;
		readonly MarkdownRenderer markdownRenderer = new();

		public ApplicantChat(IOperationsService operationsService,
							 SampleOptions options,
							 IRealEstateAgentIdentityService identityService)
		{
			this.operationsService = operationsService ?? throw new ArgumentNullException(nameof(operationsService));
			this.options           = options           ?? throw new ArgumentNullException(nameof(options));
			this.identityService   = identityService   ?? throw new ArgumentNullException(nameof(identityService));
		}

		public async IAsyncEnumerable<OperationMessageView> AddApplicantFromTextAsync(
			string emailAddress,
			string applicantText,
			Guid? conversationId,
			[EnumeratorCancellation] CancellationToken token)
		{
			Guid realEstateAgentId = identityService.GetCurrentRealEstateAgentId();

			UserMessage userMessage = new()
			{
				Text = CreateMessageText(applicantText, conversationId)
			};

			ChatRequest chatRequest = new(
				operationExternalId: options.Operations.ExternalId,
				userMessage: userMessage)
			{
				ConversationId = ChatSupport.NormaliseConversationId(conversationId),
				Tools = new List<Tool>
				{
					new(name: nameof(ApplicantRegistrar))
					{
						ToolPresence = ToolPresence.Required
					}
				},
				Context = new Dictionary<string, string>
				{
					[ContextKeys.ApplicantEmailAddress] = emailAddress.Trim(),
					[ContextKeys.RealEstateAgentId]     = realEstateAgentId.ToString()
				}
			};

			await foreach (OperationMessageView message in ChatSupport.SendAsync(
							   operationsService,
							   chatRequest,
							   markdownRenderer,
							   token))
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