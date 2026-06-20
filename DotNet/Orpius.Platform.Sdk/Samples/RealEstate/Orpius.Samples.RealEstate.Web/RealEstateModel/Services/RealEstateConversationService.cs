using System.Runtime.CompilerServices;

using Orpius.Platform.Inferencing;
using Orpius.Platform.OperationsModel.RpcOperationsService;
using Orpius.Platform.RpcServices;

namespace Orpius.Samples.RealEstate
{
	public class RealEstateConversationService
	{
		readonly IOperationsService operationsClient;
		readonly OrpiusSampleOptions sampleOptions;
		readonly IRealEstateAgentAuthenticationService authenticationService;

		public RealEstateConversationService(
			IOperationsService operationsClient,
			OrpiusSampleOptions sampleOptions,
			IRealEstateAgentAuthenticationService realEstateAgentAuthenticationService)
		{
			this.operationsClient = operationsClient
									?? throw new ArgumentNullException(nameof(operationsClient));

			this.sampleOptions = sampleOptions
								 ?? throw new ArgumentNullException(nameof(sampleOptions));

			this.authenticationService = realEstateAgentAuthenticationService
														?? throw new ArgumentNullException(
															nameof(realEstateAgentAuthenticationService));
		}

		public async IAsyncEnumerable<OperationMessageView> AddApplicantFromTextAsync(
			string emailAddress,
			string applicantText,
			[EnumeratorCancellation] CancellationToken token)
		{
			string prompt =
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

			Guid realEstateAgentId = authenticationService.GetCurrentRealEstateAgentId();

			Dictionary<string, string> sharedContext = new()
			{
				[RealEstateContextKeys.ApplicantEmailAddress] = emailAddress.Trim(),
				[RealEstateContextKeys.RealEstateAgentId]     = realEstateAgentId.ToString()
			};

			List<Tool> tools = new()
			{
				new(name: nameof(ApplicantRegistrar))
				{
					ToolPresence = ToolPresence.Required
				}
			};

			await foreach (OperationMessageView message in SendToOrpiusAsync(
							   prompt,
							   tools,
							   sharedContext,
							   token))
			{
				yield return message;
			}
		}

		public async IAsyncEnumerable<OperationMessageView> AddListingFromTextAsync(
			string listingText,
			[EnumeratorCancellation] CancellationToken token)
		{
			string prompt =
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

			List<Tool> tools = new()
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
			};

			await foreach (OperationMessageView message in SendToOrpiusAsync(
							   prompt,
							   tools,
							   new Dictionary<string, string>(),
							   token))
			{
				yield return message;
			}
		}

		async IAsyncEnumerable<OperationMessageView> SendToOrpiusAsync(
			string prompt,
			List<Tool> tools,
			Dictionary<string, string> sharedContext,
			[EnumeratorCancellation] CancellationToken token)
		{
			UserMessage userMessage = new()
			{
				Text = prompt
			};

			yield return new OperationMessageView
			{
				Role = OperationMessageRole.User,
				Text = prompt
			};

			ChatRequest chatRequest = new(
				operationExternalId: sampleOptions.Operations.ExternalId,
				userMessage: userMessage)
			{
				Tools = tools,
				Context = sharedContext
			};

			await foreach (ChatResponse response in
						   operationsClient.Chat(chatRequest).WithCancellation(token))
			{
				SystemMessage? systemMessage = response.SystemMessage;

				if (systemMessage is not null)
				{
					yield return CreateSystemMessageView(systemMessage);
				}

				AssistantMessage? assistantMessage = response.AssistantMessage;

				if (assistantMessage is not null)
				{
					yield return new OperationMessageView
					{
						Role = OperationMessageRole.Assistant,
						Text = assistantMessage.Text
					};
				}
			}
		}

		static OperationMessageView CreateSystemMessageView(SystemMessage systemMessage)
		{
			return new OperationMessageView
			{
				Role = OperationMessageRole.System,
				Text = systemMessage.Text,
				ToolName = systemMessage.ApiCallInfo?.PluginName,
				Success = systemMessage.ApiCallInfo?.Success
			};
		}
	}
}