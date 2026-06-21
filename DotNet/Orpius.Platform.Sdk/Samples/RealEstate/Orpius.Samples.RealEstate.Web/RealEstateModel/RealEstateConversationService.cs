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

		public RealEstateConversationService(
			IOperationsService operationsClient,
			OrpiusSampleOptions sampleOptions)
		{
			this.operationsClient = operationsClient
									?? throw new ArgumentNullException(nameof(operationsClient));

			this.sampleOptions = sampleOptions
								 ?? throw new ArgumentNullException(nameof(sampleOptions));
		}

		public async IAsyncEnumerable<OperationMessageView> SendAsync(
			RealEstateConversationRequest request,
			[EnumeratorCancellation] CancellationToken token)
		{
			ArgumentNullException.ThrowIfNull(request);

			UserMessage userMessage = new()
			{
				Text = request.MessageText
			};

			if (request.ShowMessageAsUserMessage)
			{
				yield return new OperationMessageView
				{
					Role           = OperationMessageRole.User,
					Text           = request.MessageText,
					ConversationId = request.ConversationId
				};
			}

			Guid? conversationId = NormaliseConversationId(request.ConversationId);

			ChatRequest chatRequest = new(
				operationExternalId: sampleOptions.Operations.ExternalId,
				userMessage: userMessage)
			{
				Tools          = request.Tools,
				Context        = request.SharedContext,
				ConversationId = conversationId
			};

			await foreach (ChatResponse response in
						   operationsClient.Chat(chatRequest).WithCancellation(token))
			{
				Guid returnedConversationId = response.ConversationId;

				SystemMessage? systemMessage = response.SystemMessage;

				if (systemMessage is not null)
				{
					yield return CreateSystemMessageView(
						systemMessage,
						returnedConversationId);
				}

				AssistantMessage? assistantMessage = response.AssistantMessage;

				if (assistantMessage is not null)
				{
					yield return new OperationMessageView
					{
						Role           = OperationMessageRole.Assistant,
						Text           = assistantMessage.Text ?? string.Empty,
						ConversationId = returnedConversationId
					};
				}
			}
		}

		static Guid? NormaliseConversationId(Guid? conversationId)
		{
			return conversationId is null
				   || conversationId == Guid.Empty
					   ? null
					   : conversationId;
		}

		static OperationMessageView CreateSystemMessageView(
			SystemMessage systemMessage,
			Guid conversationId)
		{
			return new OperationMessageView
			{
				Role           = OperationMessageRole.System,
				Text           = systemMessage.Text ?? string.Empty,
				ToolName       = systemMessage.ApiCallInfo?.PluginName,
				Success        = systemMessage.ApiCallInfo?.Success,
				ConversationId = conversationId
			};
		}
	}
}