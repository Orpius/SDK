using Microsoft.Extensions.DependencyInjection;

using Orpius.Platform.RpcServiceModel;
using Orpius.Platform.RpcServices;
using Sample_AspNetCore_ProtobufNet.RpcServiceModel;

namespace Orpius.Platform.Tooling.ToolRegistration
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddToolRegistration(
			this IServiceCollection services)
		{
			/* We send the API Key and External ID to Orpius in the headers of the request.
			   This class relies on IToolRegistrationParameters.*/
			services.AddSingleton<RegistrationHeaderHandler>();

			/* This allows tools to be resolved using the IServiceProvider. */
			services.AddSingleton<IToolResolver>(sp => new ServiceProviderAdapter(sp));
			services.AddAssociatedSingletons<IToolRegistry, ToolRegistry>();

			/* ToolProviderService requires IToolCaller.
			   ToolRegistry also serves as the IToolCaller.
			   ToolProviderService is the GRPC service that receives
			   requests from Orpius to use tools. */
			services.AddSingleton<IToolCaller>(sp => sp.GetRequiredService<ToolRegistry>());
			services.AddAssociatedSingletons<IToolProviderService, ToolProviderService>();

			/* The IRegistrationMediator abstracts the use of the actual IToolRegistrationService. */
			services.AddSingleton<IRegistrationMediator, RegistrationMediator>();
			
			return services;
		}

		public static IServiceCollection WithAutomaticProviderRegistration(
			this IServiceCollection services)
		{
			services.AddHostedService<RegistrationHostedService>();

			return services;
		}

		internal static void AddAssociatedSingletons<TInterface, TImplementation>(this IServiceCollection services)
			where TInterface : class
			where TImplementation : class, TInterface
		{
			services.AddSingleton<TImplementation>();
			services.AddSingleton<TInterface>(sp => sp.GetRequiredService<TImplementation>());
		}
	}
}
