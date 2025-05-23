using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;

using Avalonia.Controls;

using Grpc.Net.Client;

using Orpius.Platform.OperationsModel.RpcOperationsService;
using Orpius.Platform.RpcServices;

using ProtoBuf.Grpc.Client;

namespace SampleProtobufNet
{
	public class MainWindowViewModel : ViewModelBase
	{
		public MainWindowViewModel()
		{
			if (Design.IsDesignMode)
			{
				Utterances.Add(new UserUtterance() {Text  = "User says something."});
				Utterances.Add(new AgentUtterance() { Text = "Assistant says something." });
			}
		}
		string? serverUrl = "https://localhost:32774";

		public string? ServerUrl
		{
			get => serverUrl;
			set => Set(ref serverUrl, value);
		}

		string? promptText;

		public string? PromptText
		{
			get => promptText;
			set => Set(ref promptText, value);
		}

		ActionCommand? sendUtteranceCommand;

		public ICommand SendUtteranceCommand => sendUtteranceCommand ??= new(SendUtterance);

		void SendUtterance(object? arg)
		{
			_ = SendUtteranceAsync();
		}

		async Task SendUtteranceAsync()
		{
			try
			{
				await SendUtteranceCoreAsync();
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}

		async Task SendUtteranceCoreAsync()
		{
			if (string.IsNullOrWhiteSpace(promptText))
			{
				return;
			}

			if (string.IsNullOrWhiteSpace(serverUrl))
			{
				throw new InvalidOperationException("Server URL is not set.");
			}

			UserUtterance utterance = new() { Text = promptText };
			Utterances.Add(utterance);

			using var channel = GrpcChannel.ForAddress(serverUrl, GetChannelOptions());

			IOperationsService client = channel.CreateGrpcService<IOperationsService>();

			var request = new ConverseRequest { Text = promptText };

			await foreach (ConverseResponse response in client.Converse(request))
			{
				AgentUtterance agentUtterance = new() { Text = response.Text };
				Utterances.Add(agentUtterance);
			}

			PromptText = string.Empty;
		}

		GrpcChannelOptions GetChannelOptions()
		{
			HttpClientHandler httpHandler = new()
			{
				/* For use with self-signed certificate. Not for production. */
				ServerCertificateCustomValidationCallback 
					= HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
			};

			GrpcChannelOptions channelOptions = new()
			{
				HttpHandler = httpHandler,
			};

			return channelOptions;
		}

		public ObservableCollection<IUtterance> Utterances { get; } = new();
	}

	public interface IUtterance
	{
		string? Text { get; }
	}

	public class AgentUtterance : ViewModelBase, IUtterance
	{
		string? text;

		public required string? Text
		{
			get => text;
			set => Set(ref text, value);
		}
	}

	public class UserUtterance : ViewModelBase, IUtterance
	{
		string? text;

		public required string? Text
		{
			get => text;
			set => Set(ref text, value);
		}
	}
}
