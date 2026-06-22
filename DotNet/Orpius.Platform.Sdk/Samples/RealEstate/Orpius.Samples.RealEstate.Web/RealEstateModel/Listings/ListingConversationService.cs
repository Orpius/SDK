using System.Runtime.CompilerServices;

using Orpius.Platform.OperationsModel.RpcOperationsService;

namespace Orpius.Samples.RealEstate
{
	public class ListingConversationService
	{
		readonly RealEstateConversationService conversationService;

		public ListingConversationService(
			RealEstateConversationService conversationService)
		{
			this.conversationService = conversationService
									   ?? throw new ArgumentNullException(nameof(conversationService));
		}

		public async IAsyncEnumerable<OperationMessageView> AddListingFromTextAsync(
			string listingText,
			Guid? conversationId,
			string? jsonProvidedToAgent,
			[EnumeratorCancellation] CancellationToken token)
		{
			RealEstateConversationRequest request = new()
			{
				MessageText              = CreateMessageText(listingText, conversationId),
				ConversationId           = conversationId,
				ShowMessageAsUserMessage = false,
				JsonProvidedToAgent = ShouldProvideImageToAgent(conversationId)
										  ? jsonProvidedToAgent
										  : null,
				Tools = new List<Tool>
				{
					new(name: nameof(PropertyLister))
					{
						ToolPresence = ToolPresence.Required
					},
					new(name: nameof(ApplicantRegistrar))
					{
						ToolPresence = ToolPresence.Required
					},
					new(name: nameof(RealEstateAgentMessenger))
					{
						ToolPresence = ToolPresence.Required
					}
				}
			};

			await foreach (OperationMessageView message in conversationService.SendAsync(
							   request,
							   token))
			{
				yield return message;
			}
		}

		static bool ShouldProvideImageToAgent(Guid? conversationId)
		{
			return conversationId is null || conversationId == Guid.Empty;
		}

		static string CreateMessageText(string listingText,
										Guid? conversationId)
		{
			bool newConvo = conversationId    == null
							|| conversationId == Guid.Empty;

			if (newConvo)
			{
				return
					$"""
					 The estate agent has entered the following new property listing.

					 You are working behind a customer-facing real estate application.

					 Visibility rule:
					 Do not show your reasoning.
					 Do not explain your internal steps.
					 Do not mention tools, API calls, plugins, JSON, code execution, or implementation details.
					 Do not write messages such as "now I will", "next step", "I need to", or "with that apiCall".
					 Perform the required actions silently.
					 Only provide a customer-facing final message after the full workflow is complete.

					 Completion rule:
					 Do not stop after registering the listing.
					 Do not stop after retrieving applicants.
					 Do not stop after deciding matches.
					 Do not send a final response until every required step below has either been completed
					 or explicitly skipped because it does not apply.

					 Required process:
					 1. Register the property listing using PropertyLister.RegisterPropertyListing.
					 2. Retrieve applicant profiles using ApplicantRegistrar.GetApplicantProfilesForMatching.
					 3. Compare the registered listing against the applicants.
					 4. If one or more applicants match, record those matches using ApplicantRegistrar.RecordListingMatches.
					 5. If matches were recorded successfully, send notifications using RealEstateAgentMessenger.SendNotification.
					 6. Create a web page in the web/properties directory for the new property.

					 Rules for applicant matching:
					 If no applicant profiles are returned, skip steps 4 and 5.
					 If applicant profiles are returned but none match, skip steps 4 and 5.
					 If any applicants match, you must call ApplicantRegistrar.RecordListingMatches before sending notifications.

					 Rules for notifications:
					 Use RealEstateAgentMessenger for real estate agents.
					 Do not use the built-in Notifier for real estate agents.
					 Real estate agents are identified by RealEstateAgentId values from the third-party application,
					 not by Orpius user IDs.
					 When notifying an agent, mention the matched applicants using the applicant labels returned
					 by RecordListingMatches, such as J Smith or A Brown.

					 Rules for the web page:
					 The web page must be created even if there are no applicant matches.
					 Create the web page after the listing has been registered and after applicant matching
					 has been completed or skipped.
					 Only include the city or area on the page.
					 Do not include the full street address because that is private.
					 The page should be attractive and should extol the property's best features.

					 Final customer-facing response:
					 After everything is complete, write a short, polished message for the estate agent.
					 Do not include a checklist unless it sounds natural.
					 Do not mention tool names or API calls.
					 Do not mention the private street address.
					 If a property page was created, include the page link.

					 Listing details:
					 {listingText}
					 """;
			}

			return
				$"""
				 Additional listing details have been provided for the current property listing.

				 Continue the existing listing workflow.
				 Perform any remaining required actions silently.
				 Do not explain your reasoning, tools, API calls, or internal process.
				 Only provide a customer-facing message after the listing workflow is complete.

				 Additional listing details:
				 {listingText}
				 """;
		}
	}
}