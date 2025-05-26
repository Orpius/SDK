using System.Reflection;

using ProtoBuf.Grpc.Configuration;

namespace Sample_AspNetCore_ProtobufNet
{
	class BinderFromServices : ServiceBinder
	{
		readonly IServiceCollection services;

		public BinderFromServices(IServiceCollection services)
		{
			this.services = services;
		}

		public override IList<object> GetMetadata(MethodInfo method, Type contractType, Type serviceType)
		{
			Type resolvedServiceType = serviceType;

			if (serviceType.IsInterface)
			{
				resolvedServiceType = services.SingleOrDefault(descriptor => descriptor.ServiceType == serviceType)?.ImplementationType
									  ?? serviceType;
			}

			return base.GetMetadata(method, contractType, resolvedServiceType);
		}
	}
}