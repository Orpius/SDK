// ReSharper disable PartialTypeWithSinglePart
// ReSharper disable RedundantTypeDeclarationBody

using System.Collections.Generic;
using System.Threading.Tasks;

using Grpc.Core;

using ProtoBuf;
using ProtoBuf.Grpc;
using ProtoBuf.Grpc.Configuration;

using Orpius.Platform.OperationsModel.RpcOperationsService;

namespace Orpius.Platform.RpcServices
{
	[Service("orpius.platform.v1.OperationsService")]
	public partial interface IOperationsService
	{

		IAsyncEnumerable<ChatResponse> Chat(ChatRequest request, CallContext context = default);

	}
}

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Orpius.Platform.OperationsModel.RpcOperationsService
{
		
	[ProtoContract]
	public partial class ChatRequest
	{
		
	}
		
	[ProtoContract]
	public partial class ChatResponse
	{
		
	}

}

