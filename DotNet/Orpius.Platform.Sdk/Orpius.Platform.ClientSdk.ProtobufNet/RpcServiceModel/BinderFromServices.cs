using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

using ProtoBuf.Grpc.Configuration;

namespace Orpius.Platform.RpcServiceModel
{
	public class BinderFromServices : ServiceBinder
	{
		readonly IServiceCollection services;

		public BinderFromServices(IServiceCollection services)
		{
			this.services = services;
		}

		public override IList<object> GetMetadata(MethodInfo method,
												  Type contractType,
												  Type serviceType)
		{
			Type resolvedServiceType = serviceType;

			if (serviceType.IsInterface)
			{
				resolvedServiceType
					= services.SingleOrDefault(sd => sd.ServiceType == serviceType)?.ImplementationType
					  ?? serviceType;
			}

			return base.GetMetadata(method, contractType, resolvedServiceType);
		}
	}
}
