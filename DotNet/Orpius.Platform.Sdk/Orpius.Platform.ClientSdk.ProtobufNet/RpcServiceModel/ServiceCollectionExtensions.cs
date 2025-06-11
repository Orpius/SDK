
using Microsoft.Extensions.DependencyInjection;

namespace Orpius.Platform.RpcServiceModel
{
	static class ServiceCollectionExtensions
	{
		internal static void AddAssociatedSingletons<TInterface, TImplementation>(this IServiceCollection services)
			where TInterface : class
			where TImplementation : class, TInterface
		{
			services.AddSingleton<TImplementation>();
			services.AddSingleton<TInterface>(sp => sp.GetRequiredService<TImplementation>());
		}
	}
}
