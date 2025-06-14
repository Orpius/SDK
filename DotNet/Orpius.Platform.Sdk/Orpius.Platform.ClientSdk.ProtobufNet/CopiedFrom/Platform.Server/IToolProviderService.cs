// ReSharper disable PartialTypeWithSinglePart
// ReSharper disable RedundantTypeDeclarationBody

using System.Collections.Generic;
using System.Threading.Tasks;

using Grpc.Core;

using ProtoBuf;
using ProtoBuf.Grpc;
using ProtoBuf.Grpc.Configuration;

using Orpius.Platform.Tooling.RpcToolProviderService;

namespace Orpius.Platform.RpcServices
{
	[Service("orpius.platform.v1.ToolProviderService")]
	public partial interface IToolProviderService
	{
		Task<UseToolResponse> UseTool(UseToolRequest request, CallContext context = default);


	}
}

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Orpius.Platform.Tooling.RpcToolProviderService
{
		
	[ProtoContract]
	public partial class UseToolRequest
	{
			public UseToolRequest()
	{
	}
	}
		
	[ProtoContract]
	public partial class UseToolResponse
	{
			public UseToolResponse()
	{
	}
	}
	
}

