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
			[EnumeratorCancellation] CancellationToken token)
		{
			RealEstateConversationRequest request = new()
			{
				MessageText = CreateMessageText(listingText, conversationId),
				ConversationId = conversationId,
				ShowMessageAsUserMessage = false,
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

		static string CreateMessageText(string listingText,
										Guid? conversationId)
		{
			bool newConvo = conversationId == null
							|| conversationId == Guid.Empty;

			if (newConvo)
			{
				return
					$"""
					 The estate agent has entered the following new property listing.

					 Follow this process:
					 1. Register the property listing using PropertyLister.RegisterPropertyListing.
					 2. Retrieve applicant profiles using ApplicantRegistrar.GetApplicantProfilesForMatching.
					 3. Compare the registered listing against the applicants.
					 4. Record any matching applicants using ApplicantRegistrar.RecordListingMatches.
					 5. After RecordListingMatches succeeds, send notifications using RealEstateAgentMessenger.SendNotification.

					 Important notification rule:
					 Use RealEstateAgentMessenger for real estate agents.
					 Do not use the built-in Notifier for real estate agents.
					 Real estate agents are identified by RealEstateAgentId values from the third-party application,
					 not by Orpius user IDs.

					 When notifying an agent, mention the matched applicants using the applicant labels returned
					 by RecordListingMatches, such as J Smith or A Brown.

					 Listing details:
					 {listingText}
					 """;
			}

			return
				$"""
				 Additional listing details:
				 {listingText}
				 """;
		}
	}
}