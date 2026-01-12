using ProtoBuf.Grpc.Configuration;
using System.Reflection;

namespace ToolHosting_AspNetCore.RpcServiceModel
{
	class BinderFromServices : ServiceBinder
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

	static class ServiceCollectionExtensions
	{
		internal static void AddAssociatedSingletons<TInterface, TImplementation>(
			this IServiceCollection services)
			where TInterface : class
			where TImplementation : class, TInterface
		{
			services.AddSingleton<TImplementation>();
			services.AddSingleton<TInterface>(sp => sp.GetRequiredService<TImplementation>());
		}
	}
}
