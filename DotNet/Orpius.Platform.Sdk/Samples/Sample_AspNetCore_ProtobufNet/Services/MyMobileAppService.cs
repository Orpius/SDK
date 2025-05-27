using Orpius.Platform.OperationsModel.RpcOperationsService;
using Orpius.Platform.RpcServices;

using ProtoBuf.Grpc;

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
				OperationCredentials
					= new OperationCredentials
					{
						ExternalId = ApplicationState.ExternalId,
						AccessKey = ApplicationState.AccessKey
					}
			};

			await foreach (ChatResponse response in operationsClient.Chat(chatRequest))
			{
				yield return response;
			}
		}
	}
}
