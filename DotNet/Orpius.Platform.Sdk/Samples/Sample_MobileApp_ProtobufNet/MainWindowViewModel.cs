using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

using Avalonia.Controls;

using Grpc.Net.Client;

using Orpius.Platform.Inferencing;
using Orpius.Platform.OperationsModel.RpcOperationsService;

using ProtoBuf.Grpc.Client;

using Sample_AspNetCore_ProtobufNet.Services;

namespace SampleProtobufNet
{
	public class MainWindowViewModel : ViewModelBase, IDisposable
	{
		public MainWindowViewModel()
		{
			if (Design.IsDesignMode)
			{
				Messages.Add(new UserMessage { Text = "User says something." });
				Messages.Add(new AssistantMessage { Text = "Assistant says something." });
			}
		}

		readonly object channelCreationLock = new();
		GrpcChannel? grpcChannel;
		IMyMobileAppService? grpcClient;
		Guid? conversationId;

		public ObservableCollection<IChatMessage> Messages { get; } = new();

		/// <summary>
		/// The Sample_AspNetCore_ProtobufNet server URL.
		/// </summary>
		string? serverUrl = "https://localhost:7194/";

		public string? ServerUrl
		{
			get => serverUrl;
			set
			{
				if (Set(ref serverUrl, value))
				{
					ResetChannel();
				}
			}
		}

		string? promptText;

		public string? PromptText
		{
			get => promptText;
			set => Set(ref promptText, value);
		}

		ActionCommand? sendMessageCommand;

		public ICommand SendMessageCommand => sendMessageCommand ??= new ActionCommand(SendMessage);

		void SendMessage(object? arg)
		{
			_ = SendMessageAsync();
		}

		async Task SendMessageAsync()
		{
			try
			{
				await SendMessageCoreAsync();
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}

		async Task SendMessageCoreAsync()
		{
			if (string.IsNullOrWhiteSpace(promptText))
			{
				return;
			}

			if (string.IsNullOrWhiteSpace(serverUrl))
			{
				throw new InvalidOperationException("Server URL is not set.");
			}

			IMyMobileAppService client = GetClient();

			// Orpius's user message. This is sent on to the Orpius server
			UserMessage userMessage = new()
			{
				Text = promptText
			};
			
			// Your server's request message.
			// This is defined by you, and contains information
			// relevant to your application.
			// You will probably place authentication information,
			// such as an access token, in the headers of the request.
			MobileAppChatRequest chatRequest = new()
			{
				UserMessage    = userMessage,
				ConversationId = conversationId
			};

			bool userMessageAdded = false;

			// Your server relays the messages back from the Orpius server.
			// The messages may contain zero or more API calls for tools,
			// and 1 or more messages from the assistant.
			await foreach (ChatResponse response in client.Chat(chatRequest))
			{
				if (!userMessageAdded)
				{
					Messages.Add(userMessage);
					PromptText       = string.Empty;
					userMessageAdded = true;
				}

				conversationId = response.ConversationId;

				SystemMessage? systemMessage = response.SystemMessage;

				if (systemMessage is not null)
				{
					Messages.Add(systemMessage);
				}

				AssistantMessage? assistantMessage = response.AssistantMessage;

				if (assistantMessage is not null)
				{
					Messages.Add(assistantMessage);
				}
			}
		}

		GrpcChannelOptions GetChannelOptions()
		{
			return new GrpcChannelOptions
			{
				HttpHandler = new HttpClientHandler
				{
					/* For use with self-signed certificate. Not for production. */
					ServerCertificateCustomValidationCallback
						= HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
				}
			};
		}

		IMyMobileAppService GetClient()
		{
			lock (channelCreationLock)
			{
				if (grpcClient is not null)
				{
					return grpcClient;
				}

				if (string.IsNullOrWhiteSpace(serverUrl))
				{
					throw new InvalidOperationException("serverUrl cannot be null or whitespace.");
				}

				grpcChannel = GrpcChannel.ForAddress(serverUrl, GetChannelOptions());
				grpcClient  = grpcChannel.CreateGrpcService<IMyMobileAppService>();

				return grpcClient;
			}
		}

		void ResetChannel()
		{
			lock (channelCreationLock)
			{
				GrpcChannel? oldChannel = Interlocked.Exchange(ref grpcChannel, null);
				grpcClient = null;
				oldChannel?.Dispose();
			}
		}

		public void Dispose()
		{
			GrpcChannel? ch = Interlocked.Exchange(ref grpcChannel, null);
			ch?.Dispose();
		}

		ActionCommand? startNewConversationCommand;

		public ICommand StartNewConversationCommand 
			=> startNewConversationCommand ??= new(StartNewConversation);

		void StartNewConversation(object? arg)
		{
			conversationId = null;
			Messages.Clear();
		}
	}
}