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
						ExternalId = Guid.Parse("f3842aba-4757-17d9-9a37-d9b918e33579"),
						AccessKey = Guid.Parse("6bce02a0-eeb2-64f0-c17a-df23dcce0379")
					}
			};

			await foreach (ChatResponse response in operationsClient.Chat(chatRequest))
			{
				yield return response;
			}
		}
	}
}
