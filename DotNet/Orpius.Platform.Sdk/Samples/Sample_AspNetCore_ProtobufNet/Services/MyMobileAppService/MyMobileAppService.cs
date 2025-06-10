using Orpius.Platform.OperationsModel.RpcOperationsService;
using Orpius.Platform.RpcServices;

using ProtoBuf.Grpc;
using Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius;

namespace Sample_AspNetCore_ProtobufNet.Services
{
	public class MyMobileAppService : IMyMobileAppService
	{
		readonly IOperationsService operationsClient;

		public MyMobileAppService(IOperationsService operationsClient)
		{
			this.operationsClient = operationsClient ?? throw new ArgumentNullException(nameof(operationsClient));
		}

		public async IAsyncEnumerable<ChatResponse> Chat(MobileAppChatRequest request, CallContext context = default)
		{
			ChatRequest chatRequest = new()
			{
				UserMessage = request.UserMessage,
				Tools = new List<Tool>
				{
					new(name: nameof(FlightStatusChecker)) {ToolPresence = ToolPresence.Required},
					new(name: "WeatherForecast") {ToolPresence = ToolPresence.Required}
				},
				ConversationId = request.ConversationId
			};

			await foreach (ChatResponse response in operationsClient.Chat(chatRequest))
			{
				yield return response;
			}
		}
	}
}
