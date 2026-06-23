using System.Runtime.CompilerServices;

using Orpius.Platform.Inferencing;
using Orpius.Platform.OperationsModel.RpcOperationsService;
using Orpius.Platform.RpcServices;

namespace Orpius.Samples.RealEstate
{
	static class ChatSupport
	{
		public static async IAsyncEnumerable<OperationMessageView> SendAsync(
			IOperationsService operationsService,
			ChatRequest chatRequest,
			MarkdownRenderer markdownRenderer,
			[EnumeratorCancellation] CancellationToken token)
		{
			await foreach (ChatResponse response in
						   operationsService.Chat(chatRequest).WithCancellation(token))
			{
				Guid conversationId = response.ConversationId;

				if (response.SystemMessage is not null)
				{
					yield return CreateSystemMessageView(
						response.SystemMessage,
						conversationId);
				}

				if (response.AssistantMessage is not null)
				{
					yield return CreateAssistantMessageView(
						response.AssistantMessage,
						conversationId,
						markdownRenderer);
				}
			}
		}

		static OperationMessageView CreateAssistantMessageView(
			AssistantMessage assistantMessage,
			Guid conversationId,
			MarkdownRenderer markdownRenderer)
		{
			string text = NormaliseAssistantText(assistantMessage.Text);

			return new OperationMessageView
			{
				Role = OperationMessageRole.Assistant,
				Text = text,
				ConversationId = conversationId,
				Html = markdownRenderer.ToHtml(text)
			};
		}

		static OperationMessageView CreateSystemMessageView(
			SystemMessage systemMessage,
			Guid conversationId)
		{
			return new OperationMessageView
			{
				Role = OperationMessageRole.System,
				Text = systemMessage.Text ?? string.Empty,
				ToolName = systemMessage.ApiCallInfo?.PluginName,
				Success = systemMessage.ApiCallInfo?.Success,
				ConversationId = conversationId
			};
		}

		static string NormaliseAssistantText(string? text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return string.Empty;
			}

			return text
				   .Replace("\\r\\n", Environment.NewLine)
				   .Replace("\\n", Environment.NewLine)
				   .Replace("\\r", Environment.NewLine);
		}

		public static Guid? NormaliseConversationId(Guid? conversationId)
		{
			return conversationId is null || conversationId == Guid.Empty
					   ? null
					   : conversationId;
		}
	}
}