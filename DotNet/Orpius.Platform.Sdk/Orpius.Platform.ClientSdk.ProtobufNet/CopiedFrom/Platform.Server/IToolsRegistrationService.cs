// ReSharper disable PartialTypeWithSinglePart
// ReSharper disable RedundantTypeDeclarationBody

using System.Collections.Generic;
using System.Threading.Tasks;

using Grpc.Core;

using ProtoBuf;
using ProtoBuf.Grpc;
using ProtoBuf.Grpc.Configuration;

using Orpius.Platform.Tooling.RpcToolsRegistrationService;

namespace Orpius.Platform.RpcServices
{
	[Service("orpius.platform.v1.ToolsRegistrationService")]
	public partial interface IToolsRegistrationService
	{
		Task<RegisterAsProviderResponse> RegisterAsProvider(RegisterAsProviderRequest request, CallContext context = default);
		Task<DeregisterAsProviderResponse> DeregisterAsProvider(DeregisterAsProviderRequest request, CallContext context = default);


	}
}

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Orpius.Platform.Tooling.RpcToolsRegistrationService
{
		
	[ProtoContract]
	public partial class RegisterAsProviderRequest
	{
		
	}
		
	[ProtoContract]
	public partial class RegisterAsProviderResponse
	{
		
	}
	
		
	[ProtoContract]
	public partial class DeregisterAsProviderRequest
	{
		
	}
		
	[ProtoContract]
	public partial class DeregisterAsProviderResponse
	{
		
	}
	
}

