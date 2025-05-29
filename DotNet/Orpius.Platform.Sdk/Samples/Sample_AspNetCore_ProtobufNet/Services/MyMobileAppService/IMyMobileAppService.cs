using System.Collections.Generic;

using Orpius.Platform.Inferencing;
using Orpius.Platform.OperationsModel.RpcOperationsService;

using ProtoBuf;
using ProtoBuf.Grpc;
using ProtoBuf.Grpc.Configuration;

namespace Sample_AspNetCore_ProtobufNet.Services
{
	//[Service("mycompany.myapplication.v1.MyMobileAppService")]
	[Service("MyMobileAppService")]
	public interface IMyMobileAppService
	{
		IAsyncEnumerable<ChatResponse> Chat(MobileAppChatRequest request, CallContext context = default);
	}

	[ProtoContract]
	public class MobileAppChatRequest
	{
		[ProtoMember(1, IsRequired = true)]
		public required UserMessage UserMessage { get; set; }
	}

	[ProtoContract]
	public class MobileAppChatResponse
	{
		[ProtoMember(1, IsRequired = true)]
		public required UserMessage UserMessage { get; set; }
	}
}
